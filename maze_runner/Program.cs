namespace maze_runner;
using Dungeon.Strategies;
using Dungeon.Builders;
using Core;

static class Program
{
    static void Main(string[] args)
    {
        var map = new EasyDungeonStrategy().Generate(new ProcDungeonBuilder(), 40, 20);
        
        var engine = new GameEngine(map);
        engine.Run();
    }
}