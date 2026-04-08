using maze_runner.Dungeon.Strategies;

namespace maze_runner;
using Core.Engine;

static class Program
{
    static void Main(string[] args)
    {
        var player = new Entities.Player.Player();
        var engine = new GameEngine(player);
        engine.LoadLevel(new EasyDungeonStrategy());
        // engine.LoadLevel(new TestDungeonStrategy());
        engine.Run();
    }
}