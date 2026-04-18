using maze_runner.Core;
using maze_runner.Core.Logger;
using maze_runner.Dungeon.Strategies;
using maze_runner.Dungeon.Themes.Library;
using maze_runner.Entities.Player;

namespace maze_runner;
using Core.Engine;

static class Program
{
    static void Main(string[] args)
    {
        var config = ConfigLoader.Load("config.json");
        var player = new Player();
        
        GameEvents.LogBridge(new FileLog(config));
        
        var engine = new GameEngine(player, config);
        engine.LoadLevel(new LibraryTheme());
        // engine.LoadLevel(new TestDungeonStrategy());
        engine.Run();
    }
}