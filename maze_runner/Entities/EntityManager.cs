using maze_runner.Core.Logger;
using maze_runner.Dungeon.Map;

namespace maze_runner.Entities;

public class EntityManager
{
    private readonly List<Entity> _entities = new();
    
    private readonly Dictionary<(int Row, int Col), HashSet<Entity>> _spatialGrid = new();
    private Player.Player? _player;

    public Player.Player Player => _player ?? throw new InvalidOperationException("Player should be first registered");
    public IReadOnlyList<Entity> AllEntities => _entities;

    public void RegisterPlayer(Player.Player player)
    {
        _player = player;
        AddEntity(player);
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);

        if (!_spatialGrid.TryGetValue(entity.Position, out var cellEntities))
        {
            cellEntities = new HashSet<Entity>();
            _spatialGrid[entity.Position] = cellEntities;
        }
        
        cellEntities.Add(entity);
    }

    public IEnumerable<Entity> GetEntitiesAt(int row, int col)
    {
        if (_spatialGrid.TryGetValue((row, col), out var cellEntities))
        {
            return cellEntities;
        }
        return Enumerable.Empty<Entity>();
    }

    public Entity? GetAnyEntityExceptPlayerAt(int row, int col)
    {
        if (_spatialGrid.TryGetValue((row, col), out var cellEntities))
        {
            return cellEntities.FirstOrDefault(e => e != Player);
        }
    
        return null; 
    }

    public void MoveEntity(Entity entity, int newRow, int newCol)
    {
        if (_spatialGrid.TryGetValue(entity.Position, out var oldCell))
        {
            oldCell.Remove(entity);
            
            if (oldCell.Count == 0) 
                _spatialGrid.Remove(entity.Position);
        }

        entity.Position = (newRow, newCol);

        if (!_spatialGrid.TryGetValue(entity.Position, out var newCell))
        {
            newCell = new HashSet<Entity>();
            _spatialGrid[entity.Position] = newCell;
        }
        newCell.Add(entity);
    }

    public void RemoveDeadEntities()
    {
        for (int i = _entities.Count - 1; i >= 0; i--)
        {
            var entity = _entities[i];
            if (!entity.IsAlive)
            {
                if (_spatialGrid.TryGetValue(entity.Position, out var cell))
                {
                    cell.Remove(entity);
                    if (cell.Count == 0) _spatialGrid.Remove(entity.Position);
                }
                _entities.RemoveAt(i);
                
                EventTopic<EnemyDefeatedEvent>.Publish(new EnemyDefeatedEvent(entity.Name));
            }
        }
    }
}