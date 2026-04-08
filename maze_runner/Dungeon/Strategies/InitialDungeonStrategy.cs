namespace maze_runner.Dungeon.Strategies;
using Core;
using Terminal.Gui;
using Commands.Core;
using Builders;
using Commands;

public class InitialDungeonStrategy : IDungeonGenerationStrategy
{
    public LevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var ctx = builder.CreateEmptyDungeon(width, height).Build();
        
        string levelDescription = "Initial level.";
        
        return new LevelContext(ctx.Item1, ctx.Item2, levelDescription);
    }
}