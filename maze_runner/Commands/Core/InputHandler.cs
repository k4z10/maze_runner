namespace maze_runner.Commands.Core;
using maze_runner.Core;
using Terminal.Gui;

public class InputHandler
{
    private readonly Dictionary<KeyCode, ICommand> _keyBindings = new();
    
    public void RegisterCommand(KeyCode key, ICommand command) => _keyBindings[key] = command;

    public void ProcessInput(KeyCode key, IGameContext ctx)
    {
        if (_keyBindings.TryGetValue(key, out var command))
            if (command.CanExecute(ctx))
                command.Execute(ctx);
    }
}