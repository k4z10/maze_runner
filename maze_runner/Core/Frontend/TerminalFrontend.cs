using maze_runner.Commands.TerminalUI;
using maze_runner.Core.Engine;
using maze_runner.Core.Logger;
using maze_runner.Items.Models;

namespace maze_runner.Core.Frontend;
using Commands.Core;
using Terminal.Gui;
using System.Text;

public class TerminalFrontend : IGameFrontend
{
    private Window _mainWindow           = new();
    private Label _mapLabel              = new();
    private Label _leftHandLabel         = new();
    private Label _rightHandLabel        = new();
    private Label _attributesLabel       = new();
    private Label _accountLabel          = new();
    private ListView _itemsListView      = new();
    
    private View _tileInfoOverlay        = new();
    private TextView _tileInfoTextView   = new();
    private View _itemTooltipOverlay     = new();
    private TextView _tooltipTextView    = new();

    private View _howToPlayOverlay      = new();
    private TextView _howToPlayTextView = new();

    private View _journalView = new();
    private TextView _journalTextView = new();
    
    private const int TileInfoWidth     = 15;
    private const int TileInfoHeight    = 5;
    
    private int _itemInfoToggle = 0;
    private int _howToPlayOverlayToggle = 1;
    private int _journalOverlayToggle = 0;
    
    
    
    private readonly IGameContext _gameContext;
    private readonly EventBus _uiEventBus;
    private ILevelContext? _lastSubscribedLevel;
    private InputHandler _uiInput = new(); 
    
    public TerminalFrontend(IGameContext gameContext, EventBus eb)
    {
        _gameContext = gameContext;
        _uiEventBus = eb;
        
        _uiInput.RegisterCommand('?', new ToggleHelp(this), "Toggle help menu");
        _uiInput.RegisterCommand('i', new ToggleItemInfo(this), "Toggle in-game info overlay");
        _uiInput.RegisterCommand('j', new ToggleJournal(this), "Toggle journal overlay" );
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
        
        BuildUI();
        
        _uiEventBus.Subscribe<LevelChanged>(_ => BindToCurrentLevel());

        _mainWindow.Loaded += (sender, args) =>
        {
            BindToCurrentLevel();
            Render();
            UpdateUIContext();
        };
        
        _mainWindow.KeyDown += HandleKeyboard!;
        _mainWindow.MouseClick += HandleMouse!;

        Application.Run(_mainWindow);
        
        _mainWindow.Dispose();
        Application.Shutdown();        
    }

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
        {
            _gameContext.EnqueueInput(input);
        }
        
