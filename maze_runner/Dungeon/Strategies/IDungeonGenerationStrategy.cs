namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;
using Map;
public interface IDungeonGenerationStrategy
{
    IModifierDungeonBuilder GenerateTopology(IBaseDungeonBuilder baseBuilder,int width, int height);
}




