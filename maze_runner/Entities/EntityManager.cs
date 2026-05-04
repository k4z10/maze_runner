using maze_runner.Core;
using maze_runner.Core.Logger;
using maze_runner.Dungeon.Map;

namespace maze_runner.Entities;

public class EntityManager(IEventPublisher eventPublisher)
{
    private readonly List<Entity> _entities = new();
    private readonly Dictionary<(int Row, int Col), HashSet<Entity>> _spatialGrid = new();
    private Player.Player? _player;

    public Player.Player Player => _player ?? throw new InvalidOperationException("Player not registered.");
    public IReadOnlyList<Entity> AllEntities => _entities;

    public void RegisterPlayer(Player.Player player)
    {
        _player = player;
        AddEntity(player);
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
        AddToGrid(entity);
    }

    public IEnumerable<Entity> GetEntitiesAt(int row, int col)
    {
        return _spatialGrid.TryGetValue((row, col), out var cell) 
            ? cell 
            : Enumerable.Empty<Entity>();
    }

    public Entity? GetAnyEntityExceptPlayerAt(int row, int col)
    {
        if (_spatialGrid.TryGetValue((row, col), out var cell))
        {
            foreach (var entity in cell)
            {
                if (entity != _player) return entity;
            }
        }
        return null;
    }

    public void MoveEntity(Entity entity, int newRow, int newCol)
    {
        RemoveFromGrid(entity);
        entity.Position = (newRow, newCol);
        AddToGrid(entity);
    }

    public void RemoveDeadEntities()
    {
        for (int i = _entities.Count - 1; i >= 0; i--)
        {
            var entity = _entities[i];
            
            if (!entity.IsAlive)
            {
                RemoveFromGrid(entity);
                _entities.RemoveAt(i);
                eventPublisher.Publish(new EnemyDefeatedEvent(entity.Name));
            }
        }
    }

    
    
    private void AddToGrid(Entity entity)
    {
        if (!_spatialGrid.TryGetValue(entity.Position, out var cell))
        {
            cell = new HashSet<Entity>();
            _spatialGrid[entity.Position] = cell;
        }
        cell.Add(entity);
    }

    private void RemoveFromGrid(Entity entity)
    {
        if (_spatialGrid.TryGetValue(entity.Position, out var cell))
        {
            cell.Remove(entity);
            if (cell.Count == 0)
            {
                _spatialGrid.Remove(entity.Position);
            }
        }
    }
}