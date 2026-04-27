namespace maze_runner.Core.Logger;

public class FileLogger : ILogger, IDisposable
{
    private readonly StreamWriter _writer;
    
    public FileLogger(GameConfig config)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
        var fileName = $"{config.PlayerName}_{timestamp}.log";
        var filePath = Path.Combine(config.LogDirectoryPath, fileName);
        
        Directory.CreateDirectory(config.LogDirectoryPath);

        var fileStream = new FileStream(
            filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.Read
        );
        _writer = new StreamWriter(fileStream) {AutoFlush = true};
    }

    public void Log(string message)
    {
        _writer.WriteLine($"[{DateTime.Now:hh:mm:ss}] {message}");
    }

    public void Dispose()
    {
         _writer.Dispose();
    }
}