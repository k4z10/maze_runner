using maze_runner.Entities;

namespace maze_runner.Core;
using Dungeon.Map;

public interface IGameContext
{
    EntityManager EntityManager { get; }
    Map CurrentMap { get; }
}