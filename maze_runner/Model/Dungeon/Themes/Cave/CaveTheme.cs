using maze_runner.Model.Dungeon.Strategies;
using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Dungeon.Themes.Cave;

public class CaveTheme : IDungeonThemeFactory
{
    public string ThemeName => "Cave";
    public string IntroMessage => "Long, dark and mysterious abyss...";
    public IDungeonGenerationStrategy CreateMapGenerator() => new EasyDungeonStrategy();

    public IItemPool CreateItemPool() => new CaveItemPool();

    public IEnemyPool CreateEnemyPool() => new CaveEnemyPool();

    public Item GetArtifact() => new Coin(100);
}