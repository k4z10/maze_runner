namespace maze_runner.Core.Engine.Commands;
using Core;

public class Reload : ICommand
{
    public void Execute(TerminalUIManager manager) => manager.Reload();
}