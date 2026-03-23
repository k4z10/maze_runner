namespace maze_runner.Dungeon.Strategies;
using Builders;
using Map;
public interface IDungeonGenerationStrategy
{
    Map Generate(IBaseDungeonBuilder builder, int width, int height);    
}




