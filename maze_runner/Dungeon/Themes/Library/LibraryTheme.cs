using maze_runner.Dungeon.Strategies;
using maze_runner.Items.Models;
using maze_runner.Items.Weapons;

namespace maze_runner.Dungeon.Themes.Library;

public class LibraryTheme : IDungeonThemeFactory
{
    public string ThemeName => "Forgotten Library";
    public string IntroMessage => "There is mystical knowledge everywhere, go get it!";
    public IDungeonGenerationStrategy CreateMapGenerator() => new EasyDungeonStrategy();

    public IItemPool CreateItemPool() => new LibraryItemPool();

    public IEnemyPool CreateEnemyPool() => new LibraryEnemyPool();

    public Item GetArtifact() => new Cubix();
}