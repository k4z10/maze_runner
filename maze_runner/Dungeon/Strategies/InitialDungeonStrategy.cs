namespace maze_runner.Dungeon.Strategies;
using Core;
using Terminal.Gui;
using Commands.Core;
using Builders;
using Commands;

public class InitialDungeonStrategy : IDungeonGenerationStrategy
{
    public IModifierDungeonBuilder GenerateTopology(IBaseDungeonBuilder baseBuilder, int width, int height)
    {
        return baseBuilder.CreateEmptyDungeon(width, height);
    }
}