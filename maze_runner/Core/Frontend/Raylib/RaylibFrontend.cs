using maze_runner.Commands.Core;
using maze_runner.Commands.TerminalUI;
using maze_runner.Core.Engine;
using maze_runner.Entities.Player;
using maze_runner.Items.Models;

namespace maze_runner.Core.Frontend.Raylib;
using Raylib_cs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;

public class RaylibFrontend : IGameFrontend
{
    private readonly IGameContext _gameContext;
    private readonly OverlayManager _overlayManager = new();
    private readonly InputHandler _uiInputHandler = new();
    private const int TileSize = 32;
    private const int TargetFps = 60;
    
    // Konfiguracja geometrii układu
    private const int ViewportWidth = 800; // Bazowy wymiar, ulegnie nadpisaniu
    private const int SidebarWidth = 450;  // Poszerzona przestrzeń robocza

    // Paleta kolorów UI
    private readonly Color _panelBackground = new Color(30, 30, 35, 255);
    private readonly Color _panelBorder = new Color(70, 70, 80, 255);
    private readonly Color _textColor = Color.RayWhite;
    private readonly Color _accentColor = Color.Gold;
    private readonly Color _healthColor = new Color(200, 40, 40, 255);

    private readonly Dictionary<char, Color> _tileColors = new()
    {
        { '█', Color.DarkGray },
        { ' ', Color.RayWhite },
        { '~', Color.Blue }
    };

    // Zasoby graficzne
    private Font _uiFont;
    private const string FontPath = "./resources/fonts/JetBrainsMonoNerdFontMono-Medium.ttf";

    // Pamięć podręczna topologii UI (Spatial Cache)
    private readonly Dictionary<Rectangle, Item> _inventoryHitboxes = new();
    private Rectangle _leftHandHitbox;
    private Rectangle _rightHandHitbox;

    // Kinematyczny stan obiektu przeciąganego
    private Item? _draggedItem;
    private Vector2 _dragOffset;
    private bool _isDraggedFromHands;

    public RaylibFrontend(IGameContext gameContext)
    {
        _gameContext = gameContext;
    }

    public void InitializeAndRun()
    {
        int viewportWidth = _gameContext.CurrentLevel.Map.Cols * TileSize;
        int viewportHeight = _gameContext.CurrentLevel.Map.Rows * TileSize;

        int screenWidth = viewportWidth + SidebarWidth;
        int screenHeight = Math.Max(viewportHeight, 600);

        Raylib.InitWindow(screenWidth, screenHeight, _gameContext.Config.PlayerName);
        
        // _uiFont = Raylib.GetFontDefault();
        _uiFont = Raylib.LoadFontEx(FontPath, 32, null, 0);

        Raylib.SetTargetFPS(TargetFps);

        while (!Raylib.WindowShouldClose() && _gameContext.IsRunning)
        {
            ProcessInput();
            RenderFrame(viewportWidth, screenHeight); 
        }

        Raylib.UnloadFont(_uiFont);
        Raylib.CloseWindow();
    }

    private void ProcessInput()
    {
        ProcessMouseInput();

        while (true)
        {
            var key = Raylib.GetKeyPressed();
            if (key == 0) break;

            var finalChar = (char)key;
            
            if (key == (int)KeyboardKey.Escape) finalChar = (char)27;
            else if (key == (int)KeyboardKey.Tab) finalChar = (char)9;

            if (!Raylib.IsKeyDown(KeyboardKey.LeftShift) && !Raylib.IsKeyDown(KeyboardKey.RightShift))
            {
                finalChar = char.ToLower(finalChar);
            }

            if (_overlayManager.HasActiveOverlays)
            {
                _overlayManager.UpdateAndProcessInput(finalChar);
            }
            else
            {
                if (_uiInputHandler.ProcessInput(finalChar)) continue;
                _gameContext.EnqueueInput(finalChar); 
            }
        }
    }

