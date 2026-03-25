using maze_runner.Commands.Core;
using maze_runner.Dungeon.Builders;
using maze_runner.Dungeon.Strategies;
using maze_runner.Items.Models;

namespace maze_runner.Core;
using System.Text;
using Terminal.Gui;
using Player;
using Dungeon.Map;
using Items.Visitors;

public class GameEngine : IGameContext
{
    public Player Player { get; }
    public Map Map { get; private set; }
    private LevelContext _currentLevelContext;

    private Window _mainWindow      = new();
    private Label _mapLabel         = new();
    private Label _leftHandLabel    = new();
    private Label _rightHandLabel   = new();
    private Label _attributesLabel  = new();
    private Label _accountLabel     = new();
    private ListView _itemsListView = new();
    
    private View _tileInfoOverlay       = new();
    private TextView _tileInfoTextView  = new();
    private const int TileInfoWidth     = 15;
    private const int TileInfoHeight     = 5;
    private int _tileOverlayToggle = 0;

    private View _howToPlayOverlay = new();
    private TextView _howToPlayTextView = new();
    private int _howToPlayOverlayToggle = 1;

    public void LoadLevel(IDungeonGenerationStrategy strategy, int width = 40, int height = 20)
    {
        var ctx = strategy.Generate(width, height);
        _currentLevelContext = ctx;
        Map = ctx.Map;
        Player.Position = (0, 0);
    }

    public GameEngine(Player player)
    {
        var ctx = new InitialDungeonStrategy().Generate(40, 20);
        _currentLevelContext = ctx;
        Map = ctx.Map;
        Player = player;
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
        
        
        BuildUI();
        
        _mainWindow.KeyDown += HandleKeyboard!;
        _mainWindow.MouseClick += HandleMouse!;

        HowToPlayOverlay();
        Render();
        Application.Run(_mainWindow);
        
        _mainWindow.Dispose();
        Application.Shutdown();
    }

