using maze_runner.Core;
using maze_runner.Dungeon.Builders;

namespace maze_runner.Dungeon;

public class DungeonDirector
{
    public ILevelContext ConstructLevel(IDungeonThemeFactory theme, int width, int height)
    {
        var builder = new ProcDungeonBuilder();

        var strategy = theme.CreateMapGenerator();
        var topologyModifier = strategy.GenerateTopology(builder, width, height);

        var levelContext = topologyModifier
            .PopulateItems(theme.CreateItemPool(), count: 10)
            .PopulateEnemies(theme.CreateEnemyPool(), count: 1)
            .PlaceArtifact(theme.GetArtifact())
            .GetLevelContext();

        levelContext.Description = theme.IntroMessage;
        levelContext.LevelName = theme.ThemeName;

        return levelContext;
    }
}