using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Hostile;

namespace maze_runner.Model.Dungeon.Themes.Library;

public class LibraryEnemyPool : IEnemyPool
{
    private readonly WeightedPool<Func<EventBus, Entity>> _pool = new();
    private readonly GoblinTribe _goblinTribe = new();

    public LibraryEnemyPool()
    {
        _pool.Add(() => (bus => new Goblin(_goblinTribe, bus)), 1);
    }

    public Entity GetEntity(EventBus eventBus)
    {
        var factoryMethod = _pool.Draw();
        return factoryMethod(eventBus);
    }
}