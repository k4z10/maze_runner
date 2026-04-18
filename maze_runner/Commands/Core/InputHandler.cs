using System.Text;
using maze_runner.Core.Logger;

namespace maze_runner.Commands.Core;
using maze_runner.Core;
using Terminal.Gui;

public class InputHandler
{
    private readonly Dictionary<Key, (ICommand command, string description)> _keyBindings = new();
    
    public void RegisterCommand(Key key, ICommand command, string description = "") 
        => _keyBindings[key] = (command, description);

    public bool ProcessInput(Key key)
    {
        if (_keyBindings.TryGetValue(key, out var b))
        {
            b.command.Execute();
            return true;
        }
        
        return false;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in _keyBindings)
            sb.AppendLine($"[{kvp.Key.ToString()}] - {kvp.Value.description}");
        return sb.ToString();
    }
}