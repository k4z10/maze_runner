namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;

public class TestDungeonStrategy : IDungeonGenerationStrategy
{
    public IModifierDungeonBuilder GenerateTopology(IBaseDungeonBuilder baseBuilder, int width, int height)
    {
        return baseBuilder.CreateEmptyDungeon(width, height);
    }
}