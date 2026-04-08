namespace maze_runner.Commands.Core;
using maze_runner.Core;

public interface ICommand
{
    bool CanExecute(IGameContext context);
    void Execute(IGameContext context);
}