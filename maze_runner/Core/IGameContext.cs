namespace maze_runner.Core;
using Dungeon.Map;

public interface IGameContext
{
    Entities.Player.Player Player { get; }
    Map Map { get; }
}