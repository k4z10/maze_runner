namespace maze_runner.Core;
using Entities;
using Commands.Core;
using Dungeon.Map;

public interface ILevelContext
{
    public Map Map { get; set; }
    public InputHandler InputHandler { get; set; }
    public EntityManager EntityManager { get; set; }
    public string Description { get; }
    public string LevelName { get; }
}