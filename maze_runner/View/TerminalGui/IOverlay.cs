namespace maze_runner.View.TerminalGui;

public interface IOverlay
{
    bool ProcessInput(char key);
    void Render();
    bool IsFinished { get; }
}