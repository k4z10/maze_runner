namespace maze_runner.Core.Logger;

public class CompositeLogger(params IMessageLog[] loggers) : IMessageLog
{
    private readonly List<IMessageLog> _loggers = loggers.ToList();

    public void Log(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Log(message);
        }
    }
}