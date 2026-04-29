using maze_runner.Commands.Core;
using maze_runner.Core.Engine;
using maze_runner.Core.Frontend;

namespace maze_runner.Commands.TerminalUI;

public class ToggleJournal(TerminalFrontend mng) : ICommand
{
    public void Execute()
    {
        mng.ToggleJournal();
    }
}