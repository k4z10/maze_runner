namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;
using Commands.Core;
using Terminal.Gui;
using Commands;

public class TestDungeonStrategy : IDungeonGenerationStrategy
{
    public LevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var ctx = builder.CreateEmptyDungeon(width, height)
            .AddWeapons(10)
            .AddUselessItems(10)
            .Build();
        
        string levelDescription = "This is test level.";
        
        return new LevelContext(ctx.Item1, ctx.Item2, levelDescription);
    }
}