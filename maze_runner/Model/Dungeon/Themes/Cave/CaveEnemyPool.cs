using maze_runner.Core;
using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Mobs;

namespace maze_runner.Model.Dungeon.Themes.Cave;

public class CaveEnemyPool : IEnemyPool
{
    private readonly GuaranteedWeightedPool<Func<IEventPublisher, IEventSubscriber, Entity>> _pool = new();
    private readonly GoblinTribe _goblinTribe = new();
    private readonly SkeletonTribe _skeletonTribe = new();

    public CaveEnemyPool()
    {
        _pool.Add(() => ((pub, sub) => new Goblin(_goblinTribe, pub, sub)), 1);
        _pool.Add(() => ((pub, sub) => new Skeleton(_skeletonTribe, pub, sub)), 1);
    }

    public Entity GetEntity(IEventPublisher eventPublisher, IEventSubscriber eventSubscriber)
    {
        var factoryMethod = _pool.Draw();
        return factoryMethod(eventPublisher, eventSubscriber);
    }
}