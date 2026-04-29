using maze_runner.Entities;

namespace maze_runner.Core;
using Commands.Core;
using Dungeon.Map;

public class LevelContext : ILevelContext
{
    public Map Map { get; set; }
    public InputHandler InputHandler { get; set; }
    public EntityManager EntityManager { get; set; }
    public EventBus EventBus { get; set; }
    public string Description { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;

    public LevelContext()
    {
        Map = new Map();
        InputHandler = new InputHandler();
        EventBus = new EventBus();
        EntityManager = new EntityManager(EventBus);
    }
}