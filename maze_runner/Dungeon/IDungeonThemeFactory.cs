using maze_runner.Dungeon.Strategies;
using maze_runner.Items.Models;

namespace maze_runner.Dungeon;

public interface IDungeonThemeFactory
{
    string ThemeName { get; }
    string IntroMessage { get; }

    IDungeonGenerationStrategy CreateMapGenerator();
    IItemPool CreateItemPool();
    IEnemyPool CreateEnemyPool();
    Item GetArtifact();
}