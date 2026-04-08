using System.Text;
using Terminal.Gui;
namespace maze_runner.Core.Engine.Commands.Core;

public class InputHandler
{
    private readonly Dictionary<Key, (ICommand command, string description)> _keyBindings = new();
    
    public void RegisterCommand(Key key, ICommand command, string description = "") 
        => _keyBindings[key] = (command, description);

    public void ProcessInput(Key key, TerminalUIManager manager)
    {
        if (_keyBindings.TryGetValue(key, out var b))
            b.command.Execute(manager);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in _keyBindings)
            sb.AppendLine($"[{kvp.Key.ToString()}] - {kvp.Value.description}");
        return sb.ToString();
    }
}