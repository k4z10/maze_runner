namespace maze_runner.Core.Logger;

public static class UniversalLogChannel
{
    private static Action<string>? _logAction;

    public static void ConnectLogger(ILogger logger)
    {
        if (_logAction == null)
            _logAction = logger.Log;
        else
            throw new Exception("Cannot change logger while assigned.");
    }

    public static void Publish(string message)
    {
        _logAction?.Invoke(message);
    }
}