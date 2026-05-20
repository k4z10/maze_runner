using maze_runner.Core;
using maze_runner.Model.Dungeon.Builders;

namespace maze_runner.Model.Dungeon;

public class DungeonDirector
{
    public ILevelContext ConstructLevel(IDungeonThemeFactory theme, int itemsCount, int enemiesCount, int width = 40, int height = 20)
    {
        var builder = new ProcDungeonBuilder();

        var strategy = theme.CreateMapGenerator();
        var topologyModifier = strategy.GenerateTopology(builder, width, height);

        var levelContext = topologyModifier
            .PopulateItems(theme.CreateItemPool(), count: itemsCount)
            .PopulateEnemies(theme.CreateEnemyPool(), count: enemiesCount)
            .PlaceArtifact(theme.GetArtifact())
            .GetLevelContext();

        levelContext.Description = theme.IntroMessage;
        levelContext.LevelName = theme.ThemeName;

        return levelContext;
    }
}