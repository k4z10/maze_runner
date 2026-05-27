namespace maze_runner.Model.Core.Actions;

public class CommandRegistry
{
    public Dictionary<string, ICommand> Handlers { get; } = new();
    public Dictionary<string, string> Descriptions { get; } = new();

    public void RegisterCommand(string commandId, ICommand handler, string description)
    {
        Handlers[commandId] = handler;
        Descriptions[commandId] = description;
    }
}