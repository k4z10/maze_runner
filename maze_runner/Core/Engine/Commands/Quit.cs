namespace maze_runner.Core.Engine.Commands;
using Core;

public class Quit : ICommand
{
    public void Execute(TerminalUIManager manager) => manager.Quit();
}