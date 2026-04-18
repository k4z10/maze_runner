namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;

public class TestDungeonStrategy : IDungeonGenerationStrategy
{
    public ILevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var ctx = builder.CreateEmptyDungeon(width, height)
            .AddWeapons(10)
            .AddUselessItems(10)
            .Build();
        
        ctx.Description = "This is test level.";
        
        return ctx;
    }
}