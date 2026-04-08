using maze_runner.Commands.Core;
using maze_runner.Commands.Player;
using maze_runner.Entities;
using maze_runner.Entities.Combat;
using maze_runner.Entities.Mobs;
using maze_runner.Items.Modifiers;
using Terminal.Gui;

namespace maze_runner.Dungeon.Builders;
using Items.Models; 
using Items.Weapons;
using Items.UselessItems;
using Map;

public class ProcDungeonBuilder : IBaseDungeonBuilder, IModifierDungeonBuilder
{
    private Map _map = new();
    private readonly List<Room> _rooms = new();
    private Random _random = new();
    private List<(int, int)> _spawnableCords = new();
    private InputHandler _inputHandler = new();
    private EntityManager _entityManager = new();

    // Add new potential items to spawn
    private readonly IReadOnlyList<Item> _weaponsProt = [new Knife(), new LongSword(), new Sword(), new Cubix()];
    private readonly IReadOnlyList<Item> _uselessItemsProt = [new Bottle(), new Feather(), new Stick()];

    public IModifierDungeonBuilder CreateEmptyDungeon(int width, int height)
    {
        _map = new Map(height, width);

        for (int i = 0; i < _map.Rows; ++i)
            for (int j = 0; j < _map.Cols; ++j)
                _map.TrySetTile(i,j, new FloorTile());

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _spawnableCords.Add((x, y));
        
        _inputHandler.RegisterCommand(Key.W, new Move(-1, 0), "Move up");
        _inputHandler.RegisterCommand(Key.S, new Move(1, 0), "Move down");
        _inputHandler.RegisterCommand(Key.A, new Move(0, -1), "Move left");
        _inputHandler.RegisterCommand(Key.D, new Move(0, 1), "Move right");
        
        return this;
    }

    public IModifierDungeonBuilder CreateFullDungeon(int width, int height)
    {
        _map = new Map(height, width);
        
        for (int i = 0; i < _map.Rows; ++i)
            for (int j = 0; j < _map.Cols; ++j)
                _map.TrySetTile(i,j, new WallTile());
        
        _spawnableCords.Clear();
        
        _inputHandler.RegisterCommand(Key.W, new Move(-1, 0), "Move up");
        _inputHandler.RegisterCommand(Key.S, new Move(1, 0), "Move down");
        _inputHandler.RegisterCommand(Key.A, new Move(0, -1), "Move left");
        _inputHandler.RegisterCommand(Key.D, new Move(0, 1), "Move right");
        
        return this;
    }

    public IModifierDungeonBuilder AddCentralRoom(int width, int height, bool secure)
    {
        int x = (_map.Cols - width) / 2;
        int y = (_map.Rows - height) / 2;

        var central = new Room(x, y, width, height);
        CraveRoom(central);
        _rooms.Add(central);

        var boss = new MainBoss();
        boss.Position = ((_map.Rows - 1) / 2, (_map.Cols - 1) / 2);
        _entityManager.AddEntity(boss);
        
        _inputHandler.RegisterCommand(KeyCode.Z, new Attack(new NormalAttack()), "Perform -normal- attack");
        _inputHandler.RegisterCommand(KeyCode.X, new Attack(new StealthAttack()), "Perform -stealth- attack");
        _inputHandler.RegisterCommand(KeyCode.C, new Attack(new MagicAttack()), "Perform -magic- attack");
        
        return this;
    }

    public IModifierDungeonBuilder AddRooms(int maxCount)
    {
        int maxAttempts = maxCount * 7;
        for (int i = 0; i < maxAttempts && _rooms.Count < maxCount; ++i)
        {
            int width = _random.Next(3, 7);
            int height = _random.Next(3, 7);
            
            int x = _random.Next(1, _map.Cols - width - 1);
            int y = _random.Next(1, _map.Rows - height - 1);
            
            var room = new Room(x, y, width, height);
            if (!_rooms.Any(r => r.Intersects(room)))
            {
                CraveRoom(room);
                _rooms.Add(room);
            }
        }
        
        return this;
    }

    public IModifierDungeonBuilder AddStartingRoom()
    {
        var (row, col) = _map.GetSpawningPosition();
        var spawnRoom = new Room(col, row, 2, 2);
        CraveRoom(spawnRoom);
        _rooms.Add(spawnRoom);
        return this;
    }

    public IModifierDungeonBuilder AddWeapons(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var randomCords = _spawnableCords[_random.Next(_spawnableCords.Count)];
            var prototype = _weaponsProt[_random.Next(_weaponsProt.Count)];
            
            _map.GetTile(randomCords.Item2, randomCords.Item1).AddItem(new UnluckyModifier(new SharpnessModifier(prototype.Clone())));
        }

        _inputHandler.RegisterCommand(Key.E, new PickUp(), "Pick item from the ground");
        _inputHandler.RegisterCommand(Key.Q, new Drop(), "Drop selected item from inventory");
        _inputHandler.RegisterCommand(Key.F, new Equip(), "Equip selected item");
        _inputHandler.RegisterCommand(Key.F.WithShift, new Unequip(), "Unequip item (from Hands)");
        
        return this;
    }

    public IModifierDungeonBuilder AddUselessItems(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var randomCords = _spawnableCords[_random.Next(_spawnableCords.Count)];
            var prototype = _uselessItemsProt[_random.Next(_uselessItemsProt.Count)];
            
            _map.GetTile(randomCords.Item2, randomCords.Item1).AddItem(new UnluckyModifier(prototype.Clone()));
        }

        _inputHandler.RegisterCommand(Key.E, new PickUp(), "Pick item from the ground");
        _inputHandler.RegisterCommand(Key.Q, new Drop(), "Drop selected item from inventory");
        _inputHandler.RegisterCommand(Key.F, new Equip(), "Equip selected item");
        _inputHandler.RegisterCommand(Key.F.WithShift, new Unequip(), "Unequip item (from Hands)");
        
        return this;
    }

    public (Map, InputHandler, EntityManager) Build() => (_map, _inputHandler, _entityManager);

    public IModifierDungeonBuilder ConnectRooms()
    {
        for (int i = 1; i < _rooms.Count; ++i)
        {
            int startX = _rooms[i - 1].CenterX;
            int startY = _rooms[i - 1].CenterY;
            int endX = _rooms[i].CenterX;
            int endY = _rooms[i].CenterY;

            if (_random.Next(2) == 0)
            {
                CravePassthroughH(startX, endX, startY);
                CravePassthroughV(startY, endY, endX);
            }
            else
            {
                CravePassthroughV(startY, endY, startX);
                CravePassthroughH(startX, endX, endY);
            }
        }
        return this;
    }

    private void CravePassthroughH(int x1, int x2, int y)
    {
        int start = Math.Min(x1, x2);
        int end = Math.Max(x1, x2);
        for (int x = start; x <= end; ++x)
        {
            _map.TrySetTile(y, x, new FloorTile());
            _spawnableCords.Add((x, y));
        }
    }

    private void CravePassthroughV(int y1, int y2, int x)
    {
        int start = Math.Min(y1, y2);
        int end = Math.Max(y1, y2);
        for (int y = start; y <= end; ++y)
        {
            _map.TrySetTile(y, x, new FloorTile());
            _spawnableCords.Add((x, y));
        }
    }

    private void CraveRoom(Room room)
    {
        for (int i = room.Y; i < room.Y + room.Height; ++i)
            for (int j = room.X; j < room.X + room.Width; ++j)
            {
                if (!_map.TrySetTile(i, j, new FloorTile()))
                    continue;
                _spawnableCords.Add((j, i));
            }
    }
}