namespace maze_runner.Core.Logger;

public class CompositeLogger(params ILogger[] loggers) : ILogger
{
    private readonly List<ILogger> _loggers = loggers.ToList();
    public void Log(string message)
    {
        foreach (var logger in _loggers)
            logger.Log(message);
    }
}