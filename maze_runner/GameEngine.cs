using System.Collections.ObjectModel;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace maze_runner;
using Terminal.Gui;

public class GameEngine
{
    private readonly Player _player = new();
    private readonly Map _map;

    private Window _mainWindow      = new();
    private Label _mapLabel         = new();
    private Label _leftHandLabel    = new();
    private Label _rightHandLabel   = new();
    private TextView _itemsTextView = new();
    private Label _attributesLabel  = new();
    private Label _accountLabel     = new();
    
    private View _tileInfoOverlay       = new();
    private TextView _tileInfoTextView  = new();
    private const int TileInfoWidth     = 15;
    private const int TileInfoHeight     = 5;
    private int _tileOverlayToggle = 0;

    private View _howToPlayOverlay = new();
    private TextView _howToPlayTextView = new();
    private readonly string _howToPlayText;
    private int _howToPlayOverlayToggle = 1;

    public GameEngine(Map map)
    {
        _map = map;
        using var streamReader = new StreamReader("howToPlay.txt");
        _howToPlayText = streamReader.ReadToEnd();
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
            Width = _map.Cols + 2,
            Height = _map.Rows + 2,
            Title = " Map ",
            BorderStyle = LineStyle.Single
        };
        _mapLabel = new Label()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = _map.ToString(),
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
        _itemsTextView = new TextView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            WordWrap = true,
            ReadOnly = true,
            AllowsTab = false
        };
        itemsFrame.Add(_itemsTextView);
        
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
            case KeyCode.W: _player.Move(-1, 0, _map); break;
            case KeyCode.S: _player.Move(1, 0, _map); break;
            case KeyCode.A: _player.Move(0, -1, _map); break;
            case KeyCode.D: _player.Move(0, 1, _map); break;
            case KeyCode.E:
                var currentTilePick = _map.GetTile(_player.Position.X, _player.Position.Y);
                var item = currentTilePick.PopItem();
                if (item == null)
                    break;
                var pickUpAssistant = new PickUpItemVisitor(_player);
                item.Accept(pickUpAssistant);
                break;
            case KeyCode.Q:
                var currentTileDrop = _map.GetTile(_player.Position.X, _player.Position.Y);
                var itemRightHand = _player.Inventory.RightHand;
                var itemLeftHand = _player.Inventory.LeftHand;
                if (itemRightHand != null)
                {
                    var dropAssistant = new DropItemVisitor(_player, currentTileDrop);
                    itemRightHand.Accept(dropAssistant);
                    break;
                }
                if (itemLeftHand != null)
                {
                    var dropAssistant = new DropItemVisitor(_player, currentTileDrop);
                    itemLeftHand.Accept(dropAssistant);
                }
                break;
            case KeyCode.F:
                Item itemToEquip;
                try
                {
                    itemToEquip = _player.Inventory.Items.First();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                var equipAssistant = new EquipItemVisitor(_player);
                itemToEquip.Accept(equipAssistant);
                break;
            case KeyCode.I:
                if (_tileOverlayToggle == 0)
                    _tileOverlayToggle++;
                else
                    _tileOverlayToggle--;
                break;
            case (KeyCode)'?':
                if (_howToPlayOverlayToggle == 0)
                    _howToPlayOverlayToggle++;
                else 
                    _howToPlayOverlayToggle--;
                break;
            case KeyCode.Esc: Application.RequestStop(); break;
            default:
                return;
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
        var sb = new StringBuilder(_map.ToString());
        sb[_player.Position.X * (_map.Cols + 1) + _player.Position.Y] = '@'; 
        _mapLabel.Text = sb.ToString();
        if (_map.GetTile(_player.Position.X, _player.Position.Y).Items.Any())
        {
            
        }
    }

    private void InventoryDisplay()
    {
        _leftHandLabel.Text = _player.Inventory.LeftHand == null
            ? " "
            : $"{_player.Inventory.LeftHand.Name}({_player.Inventory.LeftHand.TileSymbol})";
        _rightHandLabel.Text = _player.Inventory.RightHand == null 
            ? " " 
            : $"{_player.Inventory.RightHand.Name}({_player.Inventory.RightHand.TileSymbol})";

        var sb = new StringBuilder();
        foreach (var item in _player.Inventory.Items)
            sb.AppendLine($"{item.Name}({item.TileSymbol})");
        _itemsTextView.Text = sb.ToString();
        
        _accountLabel.Text = $"Gold:  {_player.Inventory.Bundle.Gold}\n" +
                             $"Coins: {_player.Inventory.Bundle.Coins}";
        _attributesLabel.Text = $"Health:     {_player.Attributes.Health}\n" +
                                $"Stamina:    {_player.Attributes.Stamina}\n" +
                                $"Strength:   {_player.Attributes.Strength}\n" +
                                $"Resistance: {_player.Attributes.Resistance}\n" +
                                $"Luck:       {_player.Attributes.Luck}\n" +
                                $"Wisdom:     {_player.Attributes.Wisdom}";
    }

    private void TileInfoOverlay()
    {
        var currentTile = _map.GetTile(_player.Position.X, _player.Position.Y);
        if (currentTile.Items.Any() && _tileOverlayToggle == 1)
        {
            _tileInfoTextView.Text = string.Empty;
            foreach (var item in currentTile.Items)
                _tileInfoTextView.Text += $"{item.Name}({item.TileSymbol})\n";

            var terminalX = _player.Position.Y - TileInfoWidth / 2;
            var terminalY = _player.Position.X - TileInfoHeight;

            if (terminalY < 0) terminalY = _player.Position.X + 1;
            if (terminalX < 0) terminalX = 0;

            if (terminalX + TileInfoWidth > _map.Cols)
            {
                terminalX = _player.Position.Y - TileInfoWidth - 1;
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
            _howToPlayTextView.Text = _howToPlayText;
            _howToPlayOverlay.Visible = true;
        }
        else
        {
            _howToPlayOverlay.Visible = false;
        }
    }
}