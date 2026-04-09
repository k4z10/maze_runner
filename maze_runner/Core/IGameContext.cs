using maze_runner.Entities;
using maze_runner.Entities.Player;

namespace maze_runner.Core;
using Dungeon.Map;

public interface IGameContext
{
    public ILevelContext CurrentLevel { get; }
    public GameConfig Config { get; }
}