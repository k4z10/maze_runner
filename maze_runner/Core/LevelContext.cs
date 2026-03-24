namespace maze_runner.Core;
using Commands.Core;
using Dungeon.Map;

public class LevelContext(Map map, InputHandler inputHandler)
{
    public Map Map { get; } = map;
    public InputHandler InputHandler { get; } = inputHandler;
}