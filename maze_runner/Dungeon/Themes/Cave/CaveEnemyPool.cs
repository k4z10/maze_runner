using maze_runner.Entities;
using maze_runner.Entities.Mobs;

namespace maze_runner.Dungeon.Themes.Cave;

public class CaveEnemyPool : IEnemyPool
{
    private readonly GuaranteedWeightedPool<Entity> _pool = new();

    public CaveEnemyPool()
    {
        _pool.Add(() => new Goblin("Goblin"), 1);
        _pool.Add(() => new Skeleton("Skeleton"), 1);
    }

    public Entity GetEntity() => _pool.Draw();
}