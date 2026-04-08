namespace maze_runner.Core.Engine.Commands.Core;

public interface ICommand
{
    void Execute(TerminalUIManager manager);
}