using maze_runner.Core.Logger;
using maze_runner.Entities;
using Microsoft.Extensions.Logging;

namespace maze_runner.Core.Engine;
using Entities.Player;
using Dungeon.Map;
using Dungeon.Strategies;
using Commands.Core;

public class GameEngine : IGameContext
{
    public GameConfig Config { get; }
    public ILevelContext CurrentLevel { get; private set; }
    public MemoryLog Logs { get; private set; }

    public InputHandler GlobalInput { get; } = new();
    
    private readonly IGameUIManager _uiManager;
    private readonly Player _player;

    public GameEngine(Player player, GameConfig config)
    {
        Config = config;
        Logs = new MemoryLog();
        GameEvents.LogBridge(Logs);
        
        _player = player;
        _player.Name = config.PlayerName;
        var ctx = new InitialDungeonStrategy().Generate(40, 20);
        CurrentLevel = ctx;
        CurrentLevel.EntityManager.RegisterPlayer(player);
        
        _uiManager = new TerminalUIManager(this);
    }
    
    public void LoadLevel(IDungeonGenerationStrategy strategy, int width = 40, int height = 20)
    {
        var ctx = strategy.Generate(width, height);
        CurrentLevel = ctx;
        CurrentLevel.EntityManager.RegisterPlayer(_player);
        _player.Position = CurrentLevel.Map.GetSpawningPosition();
    }

    public void Run() => _uiManager.InitializeAndRun();
}