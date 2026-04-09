using maze_runner.Core;
using maze_runner.Dungeon.Strategies;
using maze_runner.Entities.Player;

namespace maze_runner;
using Core.Engine;

static class Program
{
    static void Main(string[] args)
    {
        var config = ConfigLoader.Load("config.json");
        var player = new Player();
        var engine = new GameEngine(player, config);
        engine.LoadLevel(new EasyDungeonStrategy());
        // engine.LoadLevel(new TestDungeonStrategy());
        engine.Run();
    }
}