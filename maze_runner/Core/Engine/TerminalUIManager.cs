using System.ComponentModel.Design;
using maze_runner.Core.Engine.Commands;
using maze_runner.Dungeon.Strategies;
using maze_runner.Items.Models;

namespace maze_runner.Core.Engine;
using Commands.Core;
using Terminal.Gui;
using System.Text;

public class TerminalUIManager : IGameUIManager
{
    private readonly GameEngine _engine;
    private readonly InputHandler _engineInputHandler;
    private Window _mainWindow      = new();
    private Label _mapLabel         = new();
    private Label _leftHandLabel    = new();
    private Label _rightHandLabel   = new();
    private Label _attributesLabel  = new();
    private Label _accountLabel     = new();
    private ListView _itemsListView = new();
    
    private View _tileInfoOverlay       = new();
    private TextView _tileInfoTextView  = new();
    private TextView _tooltipTextView   = new ();
    private View _itemTooltipOverlay    = new();

    private View _howToPlayOverlay = new();
    private TextView _howToPlayTextView = new();
    
    private const int TileInfoWidth     = 15;
    private const int TileInfoHeight    = 5;
    
    private int _itemInfoToggle = 0;
    private int _howToPlayOverlayToggle = 1;

    public TerminalUIManager(GameEngine engine, InputHandler inputHandler)
    {
        _engine = engine;
        _engineInputHandler = inputHandler;
    }


    public void InitializeAndRun()
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
        
        _engineInputHandler.RegisterCommand('?', new ToggleHelp(), "Toggle help menu");
        _engineInputHandler.RegisterCommand(Key.Esc, new Quit(), "Quit game");
        _engineInputHandler.RegisterCommand('i', new ToggleItemInfo(), "Toggle in-game info overlay");
        _engineInputHandler.RegisterCommand(Key.Tab, new Reload(), "Reload game");
        
        
        BuildUI();
        
        _mainWindow.KeyDown += HandleKeyboard!;
        _mainWindow.MouseClick += HandleMouse!;

        HowToPlayOverlay();
        Render();
        Application.Run(_mainWindow);
        
