using maze_runner.Commands.TerminalUI;
using maze_runner.Core.Logger;
using maze_runner.Dungeon.Strategies;
using maze_runner.Dungeon.Themes.Library;
using maze_runner.Items.Models;

namespace maze_runner.Core.Engine;
using Commands.Core;
using Terminal.Gui;
using System.Text;

public class TerminalUIManager : IGameUIManager
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
    
    public GameEngine Engine { get; }

    public TerminalUIManager(GameEngine engine)
    {
        Engine = engine;

        Engine.GlobalInput.RegisterCommand((Key)'?', new ToggleHelp(this), "Toggle help menu");
        Engine.GlobalInput.RegisterCommand(Key.Esc, new Quit(this), "Quit game");
        Engine.GlobalInput.RegisterCommand(Key.I, new ToggleItemInfo(this), "Toggle in-game info overlay");
        Engine.GlobalInput.RegisterCommand(Key.Tab, new Reload(this), "Reload game");
        Engine.GlobalInput.RegisterCommand(Key.J, new ToggleJournal(this), "Toggle journal overlay" );
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
        HowToPlayOverlay();
        JournalOverlay();
        TileInfoOverlay();
        ItemInfoOverlay();
        MapDisplay();
        InventoryDisplay();
        _mainWindow.SetNeedsDraw();
    }
    
    public void ToggleItemInfo() => _itemInfoToggle ^= 1;
    public void ToggleHelp() => _howToPlayOverlayToggle ^= 1;
    public void ToggleJournal() => _journalOverlayToggle ^= 1;
    public void Quit() => Application.RequestStop();
    public void Reload() => Engine.LoadLevel(new LibraryTheme());

    private void HandleKeyboard(object sender, Key e)
    {
        if (!Engine.GlobalInput.ProcessInput(e.KeyCode))
            if (!Engine.CurrentLevel.InputHandler.ProcessInput(e.KeyCode))
                EventTopic<UnknownInputEvent>.Publish(new UnknownInputEvent(e));

        e.Handled = true;
        Render();

        var player = Engine.CurrentLevel.EntityManager.Player;
        if (!player.IsAlive)
        {
            ShowGameOverModal();
            player.Health = player.MaxHealth;
            Render();
        }
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
        var sb = new StringBuilder(Engine.CurrentLevel.Map.Rows * (Engine.CurrentLevel.Map.Cols + Environment.NewLine.Length));
        var player = Engine.CurrentLevel.EntityManager.Player;

        for (int i = 0; i < Engine.CurrentLevel.Map.Rows; i++)
        {
            for (int j = 0; j < Engine.CurrentLevel.Map.Cols; j++)
            {
                var entity = Engine.CurrentLevel.EntityManager.GetAnyEntityExceptPlayerAt(i, j);

                sb.Append(entity?.Symbol ?? Engine.CurrentLevel.Map.GetTile(i, j).GetTileSymbol());
            }

            sb.AppendLine();
        }
        // Player always on top
        sb[player.Position.Row * (Engine.CurrentLevel.Map.Cols + Environment.NewLine.Length) + player.Position.Col] = player.Symbol;

        _mapLabel.Text = sb.ToString();
    }

    private void InventoryDisplay()
    {
        var player = Engine.CurrentLevel.EntityManager.Player;
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
                                    {Engine.Config.PlayerName}
                                        Health:     {player.Health}/{player.MaxHealth}
                                        Dexterity:  {player.CurrentStats.Dexterity}
                                        Stamina:    {player.CurrentStats.Stamina}
                                        Strength:   {player.CurrentStats.Strength}
                                        Resistance: {player.CurrentStats.Resistance}
                                        Luck:       {player.CurrentStats.Luck}
                                        Wisdom:     {player.CurrentStats.Wisdom}
                                    
                                    """;
        var entity = Engine.CurrentLevel.EntityManager.GetAnyEntityExceptPlayerAt(Engine.CurrentLevel.EntityManager.Player.Position.Row, Engine.CurrentLevel.EntityManager.Player.Position.Col);
        if (entity != null)
            _attributesLabel.Text += $"""
                                      Enemy
                                          Health:    {entity.Health}/{entity.MaxHealth}
                                          Damage:    {entity.BaseDamage}
                                          Defense:   {entity.BaseDefense} 
                                      """;

    }
    
    private void TileInfoOverlay()
    {
        var currentTile = Engine.CurrentLevel.Map.GetTile(Engine.CurrentLevel.EntityManager.Player.Position.Row, Engine.CurrentLevel.EntityManager.Player.Position.Col);
        if (currentTile.Items.Any() && _itemInfoToggle == 1)
        {
            _tileInfoTextView.Text = string.Empty;
            foreach (var item in currentTile.Items)
                _tileInfoTextView.Text += $"{item.Name}({item.TileSymbol})\n";

            var terminalX = Engine.CurrentLevel.EntityManager.Player.Position.Col - TileInfoWidth / 2;
            var terminalY = Engine.CurrentLevel.EntityManager.Player.Position.Row - TileInfoHeight;

            if (terminalY < 0) terminalY = Engine.CurrentLevel.EntityManager.Player.Position.Row + 1;
            if (terminalX < 0) terminalX = 0;

            if (terminalX + TileInfoWidth > Engine.CurrentLevel.Map.Cols)
            {
                terminalX = Engine.CurrentLevel.EntityManager.Player.Position.Col - TileInfoWidth - 1;
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
            
            sb.AppendLine("Level name: " + Engine.CurrentLevel.LevelName);
            sb.AppendLine(Engine.CurrentLevel.Description);
            sb.AppendLine();
            sb.AppendLine("1. General\n" + Engine.GlobalInput.ToString());
            sb.AppendLine();
            sb.AppendLine("2. Ingame actions\n" + Engine.CurrentLevel.InputHandler.ToString());
            
            _howToPlayTextView.Text = sb.ToString();
            _howToPlayOverlay.Visible = true;
        }
        else
        {
            _howToPlayOverlay.Visible = false;
        }
    }

    private void JournalOverlay()
    {
        if (_journalOverlayToggle == 1)
        {
            var file = Directory
                .GetFiles(Engine.Config.LogDirectoryPath)
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
        }
    }

    private void ShowGameOverModal()
    {
        if (Engine.CurrentLevel.EntityManager.Player.IsAlive)
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
            Reload();
        }
    }

    void BuildUI()
    {

        
        var mapFrame = new View()
        {
            X = 0,
            Y = 0,
            Width = Engine.CurrentLevel.Map.Cols + 2,
            Height = Engine.CurrentLevel.Map.Rows + 2,
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
        
        _itemsListView.SetSource(Engine.CurrentLevel.EntityManager.Player.Inventory.Items);
        _itemsListView.CollectionChanged += (_, args) =>
        {
            var items = Engine.CurrentLevel.EntityManager.Player.Inventory.Items;
            if (items.Count <= 0)
            {
                _tooltipTextView.Text = string.Empty;
                _itemTooltipOverlay.SetNeedsDraw();
                return;
            }

            if (args.OldStartingIndex == Engine.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex)
            {
                ItemInfoWrite(items[Engine.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex]);
            }
            if (args.NewStartingIndex == Engine.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex)
                ItemInfoWrite(items[Engine.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex]);
        };
        _itemsListView.SelectedItemChanged += (_, args) =>
        {
            Engine.CurrentLevel.EntityManager.Player.Inventory.CurrentIndex = args.Item;
            ItemInfoWrite((Item)args.Value);
        };
        _itemsListView.RowRender += (_, args) =>
        {
            args.RowAttribute = args.Row == _itemsListView.SelectedItem ?
                                new Attribute(Color.Black, Color.White) :
                                new Attribute(Color.White, Color.Black);
        };
        Engine.CurrentLevel.EntityManager.Player.Inventory.Items.CollectionChanged += (_, _) =>
        {
            if (Engine.CurrentLevel.EntityManager.Player.Inventory.Items.Count != 1) return;
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
            Width = Dim.Fill() - attributesFrameWidth,
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
        journalListView.SetSource(Engine.Logs.Messages);
        
        inventoryFrame.Add(handsFrame, itemsFrame, attributesFrame, accountFrame, journalFrame);
        
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