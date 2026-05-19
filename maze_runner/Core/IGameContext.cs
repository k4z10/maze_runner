using maze_runner.Core.Engine;
using maze_runner.Core.Frontend.Raylib;

namespace maze_runner.Core;

public interface IGameContext : IApplicationState, IInputPublisher
{
    public ILevelContext CurrentLevel { get; }
    public GameConfig Config { get; }
}