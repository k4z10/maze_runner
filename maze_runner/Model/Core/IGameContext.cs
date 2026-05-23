using maze_runner.Model.Core.Events;

namespace maze_runner.Model.Core;

public interface IGameContext
{
    public ILevelContext CurrentLevel { get; }
    public GameConfig Config { get; }
}