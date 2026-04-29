namespace maze_runner.Core.Engine;

public interface IInputPublisher
{
    void EnqueueInput(char key);
}