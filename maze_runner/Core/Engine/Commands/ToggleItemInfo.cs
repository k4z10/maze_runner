namespace maze_runner.Core.Engine.Commands;
using Core;

public class ToggleItemInfo : ICommand
{
    public void Execute(TerminalUIManager manager) => manager.ToggleItemInfo();
}