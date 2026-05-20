using System.Text;
namespace maze_runner.View;

public class InputHandler
{
    private readonly Dictionary<char, (Action command, string description)> _keyBindings = new();
    
    public void RegisterCommand(char key, Action command, string description = "") 
        => _keyBindings[key] = (command, description);

    public bool ProcessInput(char key)
    {
        if (!_keyBindings.TryGetValue(key, out var b)) return false;
        b.command.Invoke();
        return true;

    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var kvp in _keyBindings)
            sb.AppendLine($"[{kvp.Key}] - {kvp.Value.description}");
        return sb.ToString();
    }
}