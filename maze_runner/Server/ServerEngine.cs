using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using maze_runner.Model.Core;
using maze_runner.Model.Core.Logger;
using maze_runner.Model.Dungeon;
using maze_runner.Model.Entities;
using maze_runner.Network.DTOs.Actions;

namespace maze_runner.Server;

public class ServerEngine(GameConfig config, int port) : IGameContext
{
    public GameConfig Config { get; } = config;
    public ILevelContext CurrentLevel { get; private set; } = new LevelContext();
    public bool IsRunning { get; private set; } = true;
    public void StopServer() => IsRunning = false;
    
    private readonly ConcurrentQueue<ActionRequestDto> _actionQueue = new();
    
    private readonly Lock _gameLock = new();

    private readonly ConcurrentDictionary<int, StreamWriter> _clientWriters = new();
    private readonly ConcurrentQueue<int> _availablePids = new(Enumerable.Range(0, 2));

    private readonly DungeonDirector _director = new();
    private readonly GameConfig _config = config;
    private readonly FileLogger _fileLogger = new(config);
    private readonly List<string> _logsThisTick = new();

    public void LoadLevel(IDungeonThemeFactory theme, int itemsCount, int enemiesCount, int width = 40, int height = 20)
    {
        lock (_gameLock)
        {
            CurrentLevel = _director.ConstructLevel(theme, itemsCount, enemiesCount, width, height);
            CurrentLevel.EventBus.OnLogGenerated = m => { lock(_gameLock) { _fileLogger.Log(m); _logsThisTick.Add(m); } };
            
            foreach (var playerId in _clientWriters.Keys)
                SpawnPlayer(playerId);
        }
    }

    public async Task StartServerAsync(CancellationToken token = default)
    {
        _ = Task.Run(GameLoop, token);

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        try
        {
            listener.Start();
            _fileLogger.Log($"[Serwer] Nasłuchiwanie na porcie {port}...");
        }
        catch (Exception ex)
        {
            _fileLogger.Log($"[Serwer] FATAL Port niedostępny: {ex.Message}");
            return;
        }

        while (IsRunning)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(token);
                _ = Task.Run(() => HandleClientAsync(client, token), token);
            }
            catch (OperationCanceledException) {}
            catch (Exception ex)
            {
                _fileLogger.Log($"[Serwer] Błąd akceptacji klienta: {ex.Message}");
            }
        }
        
        listener.Stop();
    }

    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token = default)
    {
        if (!_availablePids.TryDequeue(out var playerId))
        {
            _fileLogger.Log("[Serwer] Odrzucono połączenie - osiągnięto limit 10 graczy.");
            tcpClient.Close();
            return;
        }
        
        _fileLogger.Log($"[Serwer] Gracz {playerId} połączony.");

        var stream = tcpClient.GetStream();
        var writer = new StreamWriter(stream) { AutoFlush = true };
        var reader = new StreamReader(stream);

        _clientWriters.TryAdd(playerId, writer);

        await writer.WriteLineAsync(playerId.ToString()); // handshake

        Player player;
        lock (_gameLock)
        {
            player = SpawnPlayer(playerId);
        }
        
        BroadcastState();

        try
        {
            while (IsRunning && tcpClient.Connected && !token.IsCancellationRequested)
            {
                string? json = await reader.ReadLineAsync(token);
                if (string.IsNullOrEmpty(json)) break;

                var request = JsonSerializer.Deserialize<ActionRequestDto>(json);
                if (request != null)
                {
                    var securedRequest = request with { PlayerId = playerId };
                    _actionQueue.Enqueue(securedRequest);
                }
            }
        }
        catch (OperationCanceledException) {}
        finally
        {
            _fileLogger.Log($"[Serwer] Gracz {playerId} rozłączony.");
            _clientWriters.TryRemove(playerId, out _);
            
            lock (_gameLock)
            {
                CurrentLevel.EntityManager.RemoveEntity(player);
                player.Dispose();
            }
            _availablePids.Enqueue(playerId);
            tcpClient.Close();
        }
    }

    private Player SpawnPlayer(int playerId)
    {
        var newPlayer = new Player($"{_config.PlayerName} {playerId}", CurrentLevel.EventBus) { Id = playerId, Symbol = char.Parse(playerId.ToString())};
        CurrentLevel.EntityManager.RegisterEntity(newPlayer);
        newPlayer.Position = CurrentLevel.Map.GetSpawningPosition();
        return newPlayer;
    }

    private void GameLoop()
    {
        const double targetTps = 60.0;
        const double optimalTimeMs = 1000.0 / targetTps;

        var stopwatch = Stopwatch.StartNew();
        double lastTime = stopwatch.Elapsed.TotalMilliseconds;

        while (IsRunning)
        {
            double currentTime = stopwatch.Elapsed.TotalMilliseconds;
            double deltaTime = currentTime - lastTime;

            if (deltaTime >= optimalTimeMs)
            {
                lastTime = currentTime;
                
                lock (_gameLock)
                {
                    ProcessInput();
                    Update(deltaTime);
                }
                
                BroadcastState();
            }
            else
            {
                int sleepTime = (int)(optimalTimeMs - deltaTime);
                if (sleepTime > 0) Thread.Sleep(sleepTime);
            }
        }
    }

    private void ProcessInput()
    {
        while (_actionQueue.TryDequeue(out var request))
        {
            if (CurrentLevel.CommandRegistry.Handlers.TryGetValue(request.CommandId, out var command))
            {
                command.Execute(this.CurrentLevel, request.PlayerId);
            }
        }
    }

    private void Update(double deltaTime)
    {
        CurrentLevel.EntityManager.RemoveDeadEntities();

        foreach (var entity in CurrentLevel.EntityManager.Entities)
        {
            entity.UpdateState(CurrentLevel, deltaTime);
        }
    }

    private void BroadcastState()
    {
        string jsonPayload;

        lock (_gameLock)
        {
            var snapshot = SnapshotGenerator.GenerateSnapshot(CurrentLevel) with { NewLogs = _logsThisTick.ToList() };
            jsonPayload = JsonSerializer.Serialize(snapshot) + "\n";
            
            _logsThisTick.Clear();
        }

        foreach (var writer in _clientWriters.Values)
        {
            try
            {
                writer.Write(jsonPayload);
            }
            catch
            {
                // Zignoruj wyjątkowe zablokowania TCP, Cleanup usunie writer w HandleClientAsync
            }
        }
    }
}