using System.Collections.ObjectModel;

namespace maze_runner.Core.Logger;

public class MemoryLogger : ILogger
{
    public ObservableCollection<string> Messages { get; } = new();

    public void Log(string message)
    {
        Messages.Insert(0, $"[{DateTime.Now:hh:mm:ss}] {message}");
        
        if (Messages.Count > 100)
            Messages.RemoveAt(Messages.Count - 1);
    }
}