using maze_runner.Core.Engine;

namespace maze_runner.Commands.TerminalUI;
using Core;

public class ToggleHelp(TerminalUIManager mng) : ICommand
{
    public void Execute() => mng.ToggleHelp();
}