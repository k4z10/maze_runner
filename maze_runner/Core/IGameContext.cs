namespace maze_runner.Core;

public interface IGameContext
{
    public ILevelContext CurrentLevel { get; }
    public GameConfig Config { get; }
}