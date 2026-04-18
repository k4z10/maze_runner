using maze_runner.Commands.Core;
using maze_runner.Core.Engine;

namespace maze_runner.Commands.TerminalUI;

public class ToggleJournal(TerminalUIManager mng) : ICommand
{
    public void Execute()
    {
        mng.ToggleJournal();
    }
}