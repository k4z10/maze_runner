using maze_runner.Model.Dungeon.Strategies;
using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Dungeon;

public interface IDungeonThemeFactory
{
    string ThemeName { get; }
    string IntroMessage { get; }

    IDungeonGenerationStrategy CreateMapGenerator();
    IItemPool CreateItemPool();
    IEnemyPool CreateEnemyPool();
    Item GetArtifact();
}