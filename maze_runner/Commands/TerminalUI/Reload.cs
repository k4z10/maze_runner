using maze_runner.Core.Engine;
using maze_runner.Core.Frontend;

namespace maze_runner.Commands.TerminalUI;
using Core;

public class Reload(TerminalFrontend mng) : ICommand
{
    public void Execute() => mng.Reload();
}