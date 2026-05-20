using System.Collections.ObjectModel;
using System.Text;
using maze_runner.Network.DTOs.GameState;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace maze_runner.View.TerminalGui;

public class TerminalFrontend : IGameFrontend
{
    private Window _mainWindow           = new();
    private Label _mapLabel              = new();
    private Label _leftHandLabel         = new();
    private Label _rightHandLabel        = new();
    private Label _attributesLabel       = new();
    private Label _accountLabel          = new();
    private ListView _itemsListView      = new();
    
    private Terminal.Gui.View _tileInfoOverlay        = new();
    private TextView _tileInfoTextView   = new();
    private Terminal.Gui.View _itemTooltipOverlay     = new();
    private TextView _tooltipTextView    = new();

    private Terminal.Gui.View _howToPlayOverlay      = new();
    private TextView _howToPlayTextView = new();

    private Terminal.Gui.View _journalView = new();
    private TextView _journalTextView = new();
    
    private const int TileInfoWidth     = 15;
    private const int TileInfoHeight    = 5;
    
    private int _itemInfoToggle = 0;
    private int _howToPlayOverlayToggle = 1;
    private int _journalOverlayToggle = 0;
    
    private InputHandler _uiInput = new();
    private GameStateSnapshotDto? _state;
    private readonly int _myPlayerId;

    public event Action<char>? OnKeyPressed;
    
    public TerminalFrontend(int myPlayerId)
    {
        _myPlayerId = myPlayerId;
        
        _uiInput.RegisterCommand('?', ToggleHelp , "Toggle help menu");
        _uiInput.RegisterCommand('i', ToggleItemInfo, "Toggle in-game info overlay");
        _uiInput.RegisterCommand('j', ToggleJournal, "Toggle journal overlay" ); 
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
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(),
            Title = "Maze Runner",
            ColorScheme = scheme,
        };
        
        BuildUI();

        _mainWindow.Loaded += (sender, args) =>
        {
            Render();
        };
        
        _mainWindow.KeyDown += HandleKeyboard!;
        _mainWindow.MouseClick += HandleMouse!;

        Application.Run(_mainWindow);
        