    private void ProcessMouseInput()
    {
        var player = _gameContext.CurrentLevel?.EntityManager?.Player;
        if (player == null || _overlayManager.HasActiveOverlays) return;

        Vector2 mousePos = Raylib.GetMousePosition();

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            foreach (var kvp in _inventoryHitboxes)
            {
                if (Raylib.CheckCollisionPointRec(mousePos, kvp.Key))
                {
                    _draggedItem = kvp.Value;
                    _dragOffset = new Vector2(mousePos.X - kvp.Key.X, mousePos.Y - kvp.Key.Y);
                    _isDraggedFromHands = false;
                    return;
                }
            }

            if (Raylib.CheckCollisionPointRec(mousePos, _leftHandHitbox) && player.Inventory.LeftHand != null)
            {
                _draggedItem = player.Inventory.LeftHand;
                _dragOffset = new Vector2(mousePos.X - _leftHandHitbox.X, mousePos.Y - _leftHandHitbox.Y);
                _isDraggedFromHands = true;
            }
            else if (Raylib.CheckCollisionPointRec(mousePos, _rightHandHitbox) && player.Inventory.RightHand != null)
            {
                _draggedItem = player.Inventory.RightHand;
                _dragOffset = new Vector2(mousePos.X - _rightHandHitbox.X, mousePos.Y - _rightHandHitbox.Y);
                _isDraggedFromHands = true;
            }
        }

