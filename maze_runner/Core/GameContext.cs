namespace maze_runner.Core;
using Dungeon.Builders;
using Dungeon.Map;
using Player;

public interface IGameContext
{
    Player Player { get; }
    Map Map { get; }
    IModifierDungeonBuilder DungeonBuilder { get; }
}