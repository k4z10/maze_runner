using maze_runner.Core;
using maze_runner.Core.Logger;
using maze_runner.Dungeon.Strategies;
using maze_runner.Dungeon.Themes.Cave;
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

        var memoryLogger = new MemoryLogger();
        UniversalLogChannel.ConnectLogger(new CompositeLogger(new FileLogger(config), memoryLogger));
        
        var engine = new GameEngine(player, config, memoryLogger);
        engine.LoadLevel(new CaveTheme(), 10, 1);
        engine.Run();
    }
}