        // Faza emisji zdarzenia (Release) - Propagacja logiki domenowej
        if (Raylib.IsMouseButtonReleased(MouseButton.Left) && _draggedItem != null)
        {
            if (!_isDraggedFromHands && (Raylib.CheckCollisionPointRec(mousePos, _leftHandHitbox) || Raylib.CheckCollisionPointRec(mousePos, _rightHandHitbox)))
            {
                player.Inventory.TryEquip(_draggedItem);
            }
            else if (_isDraggedFromHands)
            {
                if (!Raylib.CheckCollisionPointRec(mousePos, _leftHandHitbox) && !Raylib.CheckCollisionPointRec(mousePos, _rightHandHitbox))
                {
                    player.Inventory.TryUnequip(_draggedItem);
                }
            }

            _draggedItem = null; 
        }
    }

    private void RenderFrame(int viewportWidth, int screenHeight)
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        RenderMapAndItems();
        RenderEntities();
        
        RenderSidebar(viewportWidth, screenHeight);

        _overlayManager.RenderAll();

        // Renderowanie obiektu w układzie współrzędnych kursora (Najwyższy Z-Index)
        if (_draggedItem != null)
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            float dragSize = 64f; 
            float targetX = mousePos.X - _dragOffset.X;
            float targetY = mousePos.Y - _dragOffset.Y;

            // Cień sferyczny imitujący przesunięcie w osi Z
            Raylib.DrawRectangle((int)targetX + 5, (int)targetY + 5, (int)dragSize, (int)dragSize, Raylib.Fade(Color.Black, 0.5f));
            Raylib.DrawRectangle((int)targetX, (int)targetY, (int)dragSize, (int)dragSize, new Color(60, 60, 70, 255));
            Raylib.DrawRectangleLines((int)targetX, (int)targetY, (int)dragSize, (int)dragSize, _accentColor);
            
            DrawItemIcon(_draggedItem, targetX, targetY, dragSize);
        }

        Raylib.EndDrawing();
    }

    private void RenderMapAndItems()
    {
        var level = _gameContext.CurrentLevel;
        if (level?.Map == null) return;

        int rows = level.Map.Rows;
        int cols = level.Map.Cols;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var tile = level.Map.GetTile(r, c);

                char symbol = tile.GetTileSymbol();
                Color tileColor = _tileColors.GetValueOrDefault(symbol, Color.Magenta); 

                int x = c * TileSize;
                int y = r * TileSize;

                Raylib.DrawRectangle(x, y, TileSize, TileSize, tileColor);

                if (tile.Items.Count > 0)
                {
                    int itemOffset = TileSize / 4;
                    int itemSize = TileSize / 2;
                    Raylib.DrawRectangle(x + itemOffset, y + itemOffset, itemSize, itemSize, Color.Gold);
                }
            }
        }
    }

    private void RenderEntities()
    {
        var entityManager = _gameContext.CurrentLevel?.EntityManager;
        if (entityManager == null) return;

        foreach (var entity in entityManager.AllEntities)
        {
            if (!entity.IsAlive) continue;

            int x = entity.Position.Col * TileSize;
            int y = entity.Position.Row * TileSize;

            Color entityColor = entity == entityManager.Player ? Color.Green : Color.Red;
            Raylib.DrawCircle(x + TileSize / 2, y + TileSize / 2, TileSize / 2.2f, entityColor);

            string symbol = entity.Symbol.ToString();
            float fontSize = 20f;
            Vector2 textSize = Raylib.MeasureTextEx(_uiFont, symbol, fontSize, 0f);
            
            Vector2 textPos = new Vector2(
                x + (TileSize / 2f) - (textSize.X / 2f),
                y + (TileSize / 2f) - (textSize.Y / 2f)
            );

            Raylib.DrawTextEx(_uiFont, symbol, textPos, fontSize, 0f, Color.RayWhite);
        }
    }
    
    private void RenderSidebar(int startX, int screenHeight)
    {
        var player = _gameContext.CurrentLevel?.EntityManager?.Player;
        if (player == null) return;

        Raylib.DrawRectangle(startX, 0, SidebarWidth, screenHeight, _panelBackground);
        Raylib.DrawLine(startX, 0, startX, screenHeight, _panelBorder);

        int currentY = 20; 

        currentY = DrawPlayerHeader(player, startX, currentY);
        currentY = DrawStats(player, startX, currentY);
        DrawEquipmentAndInventory(player, startX, currentY, screenHeight); 
    }
    
    private int DrawPlayerHeader(Player player, int x, int y)
    {
        string name = _gameContext.Config.PlayerName;
        float fontSize = 28f;
        Vector2 nameSize = Raylib.MeasureTextEx(_uiFont, name, fontSize, 2f);
        
        Vector2 namePos = new Vector2(x + (SidebarWidth / 2f) - (nameSize.X / 2f), y);
        Raylib.DrawTextEx(_uiFont, name, namePos, fontSize, 2f, _accentColor);
        y += 40;

        int barWidth = SidebarWidth - 60;
        int barHeight = 24;
        int barX = x + 30;

        float healthRatio = Math.Clamp((float)player.Health / player.MaxHealth, 0f, 1f);
        int currentHealthWidth = (int)(barWidth * healthRatio);

        Raylib.DrawRectangle(barX, y, barWidth, barHeight, Color.DarkGray);
        Raylib.DrawRectangle(barX, y, currentHealthWidth, barHeight, _healthColor);
        Raylib.DrawRectangleLines(barX, y, barWidth, barHeight, _panelBorder);

        string hpText = $"Health: {player.Health}/{player.MaxHealth}";
        Vector2 hpTextSize = Raylib.MeasureTextEx(_uiFont, hpText, 20f, 1f);
        
        Vector2 hpPos = new Vector2(barX + (barWidth / 2f) - (hpTextSize.X / 2f), y + 2);
        Raylib.DrawTextEx(_uiFont, hpText, hpPos, 20f, 1f, _textColor);

        return y + 50;
    }

    private int DrawStats(Player player, int x, int y)
    {
        int leftCol = x + 30;
        int rightCol = x + 230; 
        int lineSpacing = 26;
        var stats = player.CurrentStats;

        Raylib.DrawTextEx(_uiFont, "ATTRIBUTES", new Vector2(leftCol, y), 22f, 1f, Color.LightGray);
        Raylib.DrawLine(leftCol, y + 25, x + SidebarWidth - 30, y + 25, _panelBorder);
        y += 35;

        Raylib.DrawTextEx(_uiFont, $"Strength:   {stats.Strength}", new Vector2(leftCol, y), 20f, 1f, _textColor);
        Raylib.DrawTextEx(_uiFont, $"Resistance: {stats.Resistance}", new Vector2(rightCol, y), 20f, 1f, _textColor);
        y += lineSpacing;

        Raylib.DrawTextEx(_uiFont, $"Dexterity:  {stats.Dexterity}", new Vector2(leftCol, y), 20f, 1f, _textColor);
        Raylib.DrawTextEx(_uiFont, $"Luck:       {stats.Luck}", new Vector2(rightCol, y), 20f, 1f, _textColor);
        y += lineSpacing;

        Raylib.DrawTextEx(_uiFont, $"Stamina:    {stats.Stamina}", new Vector2(leftCol, y), 20f, 1f, _textColor);
        Raylib.DrawTextEx(_uiFont, $"Wisdom:     {stats.Wisdom}", new Vector2(rightCol, y), 20f, 1f, _textColor);
        
        return y + 50;
    }

    private int DrawEquipmentAndInventory(Player player, int x, int y, int screenHeight)
    {
        int margin = x + 30;
        int slotSize = 64;
        int padding = 15;
        var inv = player.Inventory;

        // Ręce (Płaszczyzna docelowa upuszczania)
        Raylib.DrawTextEx(_uiFont, "EQUIPPED", new Vector2(margin, y), 22f, 1f, Color.LightGray);
        Raylib.DrawLine(margin, y + 25, x + SidebarWidth - 30, y + 25, _panelBorder);
        y += 35;

        _leftHandHitbox = new Rectangle(margin, y, slotSize, slotSize);
        _rightHandHitbox = new Rectangle(margin + slotSize + padding, y, slotSize, slotSize);

        DrawSlot(_leftHandHitbox, inv.LeftHand, "L");
        DrawSlot(_rightHandHitbox, inv.RightHand, "R");

        // Waluta wepchnięta obok rąk
        int currencyX = margin + (slotSize * 2) + (padding * 3);
        Raylib.DrawTextEx(_uiFont, $"Gold:  {inv.Gold}", new Vector2(currencyX, y + 10), 20f, 1f, _accentColor);
        Raylib.DrawTextEx(_uiFont, $"Coins: {inv.Coins}", new Vector2(currencyX, y + 35), 20f, 1f, Color.LightGray);

        y += slotSize + 40;

        // Siatka Ekwipunku
        Raylib.DrawTextEx(_uiFont, $"INVENTORY ({inv.Items.Count})", new Vector2(margin, y), 22f, 1f, Color.LightGray);
        Raylib.DrawLine(margin, y + 25, x + SidebarWidth - 30, y + 25, _panelBorder);
        y += 35;

        _inventoryHitboxes.Clear(); 
        int columns = 4; // Zwiększono do 4 kolumn ze względu na szerszy panel (450px)

        for (int i = 0; i < inv.Items.Count; i++)
        {
            var item = inv.Items[i];
            
            int col = i % columns;
            int row = i / columns;

            int slotX = margin + col * (slotSize + padding);
            int slotY = y + row * (slotSize + padding);

            if (slotY + slotSize > screenHeight - 20) break; 

            Rectangle slotRect = new Rectangle(slotX, slotY, slotSize, slotSize);
            _inventoryHitboxes[slotRect] = item;

            DrawSlot(slotRect, item, string.Empty);
        }

        return y + (inv.Items.Count / columns + 1) * (slotSize + padding);
    }

    private void DrawSlot(Rectangle rect, Item? item, string label)
    {
        Raylib.DrawRectangleRec(rect, new Color(40, 40, 45, 255));
        Raylib.DrawRectangleLinesEx(rect, 2f, _panelBorder);

        if (!string.IsNullOrEmpty(label))
        {
            Raylib.DrawTextEx(_uiFont, label, new Vector2(rect.X + 5, rect.Y + 5), 16f, 1f, Color.DarkGray);
        }

        if (item != null && item != _draggedItem)
        {
            DrawItemIcon(item, rect.X, rect.Y, rect.Width);
        }
    }

    private void DrawItemIcon(Item item, float x, float y, float size)
    {
        var symbol = item.TileSymbol.ToString();
        float fontSize = size * 0.6f;
        Vector2 textSize = Raylib.MeasureTextEx(_uiFont, symbol, fontSize, 0f);
        
        Vector2 pos = new Vector2(
            x + (size / 2f) - (textSize.X / 2f),
            y + (size / 2f) - (textSize.Y / 2f)
        );

        Raylib.DrawTextEx(_uiFont, symbol, pos, fontSize, 0f, _accentColor);
    }
}