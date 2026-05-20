using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using maze_runner.Core;
using maze_runner.Model.Dungeon;
using maze_runner.Model.Entities.Player;
using maze_runner.Network.DTOs.Actions;
using maze_runner.ServerEngine;

namespace maze_runner.Server;

public class ServerEngine(GameConfig config, int port) : IGameContext
{
    public GameConfig Config { get; } = config;
    public ILevelContext CurrentLevel { get; private set; } = new LevelContext();
    public bool IsRunning { get; private set; } = true;
    
    private readonly ConcurrentQueue<ActionRequestDto> _actionQueue = new();
    
    private readonly Lock _gameLock = new();

    private readonly ConcurrentDictionary<int, StreamWriter> _clientWriters = new();
    private int _nextPlayerId = 1;

    private readonly DungeonDirector _director = new();

    public void LoadLevel(IDungeonThemeFactory theme, int itemsCount, int enemiesCount, int width = 40, int height = 20)
    {
        lock (_gameLock)
        {
            CurrentLevel = _director.ConstructLevel(theme, itemsCount, enemiesCount, width, height);
            
            foreach (var playerId in _clientWriters.Keys)
                SpawnPlayer(playerId);
        }
    }

    public async Task StartServerAsync()
    {
        _ = Task.Run(GameLoop);

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[Serwer] Nasłuchiwanie na porcie {port}...");

        while (IsRunning)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serwer] Błąd akceptacji klienta: {ex.Message}");
            }
        }
        
        listener.Stop();
    }

    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        int playerId = Interlocked.Increment(ref _nextPlayerId);
        Console.WriteLine($"[Serwer] Gracz {playerId} połączony.");

        var stream = tcpClient.GetStream();
        var writer = new StreamWriter(stream) { AutoFlush = true };
        var reader = new StreamReader(stream);

        _clientWriters.TryAdd(playerId, writer);

        await writer.WriteLineAsync(playerId.ToString());

        Player player;
        lock (_gameLock)
        {
            player = SpawnPlayer(playerId);
        }
        
        BroadcastState();

        try
        {
            while (IsRunning && tcpClient.Connected)
            {
                string? json = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(json)) break; // Klient się rozłączył

                var request = JsonSerializer.Deserialize<ActionRequestDto>(json);
                if (request != null)
                {
                    // Wymuszenie identyfikatora sesji, aby klient nie mógł wysłać akcji w imieniu innego gracza
                    var securedRequest = request with { PlayerId = playerId };
                    _actionQueue.Enqueue(securedRequest);
                }
            }
        }
        catch (Exception)
        {
            // Ignorujemy błędy zerwanego potoku
        }
        finally
        {
            Console.WriteLine($"[Serwer] Gracz {playerId} rozłączony.");
            _clientWriters.TryRemove(playerId, out _);
            
            lock (_gameLock)
            {
                CurrentLevel.EntityManager.RemovePlayer(player);
            }
            tcpClient.Close();
        }
    }

    private Player SpawnPlayer(int playerId)
    {
        // Zakładam modyfikację w EntityManager z poprzednich kroków
        var newPlayer = new Player($"Gracz {playerId}") { Id = playerId };
        CurrentLevel.EntityManager.RegisterPlayer(newPlayer);
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
        // Usuwanie martwych bytów z uwzględnieniem wielu graczy
        CurrentLevel.EntityManager.RemoveDeadEntities();

        foreach (var mod in CurrentLevel.EntityManager.Mobs)
        {
            if (Random.Shared.NextDouble() < 0.05)
            {
                var (dRow, dCol) = (Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2)); 
                var map = CurrentLevel.Map;
                
                int targetRow = mod.Position.Row + dRow;
                int targetCol = mod.Position.Col + dCol;

                if (!map.GetTile(targetRow, targetCol).IsWalkable) continue;
                
                CurrentLevel.EntityManager.MoveEntity(mod, targetRow, targetCol);
            }
        }
    }

    private void BroadcastState()
    {
        string jsonPayload;

        lock (_gameLock)
        {
            var snapshot = SnapshotGenerator.GenerateSnapshot(CurrentLevel);
            jsonPayload = JsonSerializer.Serialize(snapshot) + "\n";
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