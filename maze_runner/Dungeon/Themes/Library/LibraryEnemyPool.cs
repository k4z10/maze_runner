using maze_runner.Core;
using maze_runner.Entities;
using maze_runner.Entities.Mobs;
using maze_runner.Items.Models;

namespace maze_runner.Dungeon.Themes.Library;

public class LibraryEnemyPool : IEnemyPool
{
    private readonly WeightedPool<Func<IEventPublisher, IEventSubscriber, Entity>> _pool = new();
    private readonly GoblinTribe _goblinTribe = new();

    public LibraryEnemyPool()
    {
        _pool.Add(() => ((pub, sub) => new Goblin(_goblinTribe, pub, sub)), 1);
    }

    public Entity GetEntity(IEventPublisher eventPublisher, IEventSubscriber eventSubscriber)
    {
        var factoryMethod = _pool.Draw();
        return factoryMethod(eventPublisher, eventSubscriber);
    }
}