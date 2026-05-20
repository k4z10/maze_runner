using maze_runner.Model.Dungeon.Builders;

namespace maze_runner.Model.Dungeon.Strategies;

public class TestDungeonStrategy : IDungeonGenerationStrategy
{
    public IModifierDungeonBuilder GenerateTopology(IBaseDungeonBuilder baseBuilder, int width, int height)
    {
        return baseBuilder.CreateEmptyDungeon(width, height);
    }
}