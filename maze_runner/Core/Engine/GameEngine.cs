using maze_runner.Core.Logger;
using maze_runner.Dungeon;
using maze_runner.Entities;

namespace maze_runner.Core.Engine;
using Entities.Player;
using Dungeon.Map;
using Commands.Core;

public class GameEngine : IGameContext
{
    public GameConfig Config { get; }
    public ILevelContext CurrentLevel { get; private set; }
    public MemoryLogger Logs { get; private set; }

    public InputHandler GlobalInput { get; } = new();
    
    private readonly IGameUIManager _uiManager;
    private readonly Player _player;
    private readonly DungeonDirector _director = new();

    public GameEngine(Player player, GameConfig config, MemoryLogger logger)
    {
        Config = config;
        Logs = logger;
        _player = player;
        _player.Name = config.PlayerName;
        CurrentLevel = new LevelContext(new Map(), new InputHandler(), new EntityManager());
        CurrentLevel.EntityManager.RegisterPlayer(player);
        _uiManager = new TerminalUIManager(this);
    }
    
    public void LoadLevel(IDungeonThemeFactory theme, int width = 40, int height = 20)
    {
        CurrentLevel = _director.ConstructLevel(theme, width, height);
        CurrentLevel.EntityManager.RegisterPlayer(_player);
        _player.Position = CurrentLevel.Map.GetSpawningPosition();
    }

    public void Run() => _uiManager.InitializeAndRun();
}