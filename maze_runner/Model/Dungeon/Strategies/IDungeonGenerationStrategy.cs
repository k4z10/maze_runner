using maze_runner.Model.Dungeon.Builders;

namespace maze_runner.Model.Dungeon.Strategies;

public interface IDungeonGenerationStrategy
{
    IModifierDungeonBuilder GenerateTopology(IBaseDungeonBuilder baseBuilder,int width, int height);
}




