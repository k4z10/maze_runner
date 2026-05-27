using System.Text;
namespace maze_runner.View;

public class InputController
{
    private readonly Dictionary<char, (Action Action, string Description)> _localActions = new();
    private readonly Dictionary<char, string> _networkBindings = new();
    private readonly Dictionary<string, string> _availableNetworkCommands = new();
    private readonly Action<string> _sendNetworkCommand;

    public InputController(Action<string> sendNetworkCommand)
    {
        _sendNetworkCommand =  sendNetworkCommand;

        _networkBindings['w'] = "MOVE_U";
        _networkBindings['s'] = "MOVE_D";
        _networkBindings['a'] = "MOVE_L";
        _networkBindings['d'] = "MOVE_R";

        _networkBindings['e'] = "PICKUP";
        _networkBindings['q'] = "DROP";
        _networkBindings['f'] = "EQUIP";
        _networkBindings['F'] = "UNEQUIP";
    }
    
    public void RegisterLocalAction(char key, Action action, string desc = "") => _localActions[key] = (action, desc);

    public void UpdateAvailableNetworkCommand(Dictionary<string, string> cmd)
    {
        _availableNetworkCommands.Clear();
        foreach (var (id, desc) in cmd)
            _availableNetworkCommands[id] = desc;
    }

    public void ProcessInput(char key)
    {
        if (_localActions.TryGetValue(key, out var local))
        {
            local.Action();
            return;
        }

        if (!_networkBindings.TryGetValue(key, out var networkCommandId)) return;
        if (_availableNetworkCommands.ContainsKey(networkCommandId))
            _sendNetworkCommand(networkCommandId);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("User Interface:");
        foreach (var kvp in _localActions)
        {
            var key = kvp.Key;
            if (!char.IsLetterOrDigit(key)) key = ' ';
            sb.AppendLine($"    [{key}] - {kvp.Value.Description}");
        }
        sb.AppendLine("In-game actions:");
        foreach (var kvp in _networkBindings)
        {
            if (!_availableNetworkCommands.TryGetValue(kvp.Value, out var desc)) continue;
            
            var key = kvp.Key;
            if (!char.IsAscii(key)) key = ' ';
            sb.AppendLine($"    [{key}] - {desc}");
        }

        return sb.ToString();
    }
}