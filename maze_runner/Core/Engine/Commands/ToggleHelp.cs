namespace maze_runner.Core.Engine.Commands;
using Core;

public class ToggleHelp : ICommand
{
    public void Execute(TerminalUIManager manager) => manager.ToggleHelp();
}