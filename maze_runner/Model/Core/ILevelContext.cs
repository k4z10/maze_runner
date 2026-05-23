using maze_runner.Model.Core.Actions;
using maze_runner.Model.Core.Events;
using maze_runner.Model.Dungeon.Map;
using maze_runner.Model.Entities;

namespace maze_runner.Model.Core;

public interface ILevelContext
{
    public Map Map { get; set; }
    public CommandRegistry CommandRegistry { get; set; }
    public EntityManager EntityManager { get; set; }
    public EventBus EventBus { get; set; }
    public string Description { get; set; }
    public string LevelName { get; set; }
}