namespace maze_runner.Dungeon.Strategies;
using Core;
using Terminal.Gui;
using Commands.Core;
using Builders;
using Commands;

public class InitialDungeonStrategy : IDungeonGenerationStrategy
{
    public ILevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var ctx = builder.CreateEmptyDungeon(width, height).Build();
        
        ctx.Description = "Initial level.";        
        
        return ctx;
    }
}