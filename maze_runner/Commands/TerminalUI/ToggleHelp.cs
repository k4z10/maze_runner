using maze_runner.Core.Frontend;

namespace maze_runner.Commands.TerminalUI;
using Core;

public class ToggleHelp(TerminalFrontend mng) : ICommand
{
    public void Execute() => mng.ToggleHelp();
}