    private void BuildUI()
    {
        _howToPlayOverlay = new View()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Title = " How to play? Miniguide.",
            BorderStyle = LineStyle.Single,
            Visible = false,
        };
        _howToPlayTextView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            WordWrap = true,
            ReadOnly = true,
        };
        _howToPlayOverlay.Add(_howToPlayTextView);
        
        var mapFrame = new View()
        {
            X = 0,
            Y = 0,
            Width = Map.Cols + 2,
            Height = Map.Rows + 2,
            Title = " Map ",
            BorderStyle = LineStyle.Single
        };
        _mapLabel = new Label()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = Map.ToString(),
        };
        // info overlay na temat zawartości kafelka
        _tileInfoOverlay = new View()
        {
            X = 0,
            Y = 0,
            Width = TileInfoWidth,
            Height = TileInfoHeight,
            Title = " Contents ",
            BorderStyle = LineStyle.RoundedDotted,
            Visible = false
        };
        _tileInfoTextView = new TextView()
        {
            X = 0,
            Y = 0,
            Height = Dim.Fill(),
            Width = Dim.Fill(),
            WordWrap = true,
            ReadOnly = true,
        };
        _tileInfoOverlay.Add(_tileInfoTextView);
        
        mapFrame.Add(_mapLabel, _tileInfoOverlay);



        var inventoryFrame = new View()
        {
            X = Pos.Right(mapFrame) + 1,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Height(mapFrame),
            BorderStyle = LineStyle.Single
        };

        // Hands Frame
        var handsFrame = new View()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 5,
            Title = " Hands ",
            BorderStyle = LineStyle.Rounded,
        };
        var leftHandFrame = new View()
            { X = 0, Y = 0, Width = Dim.Percent(50), Height = 3, BorderStyle = LineStyle.Single };
        var rightHandFrame = new View()
        {
            X = Pos.Right(leftHandFrame), Y = 0, Width = Dim.Percent(50), Height = 3, BorderStyle = LineStyle.Single
        };
        _leftHandLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill() };
        leftHandFrame.Add(_leftHandLabel);
        _rightHandLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill() };
        rightHandFrame.Add(_rightHandLabel);
        handsFrame.Add(leftHandFrame, rightHandFrame);

        // Inventory.Items Frame
        var itemsFrame = new View()
        {
            X = 0,
            Y = Pos.Bottom(handsFrame),
            Width = Dim.Percent(50),
            Height = 8,
            Title = " Inventory ",
            BorderStyle = LineStyle.Rounded
        };
        _itemsListView = new ListView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            AllowsMarking = false,
        };
        _itemsListView.SetSource(Player.Inventory.Items);
        _itemsListView.SelectedItemChanged += (sender, args) =>
        {
            Player.Inventory.CurrentIndex = args.Item;
        };
        _itemsListView.RowRender += (sender, args) =>
        {
            args.RowAttribute = args.Row == _itemsListView.SelectedItem ?
                                new Attribute(Color.Black, Color.White) :
                                new Attribute(Color.White, Color.Black);
        };
        itemsFrame.Add(_itemsListView);
        
        // Attributes Frame
        var attributesFrame = new View()
        {
            X = Pos.Right(itemsFrame),
            Y = Pos.Bottom(handsFrame),
            Width = Dim.Percent(50),
            Height = Dim.Height(itemsFrame),
            Title = " Attributes ",
            BorderStyle = LineStyle.Rounded
        };
        _attributesLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill() };
        attributesFrame.Add(_attributesLabel);
        
        // Money Frame
        var accountFrame = new View()
        {
            X = 0,
            Y = Pos.Bottom(itemsFrame),
            Width = Dim.Fill(),
            Height = 4,
            Title = " Bundle ",
            BorderStyle = LineStyle.Rounded
        };
        _accountLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill(), }; 
        accountFrame.Add(_accountLabel);
        
        
        
        inventoryFrame.Add(handsFrame, itemsFrame, attributesFrame, accountFrame);
        
        _mainWindow.Add(mapFrame, inventoryFrame, _howToPlayOverlay); // _howToPlayOverlay always at the end.
    }

    private void HandleKeyboard(object sender, Key e)
    {
        switch (e.KeyCode)
        {
            case KeyCode.Tab:
                LoadLevel(new EasyDungeonStrategy());
                break;
            case KeyCode.I:
                _tileOverlayToggle ^= 1;
                break;
            case (KeyCode)'?':
                _howToPlayOverlayToggle ^= 1;
                break;
            case KeyCode.Esc: Application.RequestStop(); break;
            default:
                _currentLevelContext.InputHandler.ProcessInput((KeyCode)e, this);
                break;
        }
        
        HowToPlayOverlay();
        TileInfoOverlay();
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
        else if ((args.Flags & MouseFlags.WheeledUp) != 0)
        {
        }

        args.Handled = true;
    }

    private void Render()
    {
        MapDisplay();
        InventoryDisplay();
        _mainWindow.SetNeedsDraw();
    }

    private void MapDisplay()
    {
        var sb = new StringBuilder(Map.ToString());
        sb[Player.Position.Row * (Map.Cols + 1) + Player.Position.Col] = '@'; 
        _mapLabel.Text = sb.ToString();
        
        if (Map.GetTile(Player.Position.Row, Player.Position.Col).Items.Any())
        {
            
        }
    }

    private void InventoryDisplay()
    {
        _leftHandLabel.Text = Player.Inventory.LeftHand == null
            ? " "
            : $"{Player.Inventory.LeftHand.Name}({Player.Inventory.LeftHand.TileSymbol})";
        _rightHandLabel.Text = Player.Inventory.RightHand == null 
            ? " " 
            : $"{Player.Inventory.RightHand.Name}({Player.Inventory.RightHand.TileSymbol})";

        var sb = new StringBuilder();
        foreach (var item in Player.Inventory.Items)
            sb.AppendLine($"{item.Name}({item.TileSymbol})");
        
        _accountLabel.Text = $"Gold:  {Player.Inventory.Bundle.Gold}\n" +
                             $"Coins: {Player.Inventory.Bundle.Coins}";
        _attributesLabel.Text = $"Health:     {Player.Attributes.Health}\n" +
                                $"Stamina:    {Player.Attributes.Stamina}\n" +
                                $"Strength:   {Player.Attributes.Strength}\n" +
                                $"Resistance: {Player.Attributes.Resistance}\n" +
                                $"Luck:       {Player.Attributes.Luck}\n" +
                                $"Wisdom:     {Player.Attributes.Wisdom}";
    }

    private void TileInfoOverlay()
    {
        var currentTile = Map.GetTile(Player.Position.Row, Player.Position.Col);
        if (currentTile.Items.Any() && _tileOverlayToggle == 1)
        {
            _tileInfoTextView.Text = string.Empty;
            foreach (var item in currentTile.Items)
                _tileInfoTextView.Text += $"{item.Name}({item.TileSymbol})\n";

            var terminalX = Player.Position.Col - TileInfoWidth / 2;
            var terminalY = Player.Position.Row - TileInfoHeight;

            if (terminalY < 0) terminalY = Player.Position.Row + 1;
            if (terminalX < 0) terminalX = 0;

            if (terminalX + TileInfoWidth > Map.Cols)
            {
                terminalX = Player.Position.Col - TileInfoWidth - 1;
                if (terminalX < 0) terminalX = 0;
            }
            
            _tileInfoOverlay.X = terminalX;
            _tileInfoOverlay.Y = terminalY;

            if (!_tileInfoOverlay.Visible)
                _tileInfoOverlay.Visible = true;
            _tileInfoOverlay.SetNeedsDraw();
        }
        else
        {
            if (_tileInfoOverlay.Visible)
            {
                _tileInfoOverlay.Visible = false;
                // _mapLabel.SetNeedsDraw();
            }
        }
    }

    private void HowToPlayOverlay()
    {
        if (_howToPlayOverlayToggle == 1)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_currentLevelContext.LevelName);
            sb.AppendLine(_currentLevelContext.Description);
            sb.AppendLine();
            sb.AppendLine(_currentLevelContext.InputHandler.ToString());
            sb.Append("""
                      [?] - toggle help
                      [i] - toggle tile info
                      [tab] - generate new level
                      [esc] - quit game
                      """);
            
            _howToPlayTextView.Text = sb.ToString();
            _howToPlayOverlay.Visible = true;
        }
        else
        {
            _howToPlayOverlay.Visible = false;
        }
    }
}