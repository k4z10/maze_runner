using maze_runner.Entities;

namespace maze_runner.Core.Engine;
using Entities.Player;
using Dungeon.Map;
using Dungeon.Strategies;
using Commands.Core;

public class GameEngine : IGameContext
{
    public EntityManager EntityManager { get; private set; }
    public Map CurrentMap { get; private set; }
    public LevelContext CurrentLevelContext { get; private set; }
    
    private readonly IGameUIManager _uiManager;
    private Player _player;

    public GameEngine(Player player)
    {
        _player = player;
        var ctx = new InitialDungeonStrategy().Generate(40, 20);
        CurrentLevelContext = ctx;
        CurrentMap = ctx.Map;
        
        EntityManager = ctx.EntityManager;
        EntityManager.RegisterPlayer(player);
        
        var inputHandler = new InputHandler();
        _uiManager = new TerminalUIManager(this, inputHandler);
    }
    
    public void LoadLevel(IDungeonGenerationStrategy strategy, int width = 40, int height = 20)
    {
        var ctx = strategy.Generate(width, height);
        CurrentLevelContext = ctx;
        CurrentMap = ctx.Map;
        EntityManager = ctx.EntityManager;
        EntityManager.RegisterPlayer(_player);
        
        EntityManager.Player.Position = CurrentMap.GetSpawningPosition();
    }

    public void Run() => _uiManager.InitializeAndRun();
}