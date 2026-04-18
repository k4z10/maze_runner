using maze_runner.Core.Engine;

namespace maze_runner.Commands.TerminalUI;
using Core;

public class Reload(TerminalUIManager mng) : ICommand
{
    public void Execute() => mng.Reload();
}