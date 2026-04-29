namespace maze_runner.Core.Frontend;

public interface IOverlay
{
    bool ProcessInput(char key);
    void Render();
    bool IsFinished { get; }
}