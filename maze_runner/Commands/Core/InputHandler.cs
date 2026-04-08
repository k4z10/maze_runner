using System.Text;

namespace maze_runner.Commands.Core;
using maze_runner.Core;
using Terminal.Gui;

public class InputHandler
{
    private readonly Dictionary<Key, (ICommand command, string description)> _keyBindings = new();
    
    public void RegisterCommand(Key key, ICommand command, string description = "") 
        => _keyBindings[key] = (command, description);

    public void ProcessInput(Key key, IGameContext ctx)
    {
        if (_keyBindings.TryGetValue(key, out var b))
            if (b.command.CanExecute(ctx))
                b.command.Execute(ctx);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in _keyBindings)
            sb.AppendLine($"[{kvp.Key.ToString()}] - {kvp.Value.description}");
        return sb.ToString();
    }
}