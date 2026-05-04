using maze_runner.Entities;
using maze_runner.Entities.Mobs;

namespace maze_runner.Dungeon.Themes.Cave;

public class CaveEnemyPool : IEnemyPool
{
    private readonly GuaranteedWeightedPool<Entity> _pool = new();
    private readonly GoblinTribe _goblinTribe = new();
    private readonly SkeletonTribe _skeletonTribe = new();

    public CaveEnemyPool()
    {
        _pool.Add(() => new Goblin(_goblinTribe), 1);
        _pool.Add(() => new Skeleton(_skeletonTribe), 1);
    }

    public Entity GetEntity() => _pool.Draw();
}