namespace maze_runner.Core.Logger;

public class FileLog : IMessageLog
{
    private string _filePath;
    
    public FileLog(GameConfig config)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
        var fileName = $"{config.PlayerName}_{timestamp}.log";
        _filePath = Path.Combine(config.LogDirectoryPath, fileName);
        
        Directory.CreateDirectory(config.LogDirectoryPath);
        File.Create(_filePath).Close();
    }

    public void Log(string message)
    {
        File.AppendAllText(_filePath, $"[{DateTime.Now:hh:mm:ss}] {message}{Environment.NewLine}");
    }
}