        e.Handled = true;
    }

    private void OnRenderRequested(RenderFrame ctx)
    {
        Application.Invoke(() =>
        {
            var player = _gameContext.CurrentLevel.EntityManager.Player;
            if (!player.IsAlive)
            {
                ShowGameOverModal();
            }

            Render();
        });
    }
    
    private void BindToCurrentLevel()
    {
        if (_lastSubscribedLevel != null)
            _lastSubscribedLevel.EventBus.Unsubscribe<RenderFrame>(OnRenderRequested);

        _lastSubscribedLevel = _gameContext.CurrentLevel;
        
        if (_lastSubscribedLevel != null)
            _lastSubscribedLevel.EventBus.Subscribe<RenderFrame>(OnRenderRequested);
    }
    
    void Render()
    {
        HowToPlayOverlay();
        JournalOverlay();
        TileInfoOverlay();
        ItemInfoOverlay();
        MapDisplay();
        InventoryDisplay();
        _mainWindow.SetNeedsDraw();
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
    
    private void MapDisplay()
    {
        var sb = new StringBuilder(_gameContext.CurrentLevel.Map.Rows * (_gameContext.CurrentLevel.Map.Cols + Environment.NewLine.Length));
        var player = _gameContext.CurrentLevel.EntityManager.Player;

        for (int i = 0; i < _gameContext.CurrentLevel.Map.Rows; i++)
        {
            for (int j = 0; j < _gameContext.CurrentLevel.Map.Cols; j++)
            {
                var entity = _gameContext.CurrentLevel.EntityManager.GetAnyEntityExceptPlayerAt(i, j);

                sb.Append(entity?.Symbol ?? _gameContext.CurrentLevel.Map.GetTile(i, j).GetTileSymbol());
            }

            sb.AppendLine();
        }
        // Player always on top
        sb[player.Position.Row * (_gameContext.CurrentLevel.Map.Cols + Environment.NewLine.Length) + player.Position.Col] = player.Symbol;

        _mapLabel.Text = sb.ToString();
    }

    private void InventoryDisplay()
    {
        var player = _gameContext.CurrentLevel.EntityManager.Player;
        _leftHandLabel.Text = player.Inventory.LeftHand == null
            ? " "
            : $"{player.Inventory.LeftHand.Name}({player.Inventory.LeftHand.TileSymbol})";
        _rightHandLabel.Text = player.Inventory.RightHand == null 
            ? " " 
            : $"{player.Inventory.RightHand.Name}({player.Inventory.RightHand.TileSymbol})";

        var sb = new StringBuilder();
        foreach (var item in player.Inventory.Items)
            sb.AppendLine($"{item.Name}({item.TileSymbol})");
        
        _accountLabel.Text = $"Gold:  {player.Inventory.Gold}\n" +
                             $"Coins: {player.Inventory.Coins}";
        _attributesLabel.Text =     $"""
                                    {_gameContext.Config.PlayerName}
                                        Health:     {player.Health}/{player.MaxHealth}
                                        Dexterity:  {player.CurrentStats.Dexterity}
                                        Stamina:    {player.CurrentStats.Stamina}
                                        Strength:   {player.CurrentStats.Strength}
                                        Resistance: {player.CurrentStats.Resistance}
                                        Luck:       {player.CurrentStats.Luck}
                                        Wisdom:     {player.CurrentStats.Wisdom}
                                    
                                    """;
        var entity = _gameContext.CurrentLevel.EntityManager.GetAnyEntityExceptPlayerAt(_gameContext.CurrentLevel.EntityManager.Player.Position.Row, _gameContext.CurrentLevel.EntityManager.Player.Position.Col);
        if (entity != null)
            _attributesLabel.Text += $"""
                                      Enemy
                                          Health:    {entity.Health}/{entity.MaxHealth}
                                          Damage:    {entity.EffectiveDamage}
                                          Defense:   {entity.EffectiveDefense} 
                                      """;

    }
    
    private void TileInfoOverlay()
    {
        var currentTile = _gameContext.CurrentLevel.Map.GetTile(_gameContext.CurrentLevel.EntityManager.Player.Position.Row, _gameContext.CurrentLevel.EntityManager.Player.Position.Col);
        if (currentTile.Items.Any() && _itemInfoToggle == 1)
        {
            _tileInfoTextView.Text = string.Empty;
            foreach (var item in currentTile.Items)
                _tileInfoTextView.Text += $"{item.Name}({item.TileSymbol})\n";

            var terminalX = _gameContext.CurrentLevel.EntityManager.Player.Position.Col - TileInfoWidth / 2;
            var terminalY = _gameContext.CurrentLevel.EntityManager.Player.Position.Row - TileInfoHeight;

            if (terminalY < 0) terminalY = _gameContext.CurrentLevel.EntityManager.Player.Position.Row + 1;
            if (terminalX < 0) terminalX = 0;

            if (terminalX + TileInfoWidth > _gameContext.CurrentLevel.Map.Cols)
            {
                terminalX = _gameContext.CurrentLevel.EntityManager.Player.Position.Col - TileInfoWidth - 1;
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
        _howToPlayOverlay.Visible = _howToPlayOverlayToggle == 1;
    }

    private void UpdateUIContext()
    {
        _howToPlayOverlay.Visible = true;
        
        var sb = new StringBuilder();
        
        sb.AppendLine("Level name: " + _gameContext.CurrentLevel.LevelName);
        sb.AppendLine(_gameContext.CurrentLevel.Description);
        sb.AppendLine();
        sb.AppendLine("1. General\n" + _uiInput.ToString());
        sb.AppendLine();
        sb.AppendLine("2. Ingame actions\n" + _gameContext.CurrentLevel.InputHandler.ToString());
        
        _howToPlayTextView.Text = sb.ToString();
        HowToPlayOverlay();
    }

    private void JournalOverlay()
    {
        if (_journalOverlayToggle == 1)
        {
            var file = Directory
                .GetFiles(_gameContext.Config.LogDirectoryPath)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .FirstOrDefault();
            
            var journal = file == null ? string.Empty : File.ReadAllText(file.FullName);
            
            _journalTextView.Text = journal;
            _journalView.Visible = true;
        }
        else
        {
            _journalView.Visible = false;
            _journalTextView.Text = string.Empty;
        }
    }

    private void ShowGameOverModal()
    {
        if (_gameContext.CurrentLevel.EntityManager.Player.IsAlive)
            return;
        
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

        var buttonContainer = new View()
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
        var mapFrame = new View()
        {
            X = 0,
            Y = 0,
            Width = _gameContext.CurrentLevel.Map.Cols + 2,
            Height = _gameContext.CurrentLevel.Map.Rows + 2,
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
        var attributesFrameWidth = 25;
        
        var itemsFrame = new View()
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
        _itemTooltipOverlay = new View()
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
        
        _itemsListView.SetSource(_gameContext.CurrentLevel.EntityManager.Player.Inventory.Items);
        _itemsListView.CollectionChanged += (_, args) =>
        {
            var items = _gameContext.CurrentLevel.EntityManager.Player.Inventory.Items;
            if (items.Count <= 0)
            {
                _tooltipTextView.Text = string.Empty;
                _itemTooltipOverlay.SetNeedsDraw();
                return;
            }

            if (args.OldStartingIndex == _gameContext.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex)
            {
                ItemInfoWrite(items[_gameContext.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex]);
            }
            if (args.NewStartingIndex == _gameContext.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex)
                ItemInfoWrite(items[_gameContext.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex]);
        };
        _itemsListView.SelectedItemChanged += (_, args) =>
        {
            _gameContext.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex = args.Item;
            ItemInfoWrite((Item)args.Value);
        };
        _itemsListView.RowRender += (_, args) =>
        {
            args.RowAttribute = args.Row == _itemsListView.SelectedItem ?
                                new Attribute(Color.Black, Color.White) :
                                new Attribute(Color.White, Color.Black);
        };
        _gameContext.CurrentLevel.EntityManager.Player.Inventory.Items.CollectionChanged += (_, _) =>
        {
            if (_gameContext.CurrentLevel.EntityManager.Player.Inventory.Items.Count != 1) return;
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
        
        // Journal Frame
        var journalFrame = new View()
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
        var latestLogFile = Directory.GetFiles(_gameContext.Config.LogDirectoryPath)
            .Select(f => new FileInfo(f))
            .OrderByDescending(fi => fi.LastWriteTime)
            .FirstOrDefault()?.FullName;
        if (latestLogFile != null)
        {
            var logsFile = new ObservableFile(latestLogFile);
            journalListView.SetSource(logsFile.Lines);

            logsFile.Lines.CollectionChanged += (s, args) =>
            {
                journalListView.SetNeedsDraw();
                if (logsFile.Lines.Count > 0)
                {
                    journalListView.SelectedItem = logsFile.Lines.Count - 1;
                    journalListView.TopItem = Math.Max(0, logsFile.Lines.Count - journalFrame.Frame.Height - 2);
                }
            };
        }
        
        inventoryFrame.Add(handsFrame, itemsFrame, journalFrame, attributesFrame, accountFrame);
        
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
            ReadOnly = true,
        };
        _howToPlayOverlay.Add(_howToPlayTextView);
        _journalView = new View()
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