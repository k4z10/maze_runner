using maze_runner.Core.Engine;
using maze_runner.Core.Frontend;

namespace maze_runner.Commands.TerminalUI;
using Core;

public class ToggleItemInfo(TerminalFrontend mng) : ICommand
{
    public void Execute() => mng.ToggleItemInfo();
}