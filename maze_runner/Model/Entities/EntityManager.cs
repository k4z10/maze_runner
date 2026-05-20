using maze_runner.Core;
using maze_runner.Model.Entities.Mobs;

namespace maze_runner.Model.Entities;

public class EntityManager(IEventPublisher eventPublisher)
{
    private readonly List<Entity> _mobs = new();
    private readonly Dictionary<(int Row, int Col), HashSet<Entity>> _spatialGrid = new();

    private readonly List<Player.Player> _players = new(10);
    public IReadOnlyList<Entity> Mobs => _mobs;
    public IReadOnlyList<Player.Player> Players => _players;

    public void RegisterPlayer(Player.Player player)
    {
        if (_players.Contains(player)) return;
        _players.Add(player);
    }

    public void RemovePlayer(Player.Player player)
    {
        _players.Remove(player);
    }
    
    public void AddEntity(Entity entity)
    {
        _mobs.Add(entity);
        AddToGrid(entity);
    }

    public IEnumerable<Entity> GetEntitiesAt(int row, int col)
    {
        return _spatialGrid.TryGetValue((row, col), out var cell) 
            ? cell 
            : Enumerable.Empty<Entity>();
    }

    public Entity? GetMobAt(int row, int col)
    {
        if (_spatialGrid.TryGetValue((row, col), out var cell))
        {
            return cell.FirstOrDefault();
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
        for (int i = _mobs.Count - 1; i >= 0; i--)
        {
            var entity = _mobs[i];

            if (entity.IsAlive) continue;
            
            RemoveFromGrid(entity);
            _mobs.RemoveAt(i);
            eventPublisher.Publish(new EnemyDefeatedEvent(entity.Name));
        }
    }

    
    
    private void AddToGrid(Entity entity)
    {
        if (!_spatialGrid.TryGetValue(entity.Position, out var cell))
        {
            cell = [];
            _spatialGrid[entity.Position] = cell;
        }
        cell.Add(entity);
    }

    private void RemoveFromGrid(Entity entity)
    {
        if (!_spatialGrid.TryGetValue(entity.Position, out var cell)) return;
        
        cell.Remove(entity);
        if (cell.Count == 0)
            _spatialGrid.Remove(entity.Position);
    }
}