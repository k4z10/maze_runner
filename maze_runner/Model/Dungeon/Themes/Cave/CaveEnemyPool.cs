using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Mobs;

namespace maze_runner.Model.Dungeon.Themes.Cave;

public class CaveEnemyPool : IEnemyPool
{
    private readonly GuaranteedWeightedPool<Func<EventBus, Entity>> _pool = new();
    private readonly GoblinTribe _goblinTribe = new();
    private readonly SkeletonTribe _skeletonTribe = new();

    public CaveEnemyPool()
    {
        _pool.Add(() => (bus => new Goblin(_goblinTribe, bus)), 1);
        _pool.Add(() => (bus => new Skeleton(_skeletonTribe, bus)), 1);
    }

    public Entity GetEntity(EventBus eventBus)
    {
        var factoryMethod = _pool.Draw();
        return factoryMethod(eventBus);
    }
}