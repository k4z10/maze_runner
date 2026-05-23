namespace maze_runner.Model.Core.Actions;

public class CommandRegistry
{
    public Dictionary<string, ICommand> Handlers { get; } = new();
    public Dictionary<char, string> KeyBindings { get; } = new();
    public Dictionary<string, string> Descriptions { get; } = new();

    public void RegisterCommand(char key, string commandId, ICommand handler, string description)
    {
        Handlers[commandId] = handler;
        KeyBindings[key] = commandId;
        Descriptions[commandId] = description;
    }
}