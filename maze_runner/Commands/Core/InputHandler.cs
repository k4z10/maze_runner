using System.Text;

namespace maze_runner.Commands.Core;
using maze_runner.Core;
using Terminal.Gui;

public class InputHandler
{
    private readonly Dictionary<Key, ICommand> _keyBindings = new();
    
    public void RegisterCommand(Key key, ICommand command) => _keyBindings[key] = command;

    public void ProcessInput(Key key, IGameContext ctx)
    {
        if (_keyBindings.TryGetValue(key, out var command))
            if (command.CanExecute(ctx))
                command.Execute(ctx);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in _keyBindings)
            sb.AppendLine($"[{kvp.Key.ToString()}] - {kvp.Value.Description}");
        return sb.ToString();
    }
}