        _mainWindow.Dispose();
        Application.Shutdown();        
    }
    
    public void RenderSnapshot(GameStateSnapshotDto newState)
    {
        _state = newState;

        Application.Invoke(() =>
        {
            var me = GetMe();
            // if (me != null && !me.IsAlive) ShowGameOverModal();
            
            Render();
        });
    }
    
    private PlayerDto? GetMe() => _state?.Players.FirstOrDefault(p => p.Id == _myPlayerId);

    public void ToggleItemInfo() => _itemInfoToggle ^= 1;
    public void ToggleHelp() => _howToPlayOverlayToggle ^= 1;
    public void ToggleJournal() => _journalOverlayToggle ^= 1;
    public void Quit() => Application.RequestStop();

    private void HandleKeyboard(object sender, Key e)
    {
        char input;
        if (e.AsRune.IsAscii)
            input = (char)e.AsRune.Value;
        else
            input = (char)((uint)e.KeyCode & 0xFFFF);
        
        if (input == (char)27) Quit();
        
        if (!_uiInput.ProcessInput(input))
            OnKeyPressed?.Invoke(input);
        
        e.Handled = true;
        Render();
    }

    void Render()
    {
        if (_state == null) return;
        
        HowToPlayWrite();
        JournalOverlay();
        TileInfoOverlay();
        ItemInfoOverlay();
        MapDisplay();
        InventoryDisplay();
        _mainWindow.SetNeedsDraw();
    }
    
    private void HandleMouse(object sender, MouseEventArgs args)
    {
        args.Handled = true;
    }
    
    private void MapDisplay()
    {
        if (_state?.Map == null) return;
        var map = _state.Map;
        var me = GetMe();
        var grid = new char[map.Height, map.Width];

        for (int i = 0; i < map.Height; i++)
            for (int j = 0; j < map.Width; j++)
                grid[i, j] = map.Topology[i * map.Width + j];
        
        foreach (var dropped in map.DroppedItems)
        {
            if (dropped.Row >= 0 && dropped.Row < map.Height && dropped.Col >= 0 && dropped.Col < map.Width)
                grid[dropped.Row, dropped.Col] = dropped.Item.Symbol;
        }

        foreach (var entity in _state.Entities)
        {
            if (entity.Row >= 0 && entity.Row < map.Height && entity.Col >= 0 && entity.Col < map.Width)
                grid[entity.Row, entity.Col] = entity.Symbol;
        }
        
        foreach (var player in _state.Players)
        {
            if (player.Row >= 0 && player.Row < map.Height && player.Col >= 0 && player.Col < map.Width)
                grid[player.Row, player.Col] = player.Symbol;
        }

        var sb = new StringBuilder();
        for (int i = 0; i < map.Height; i++)
        {
            for (int j = 0; j < map.Width; j++)
            {
                sb.Append(grid[i, j]);
            }
            sb.AppendLine();
        }

        _mapLabel.Text = sb.ToString();
    }

    private void InventoryDisplay()
    {
        var me = GetMe();
        if (me == null) return;
        
        _leftHandLabel.Text = me.Inventory.LeftHand == null
            ? " "
            : $"{me.Inventory.LeftHand.Name}({me.Inventory.LeftHand.Symbol})";
        _rightHandLabel.Text = me.Inventory.RightHand == null 
            ? " " 
            : $"{me.Inventory.RightHand.Name}({me.Inventory.RightHand.Symbol})";

        var itemList = me.Inventory.Items.Select(i => $"{i.Name}({i.Symbol})");
        _itemsListView.SetSource(new ObservableCollection<string>(itemList));
        
        var sb = new StringBuilder();
        foreach (var item in me.Inventory.Items)
            sb.AppendLine($"{item.Name}({item.Symbol})");
        
        _accountLabel.Text = $"Gold:  {me.Inventory.Gold}\n" +
                             $"Coins: {me.Inventory.Coins}";
        _attributesLabel.Text =     $"""
                                    {me.Name}
                                        Health:     {me.Health}/{me.MaxHealth}
                                        Dexterity:  {me.Stats.Dexterity}
                                        Stamina:    {me.Stats.Stamina}
                                        Strength:   {me.Stats.Strength}
                                        Resistance: {me.Stats.Resistance}
                                        Luck:       {me.Stats.Luck}
                                        Wisdom:     {me.Stats.Wisdom}
                                    
                                    """;
        var entity = _state?.Entities.FirstOrDefault(e => e.Row == me.Row && e.Col == me.Col);
        if (entity != null)
            _attributesLabel.Text += $"""
                                      Enemy
                                          Health:    {entity.Health}/{entity.MaxHealth}
                                          Damage:    {entity.Damage}
                                          Defense:   {entity.Defense} 
                                      """;

    }
    
    private void TileInfoOverlay()
    {
        var me = GetMe();
        if (me == null || _state?.Map == null) return;
        
        var itemsOnMyTile = _state.Map.DroppedItems.Where(i => i.Row == me.Row && i.Col == me.Col).ToList();
        if (itemsOnMyTile.Any() && _itemInfoToggle == 1)
        {
            _tileInfoTextView.Text = string.Empty;
            foreach (var dto in itemsOnMyTile) 
                _tileInfoTextView.Text += $"{dto.Item.Name}({dto.Item.Symbol})\n";

            var terminalX = me.Col - TileInfoWidth / 2;
            var terminalY = me.Row - TileInfoHeight;

            if (terminalY < 0) terminalY = me.Row + 1;
            if (terminalX < 0) terminalX = 0;

            if (terminalX + TileInfoWidth > _state.Map.Width)
            {
                terminalX = me.Col - TileInfoWidth - 1;
                if (terminalX < 0) terminalX = 0;
            }
            
            _tileInfoOverlay.X = terminalX;
            _tileInfoOverlay.Y = terminalY;

            if (!_tileInfoOverlay.Visible) _tileInfoOverlay.Visible = true;
            _tileInfoOverlay.SetNeedsDraw();
        }
        else
        {
            if (_tileInfoOverlay.Visible) _tileInfoOverlay.Visible = false;
        }
    }
    
    private void ItemInfoOverlay() => _itemTooltipOverlay.Visible = _itemInfoToggle != 0;
    
    private void ItemInfoWrite(ItemDto item)
    {
        string text = $"""
                       ({item.Symbol}) {item.Name}
                       """;
        if (item.Damage.HasValue)
        {
            text += $"""

                     Damage: {item.Damage}
                     """;
        }

        _tooltipTextView.Text = text;
    }

    private void HowToPlayWrite()
    {
        if (_state == null) return;
        
        var sb = new StringBuilder();
        
        sb.AppendLine("Level name: " + _state.LevelMeta.Name); // TODO
        sb.AppendLine(_state.LevelMeta.Description);
        sb.AppendLine();
        sb.AppendLine("1. UI control");
        sb.AppendLine(_uiInput.ToString());
        sb.AppendLine();
        sb.AppendLine("2. In-game actions");
        foreach (var cmd in _state.LevelMeta.Commands)
            sb.AppendLine($"{cmd.Key} - {cmd.Description}");
    
        _howToPlayTextView.Text = sb.ToString();
        _howToPlayOverlay.Visible = _howToPlayOverlayToggle == 1;
    }

    private void JournalOverlay()
    {
        if (_journalOverlayToggle == 1 && _state != null)
        {
            _journalTextView.Text += _state.RecentLogs;
            _journalView.Visible = true;
        }
        else
        {
            _journalView.Visible = false;
        }
    }

    private void ShowGameOverModal()
    {
        var me = GetMe();
        if (me == null) return;
        if (me.IsAlive) return;
        
        var gameOverDialog = new Dialog()
        {
            Title = " Game Over ",
            Width = 40,
            Height = 10,
            X = Pos.Center(),
            Y = Pos.Center(),
        };

        var deathLabel = new Label()
        {
            Text = "Sadly you didn't make it :(",
            X = Pos.Center(),
            Y = 2
        };

        var btnRestart = new Button()
        {
            Text = "New Game"
        };

        var btnQuit = new Button()
        {
            Text = "Exit"
        };

        var buttonContainer = new Terminal.Gui.View()
        {
            X = Pos.Center(),
            Y = Pos.Bottom(deathLabel) + 2,
            Width = Dim.Fill(),
            Height = 1
        };

        btnRestart.X = Pos.Center() - 8;
        btnQuit.X = Pos.Right(btnRestart) + 2;

        buttonContainer.Add(btnRestart, btnQuit);

        var shouldQuit = false;
        btnQuit.Accepting += (sender, args) =>
        {
            shouldQuit = true;
            Application.RequestStop();
        };

        btnRestart.Accepting += (sender, args) =>
        {
            shouldQuit = false;
            Application.RequestStop(); 
        };

        gameOverDialog.Add(deathLabel, buttonContainer);

        Application.Run(gameOverDialog);
        
        gameOverDialog.Dispose();
        if (shouldQuit)
        {
            Quit();
        }
        else
        {
            Quit();
        }
    }

    void BuildUI()
    {
        var mapFrame = new Terminal.Gui.View()
        {
            X = 0,
            Y = 0,
            Width = 40 + 2,
            Height = 20 + 2,
            Title = " Map ",
            BorderStyle = LineStyle.Single
        };
        _mapLabel = new Label()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        _tileInfoOverlay = new Terminal.Gui.View()
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



        var inventoryFrame = new Terminal.Gui.View()
        {
            X = Pos.Right(mapFrame) + 1,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Height(mapFrame),
            BorderStyle = LineStyle.Single
        };

        // Hands Frame
        var handsFrame = new Terminal.Gui.View()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 5,
            Title = " Hands ",
            BorderStyle = LineStyle.Rounded,
        };
        var leftHandFrame = new Terminal.Gui.View()
            { X = 0, Y = 0, Width = Dim.Percent(50), Height = 3, BorderStyle = LineStyle.Single };
        var rightHandFrame = new Terminal.Gui.View()
        {
            X = Pos.Right(leftHandFrame), Y = 0, Width = Dim.Percent(50), Height = 3, BorderStyle = LineStyle.Single
        };
        _leftHandLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill() };
        leftHandFrame.Add(_leftHandLabel);
        _rightHandLabel = new Label() { Width = Dim.Fill(), Height = Dim.Fill() };
        rightHandFrame.Add(_rightHandLabel);
        handsFrame.Add(leftHandFrame, rightHandFrame);

        // Inventory.Items Frame
        var attributesFrameWidth = 25;
        
        var itemsFrame = new Terminal.Gui.View()
        {
            X = 0,
            Y = Pos.Bottom(handsFrame),
            Width = Dim.Func(() => Math.Max(1, (inventoryFrame.Frame.Width > 0 ? inventoryFrame.Frame.Width : 100) - attributesFrameWidth)),
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
        _itemTooltipOverlay = new Terminal.Gui.View()
        {
            X = 20,
            Y = -1,
            Width = Dim.Func(() => Math.Max(1, itemsFrame.Frame.Width - 20)),
            Height = Dim.Height(itemsFrame),
            BorderStyle = LineStyle.Rounded,
            Visible = false,
        };
        _tooltipTextView = new TextView() { Width = Dim.Fill(), Height = Dim.Fill(), WordWrap = true, ReadOnly = true };
        _itemTooltipOverlay.Add(_tooltipTextView);
        
        _itemsListView.SelectedItemChanged += (_, args) =>
        {
            var me = GetMe();
            if (me == null || args.Item >= me.Inventory.Items.Count) return;
            ItemInfoWrite(me.Inventory.Items[args.Item]);
        };
        _itemsListView.RowRender += (_, args) =>
        {
            args.RowAttribute = args.Row == _itemsListView.SelectedItem ?
                                new Attribute(Color.Black, Color.White) :
                                new Attribute(Color.White, Color.Black);
        };
        
        itemsFrame.Add(_itemsListView, _itemTooltipOverlay);
        
        // Attributes Frame
        var attributesFrame = new Terminal.Gui.View()
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
        var accountFrame = new Terminal.Gui.View()
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
        
        // Journal Frame
        var journalFrame = new Terminal.Gui.View()
        {
            X = Pos.Right(accountFrame),
            Y = accountFrame.Y,
            Width = Dim.Fill() - attributesFrameWidth + 2, 
            Height = Dim.Fill(),
            Title = " Journal ",
            BorderStyle = LineStyle.Rounded
        };
        var journalListView = new ListView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            AllowsMarking = false,
        };
        journalFrame.Add(journalListView);
        
        inventoryFrame.Add(handsFrame, itemsFrame, journalFrame, attributesFrame, accountFrame);
        
        _howToPlayOverlay = new Terminal.Gui.View()
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
            ReadOnly = true,
        };
        _howToPlayOverlay.Add(_howToPlayTextView);
        _journalView = new Terminal.Gui.View()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Title = " Journal ",
            BorderStyle = LineStyle.Single,
            Visible = false,
        };
        _journalTextView = new TextView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            WordWrap = true,
            ReadOnly = true,
        };
        _journalView.Add(_journalTextView);
        
        _mainWindow.Add(mapFrame, inventoryFrame, _journalView, _howToPlayOverlay); // _howToPlayOverlay always at the end.
    }

}