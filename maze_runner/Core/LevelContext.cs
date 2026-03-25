namespace maze_runner.Core;
using Commands.Core;
using Dungeon.Map;

public class LevelContext(Map map, InputHandler inputHandler, string description, string name = "Level")
{
    public Map Map { get; } = map;
    public InputHandler InputHandler { get; } = inputHandler;
    public string Description { get; set; } =  description;
    public string LevelName { get; set; } = name;
}