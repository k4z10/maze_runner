using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities;

namespace maze_runner.Model.Dungeon;

public interface IEnemyPool
{
    Entity GetEntity(EventBus eventBus);
}