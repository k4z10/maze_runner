using maze_runner.Entities;
using maze_runner.Entities.Mobs;
using maze_runner.Items.Models;

namespace maze_runner.Dungeon.Themes.Library;

public class LibraryEnemyPool : IEnemyPool
{
    private readonly WeightedPool<Entity> _pool = new();

    public LibraryEnemyPool()
    {
        _pool.Add(() => new MainBoss(), 100);
    }

    public Entity GetEntity() => _pool.Draw();
}