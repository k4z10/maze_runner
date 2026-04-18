using maze_runner.Entities;

namespace maze_runner.Core;
using Commands.Core;
using Dungeon.Map;

public class LevelContext(Map map, InputHandler inputHandler, EntityManager entityManager, string description = "", string name = "Level") : ILevelContext
{
    public Map Map { get; set; } = map;
    public InputHandler InputHandler { get; set; } = inputHandler;
    public EntityManager EntityManager { get; set; } = entityManager;
    public string Description { get; set; } =  description;
    public string LevelName { get; set; } = name;
}