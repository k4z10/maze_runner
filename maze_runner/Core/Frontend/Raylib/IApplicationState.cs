namespace maze_runner.Core.Frontend.Raylib;

public interface IApplicationState
{
    public bool IsRunning { get; }
    void RequestQuit();
}