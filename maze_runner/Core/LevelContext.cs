using maze_runner.Model.Dungeon.Map;
using maze_runner.Model.Entities;

namespace maze_runner.Core;

public class LevelContext : ILevelContext
{
    public Map Map { get; set; }
    public CommandRegistry CommandRegistry { get; set; }
    public EntityManager EntityManager { get; set; }
    public EventBus EventBus { get; set; }
    public string Description { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;

    public LevelContext()
    {
        Map = new Map();
        CommandRegistry = new CommandRegistry();
        EventBus = new EventBus();
        EntityManager = new EntityManager(EventBus);
    }
}