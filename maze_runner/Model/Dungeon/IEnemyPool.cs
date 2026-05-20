using maze_runner.Core;
using maze_runner.Model.Entities;

namespace maze_runner.Model.Dungeon;

public interface IEnemyPool
{
    Entity GetEntity(IEventPublisher eventPublisher, IEventSubscriber eventSubscriber);
}