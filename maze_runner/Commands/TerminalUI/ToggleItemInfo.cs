using maze_runner.Core.Engine;

namespace maze_runner.Commands.TerminalUI;
using Core;

public class ToggleItemInfo(TerminalUIManager mng) : ICommand
{
    public void Execute() => mng.ToggleItemInfo();
}