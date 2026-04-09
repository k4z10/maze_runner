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
    public EventBus EventBus { get; }
    public IMessageLog Logger { get; }
    public ILevelContext CurrentLevel { get; private set; }
    
    private readonly IGameUIManager _uiManager;
    private readonly Player _player;

    public GameEngine(Player player, GameConfig config, EventBus eventBus, IMessageLog logger)
    {
        Config = config;
        EventBus = eventBus;
        Logger = logger;
        
        _player = player;
        var ctx = new InitialDungeonStrategy().Generate(40, 20);
        CurrentLevel = ctx;
        CurrentLevel.EntityManager.RegisterPlayer(player);
        
        var inputHandler = new InputHandler();
        _uiManager = new TerminalUIManager(this, inputHandler);
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