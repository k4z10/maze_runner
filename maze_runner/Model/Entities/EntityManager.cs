using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities.Mobs;

namespace maze_runner.Model.Entities;

public class EntityManager(IEventPublisher eventPublisher)
{
    private readonly HashSet<Entity> _entities = new();
    public IEnumerable<Entity> Entities => _entities;

    public void RegisterEntity(Entity entity) => _entities.Add(entity);
    public void RemoveEntity(Entity entity) => _entities.Remove(entity);
    public Entity? GetEntityAt(int row, int col) => _entities.FirstOrDefault(e => e.Position.Row == row && e.Position.Col == col); 

    public void RemoveDeadEntities()
    {
        foreach (var entity in _entities.Where(entity => !entity.IsAlive))
        {
            _entities.Remove(entity);
            entity.Dispose();
            eventPublisher.Publish(new EntityDefeatedEvent(entity.Name));
        }
    }
}