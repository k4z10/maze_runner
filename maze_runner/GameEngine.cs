using System.Text;

namespace maze_runner;
using Terminal.Gui;

public class GameEngine
{
    private Player _player;
    private Map _map;
    
    private Window _mainWindow;
    private Label _mapLabel;

    public GameEngine(Map map)
    {
        _player = new Player();
        _map = map;
    }

    public void Run()
    {
        Application.Init();

        ColorScheme scheme = new ColorScheme()
        {
            Normal = new Attribute(Color.White, Color.Black),
            Focus = new Attribute(Color.White, Color.Black),
            HotNormal = new Attribute(Color.White, Color.Black),
            HotFocus = new Attribute(Color.White, Color.Black),
        };

        _mainWindow = new Window()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Title = "Maze Runner",
            ColorScheme = scheme,
        };

        _mapLabel = new Label()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = _map.ToString(),
        };
        _mainWindow.Add(_mapLabel);

        _mainWindow.KeyDown += HandleKeyboard!;
        _mainWindow.MouseClick += HandleMouse!;

        Render();
        Application.Run(_mainWindow);
        
        _mainWindow.Dispose();
        Application.Shutdown();
    }

    private void HandleKeyboard(object sender, Key e)
    {
        switch (e.KeyCode)
        {
            case KeyCode.W:
                _player.Move(-1, 0, _map);
                break;
            case KeyCode.S:
                _player.Move(1, 0, _map);
                break;
            case KeyCode.A:
                _player.Move(0, -1, _map);
                break;
            case KeyCode.D:
                _player.Move(0, 1, _map);
                break;
            case KeyCode.E:
                break;
            case KeyCode.Q:
                break;
            case KeyCode.Esc:
                Application.RequestStop();
                break;
            default:
                return;
        }

        e.Handled = true;
        Render();
    }

    private void HandleMouse(object sender, MouseEventArgs args)
    {
        if (args.IsSingleClicked)
        {
            int targetX = args.Position.X;
            int targetY = args.Position.Y;
        }
        else if (args.IsWheel)
        {
        }

        args.Handled = true;
    }

    private void Render()
    {
        MapDisplay();
        _mainWindow.SetNeedsDraw();
    }

    private void MapDisplay()
    {
        var sb = new StringBuilder(_map.ToString());
        sb[_player.Position.X * (_map.Cols + 1) + _player.Position.Y] = '@'; 
        // sb[_player.Position.X * (_map.SizeY + 1)] = '*';
        _mapLabel.Text = sb.ToString();
    }
}