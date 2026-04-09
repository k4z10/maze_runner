using System.Collections.ObjectModel;

namespace maze_runner.Core.Logger;

public class MemoryLog : IMessageLog
{
    public ObservableCollection<string> Messages { get; } = new();

    public void Log(string message)
    {
        Messages.Add($"[{DateTime.Now:hh:mm:ss}] {message}");
    }
}