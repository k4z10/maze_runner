namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;
using Map;
public interface IDungeonGenerationStrategy
{
    LevelContext Generate(int width, int height); 
}