        _mainWindow.Dispose();
        Application.Shutdown();        
    }

    public void Render()
    {
        MapDisplay();
        InventoryDisplay();
        _mainWindow.SetNeedsDraw();
    }
    
    public void ToggleItemInfo() => _itemInfoToggle ^= 1;
    public void ToggleHelp() => _howToPlayOverlayToggle ^= 1;
    public void Quit() => Application.RequestStop();
    public void Reload() => _engine.LoadLevel(new EasyDungeonStrategy());

    private void HandleKeyboard(object sender, Key e)
    {
        _engineInputHandler.ProcessInput(e.KeyCode, this);
        _engine.CurrentLevelContext.InputHandler.ProcessInput(e.KeyCode, _engine);
        
        HowToPlayOverlay();
        TileInfoOverlay();
        ItemInfoOverlay();
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
    
    private void MapDisplay() => _mapLabel.Text = _engine.CurrentMap.ToString();        
    private void InventoryDisplay()
    {
        _leftHandLabel.Text = _engine.Player.Inventory.LeftHand == null
            ? " "
            : $"{_engine.Player.Inventory.LeftHand.Name}({_engine.Player.Inventory.LeftHand.TileSymbol})";
        _rightHandLabel.Text = _engine.Player.Inventory.RightHand == null 
            ? " " 
            : $"{_engine.Player.Inventory.RightHand.Name}({_engine.Player.Inventory.RightHand.TileSymbol})";

        var sb = new StringBuilder();
        foreach (var item in _engine.Player.Inventory.Items)
            sb.AppendLine($"{item.Name}({item.TileSymbol})");
        
        _accountLabel.Text = $"Gold:  {_engine.Player.Inventory.Gold}\n" +
                             $"Coins: {_engine.Player.Inventory.Coins}";
        _attributesLabel.Text = $"Health:     {_engine.Player.Health}/{_engine.Player.MaxHealth}\n" +
                                $"Dexterity:  {_engine.Player.CurrentStats.Dexterity}\n" +
                                $"Stamina:    {_engine.Player.CurrentStats.Stamina}\n" +
                                $"Strength:   {_engine.Player.CurrentStats.Strength}\n" +
                                $"Resistance: {_engine.Player.CurrentStats.Resistance}\n" +
                                $"Luck:       {_engine.Player.CurrentStats.Luck}\n" +
                                $"Wisdom:     {_engine.Player.CurrentStats.Wisdom}";
    }
    
    private void TileInfoOverlay()
    {
        var currentTile = _engine.CurrentMap.GetTile(_engine.Player.Position.Row, _engine.Player.Position.Col);
        if (currentTile.Items.Any() && _itemInfoToggle == 1)
        {
            _tileInfoTextView.Text = string.Empty;
            foreach (var item in currentTile.Items)
                _tileInfoTextView.Text += $"{item.Name}({item.TileSymbol})\n";

            var terminalX = _engine.Player.Position.Col - TileInfoWidth / 2;
            var terminalY = _engine.Player.Position.Row - TileInfoHeight;

            if (terminalY < 0) terminalY = _engine.Player.Position.Row + 1;
            if (terminalX < 0) terminalX = 0;

            if (terminalX + TileInfoWidth > _engine.CurrentMap.Cols)
            {
                terminalX = _engine.Player.Position.Col - TileInfoWidth - 1;
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
    
    private void ItemInfoOverlay() => _itemTooltipOverlay.Visible = _itemInfoToggle != 0;
    private void ItemInfoWrite(Item item)
    {
        var selectedItem = item;
        string text = $"""
                       ({item.TileSymbol}) {item.Name}
                       {item.Description}
                       """;
        var weaponFeature = item.GetWeaponFeature();
        if (weaponFeature != null)
        {
            text += $"""

                     Damage: {weaponFeature.Damage}
                     Weight: {(weaponFeature.RequiredHands == 1 ? "Light" :"Heavy")}
                     """;
        }
        
        _tooltipTextView.Text = text;
    }

    private void HowToPlayOverlay()
    {
        if (_howToPlayOverlayToggle == 1)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("Level name: " + _engine.CurrentLevelContext.LevelName);
            sb.AppendLine(_engine.CurrentLevelContext.Description);
            sb.AppendLine();
            sb.AppendLine("1. General\n" + _engineInputHandler.ToString());
            sb.AppendLine();
            sb.AppendLine("2. Ingame actions\n" + _engine.CurrentLevelContext.InputHandler.ToString());
            
            _howToPlayTextView.Text = sb.ToString();
            _howToPlayOverlay.Visible = true;
        }
        else
        {
            _howToPlayOverlay.Visible = false;
        }
    }

    void BuildUI()
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
            Width = _engine.CurrentMap.Cols + 2,
            Height = _engine.CurrentMap.Rows + 2,
            Title = " Map ",
            BorderStyle = LineStyle.Single
        };
        _mapLabel = new Label()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = _engine.CurrentMap.ToString(),
        };
        // info overlay na temat zawartości kafelka
        _tileInfoOverlay = new View()
        {
            X = 0,
            Y = 0,
            Width = TileInfoWidth,
            Height = TileInfoHeight,
            Title = " Contents ",
            BorderStyle = LineStyle.Rounded,
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
        var attributesFrameWidth = 21;
        var itemsFrame = new View()
        {
            X = 0,
            Y = Pos.Bottom(handsFrame),
            Width = Dim.Fill() - attributesFrameWidth,
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
        _itemTooltipOverlay = new View()
        {
            X = 20,
            Y = -1,
            Width = Dim.Fill(),
            Height = Dim.Height(itemsFrame),
            BorderStyle = LineStyle.Rounded,
            Visible = false,
        };
        _tooltipTextView = new TextView() { Width = Dim.Fill(), Height = Dim.Fill(), WordWrap = true, ReadOnly = true };
        _itemTooltipOverlay.Add(_tooltipTextView);
        
        _itemsListView.SetSource(_engine.Player.Inventory.Items);
        _itemsListView.CollectionChanged += (_, args) =>
        {
            var items = _engine.Player.Inventory.Items;
            if (items.Count <= 0)
            {
                _tooltipTextView.Text = string.Empty;
                _itemTooltipOverlay.SetNeedsDraw();
                return;
            }

            if (args.OldStartingIndex == _engine.Player.Inventory.CurrentIndex)
            {
                ItemInfoWrite(items[_engine.Player.Inventory.CurrentIndex]);
            }
            if (args.NewStartingIndex == _engine.Player.Inventory.CurrentIndex)
                ItemInfoWrite(items[_engine.Player.Inventory.CurrentIndex]);
        };
        _itemsListView.SelectedItemChanged += (_, args) =>
        {
            _engine.Player.Inventory.CurrentIndex = args.Item;
            ItemInfoWrite((Item)args.Value);
        };
        _itemsListView.RowRender += (_, args) =>
        {
            args.RowAttribute = args.Row == _itemsListView.SelectedItem ?
                                new Attribute(Color.Black, Color.White) :
                                new Attribute(Color.White, Color.Black);
        };
        _engine.Player.Inventory.Items.CollectionChanged += (_, _) =>
        {
            if (_engine.Player.Inventory.Items.Count != 1) return;
            _itemsListView.SelectedItem = 0;
        };
        
        itemsFrame.Add(_itemsListView, _itemTooltipOverlay);
        
        // Attributes Frame
        var attributesFrame = new View()
        {
            X = Pos.Right(itemsFrame),
            Y = Pos.Bottom(handsFrame),
            Width = attributesFrameWidth,
            Height = Dim.Fill(),
            Title = " Attributes ",
            BorderStyle = LineStyle.Rounded
        };
        _attributesLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill() };
        attributesFrame.Add(_attributesLabel);
        
        // Money Frame
        var monyFrameWidth = 12;
        var accountFrame = new View()
        {
            X = 0,
            Y = Pos.Bottom(itemsFrame),
            Width = monyFrameWidth,
            Height = Dim.Fill(),
            Title = " Bundle ",
            BorderStyle = LineStyle.Rounded
        };
        _accountLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill(), }; 
        accountFrame.Add(_accountLabel);
        
        
        
        inventoryFrame.Add(handsFrame, itemsFrame, attributesFrame, accountFrame);
        
        _mainWindow.Add(mapFrame, inventoryFrame, _howToPlayOverlay); // _howToPlayOverlay always at the end.
    }
}