using System.Collections.Concurrent;
using maze_runner.Core.Frontend;
using maze_runner.Core.Frontend.Raylib;
using maze_runner.Core.Logger;
using maze_runner.Dungeon;
using maze_runner.Entities;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace maze_runner.Core.Engine;
using Entities.Player;
using Dungeon.Map;
using Commands.Core;

public class GameEngine : IGameContext
{
    public GameConfig Config { get; }
    public ILevelContext CurrentLevel { get; private set; }
    public MemoryLogger Logger { get; private set; }

    private readonly ConcurrentQueue<char> _inputQueue = new();
    private volatile bool _isRunning = true;
    public bool IsRunning => _isRunning;

    public InputHandler GlobalInput { get; } = new();
    
    private readonly IGameFrontend _uiManager;
    private readonly Player _player;
    private readonly DungeonDirector _director = new();

    public GameEngine(Player player, GameConfig config, MemoryLogger logger)
    {
        Config = config;
        Logger = logger;
        _player = player;
        _player.Name = config.PlayerName;
        CurrentLevel = new LevelContext();
        CurrentLevel.EntityManager.RegisterPlayer(player);
        _uiManager = new RaylibFrontend(this);
    }

    public void EnqueueInput(char key) => _inputQueue.Enqueue(key);
    public void RequestQuit() => _isRunning = false;
    
    public void LoadLevel(IDungeonThemeFactory theme, int items, int enemies, int width = 40, int height = 20)
    {
        CurrentLevel = _director.ConstructLevel(theme, items, enemies, width, height);
        CurrentLevel.EntityManager.RegisterPlayer(_player);
        _player.Position = CurrentLevel.Map.GetSpawningPosition();
    }

    public void Run()
    {
        Task.Run(GameLoop);
        _uiManager.InitializeAndRun();
    }



    private void GameLoop()
    {
        const double targetTps = 60.0;
        const double optimalTimeMs = 1000.0 / targetTps;

        var stopwatch = Stopwatch.StartNew();
        double lastTime = stopwatch.Elapsed.TotalMilliseconds;

        while (_isRunning)
        {
            double currentTime = stopwatch.Elapsed.TotalMilliseconds;
            double deltaTime = currentTime - lastTime;

            if (deltaTime >= optimalTimeMs)
            {
                lastTime = currentTime;
                
                ProcessInput();

                // Faza symulacji: Aktualizacja encji z uwzględnieniem upływu czasu (Delta Time)
                Update(deltaTime);
            }
            else
            {
                int sleepTime = (int)(optimalTimeMs - deltaTime);
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
            }
            
            if (!_isRunning) return;
        }
    }

    private void ProcessInput()
    {
        while (_inputQueue.TryDequeue(out var input))
        {
            if (!CurrentLevel.InputHandler.ProcessInput(input))
                CurrentLevel.EventBus.Publish(new UnknownInputEvent(input));
        }
    }

    private void Update(double deltaTime)
    {
        foreach (var entity in CurrentLevel.EntityManager.AllEntities)
        {
            if (entity == _player) continue;
            if (Random.Shared.NextDouble() < 0.01)
            {
                var(dRow, dCol) = (Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2)); 
                var map = CurrentLevel.Map;
                if (!map.GetTile(entity.Position.Row + dRow, entity.Position.Col + dCol).TryEnter()) continue;
                CurrentLevel.EntityManager.MoveEntity(entity, entity.Position.Row + dRow, entity.Position.Col + dCol, map);
            }
        }
    }
}