namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;
using Map;
public interface IDungeonGenerationStrategy
{
    ILevelContext Generate(int width, int height);
}




