namespace maze_runner.Core.Engine;
using Entities.Player;
using Dungeon.Map;
using Dungeon.Strategies;
using Commands.Core;

public class GameEngine : IGameContext
{
    public Player Player { get; private set; }
    public Map CurrentMap { get; private set; }
    public LevelContext CurrentLevelContext { get; private set; }
    
    private readonly IGameUIManager _uiManager;

    public GameEngine(Player player)
    {
        Player = player;
        var ctx = new InitialDungeonStrategy().Generate(40, 20);
        CurrentLevelContext = ctx;
        CurrentMap = ctx.Map;
        
        var inputHandler = new InputHandler();
        _uiManager = new TerminalUIManager(this, inputHandler);
    }
    
    public void LoadLevel(IDungeonGenerationStrategy strategy, int width = 40, int height = 20)
    {
        var ctx = strategy.Generate(width, height);
        Player = new Player();
        CurrentLevelContext = ctx;
        CurrentMap = ctx.Map;
        CurrentMap.RegisterEntity(Player);
        Player.Position = (0, 0);
    }

    public void Run() => _uiManager.InitializeAndRun();
}