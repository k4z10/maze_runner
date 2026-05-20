using System.Collections.ObjectModel;
using Terminal.Gui;

namespace maze_runner.View;

public class ObservableFile : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly string _filePath;
    private long _lastPosition = 0;

    public ObservableCollection<string> Lines { get; } = [];

    public ObservableFile(string filePath)
    {
        _filePath = filePath;

        ReadNewLines();

        var directory = Path.GetDirectoryName(filePath);
        var fileName = Path.GetFileName(filePath);

        _watcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnFileChanged;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // Terminal.Gui.Application.Invoke wymusza wykonanie w głównym wątku UI!
        Application.Invoke(() => 
        {
            ReadNewLines();
        });
    }

    private void ReadNewLines()
    {
        if (!File.Exists(_filePath)) return;

        try
        {
            using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (stream.Length < _lastPosition)
            {
                Lines.Clear();
                _lastPosition = 0;
            }
            stream.Position = _lastPosition;

            using var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                Lines.Add(line);
            }

            _lastPosition = stream.Position;
        }
        catch (IOException)
        {
        }
    }

    public void Dispose()
    {
        if (_watcher != null)
        {
            _watcher.Changed -= OnFileChanged;
            _watcher.Dispose();
        }
    }
}