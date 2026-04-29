using maze_runner.Commands.Core;
using maze_runner.Commands.Player;
using maze_runner.Core;
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
    private readonly List<Room> _rooms = [];
    private readonly Random _random = new();
    private readonly List<(int, int)> _spawnableCords = [];
    private readonly LevelContext _ctx = new(); 

    // Add new potential items to spawn
    private readonly IReadOnlyList<Item> _weaponsProt = [new Knife(), new LongSword(), new Sword(), new Cubix()];
    private readonly IReadOnlyList<Item> _uselessItemsProt = [new Bottle(), new Feather(), new Stick()];
    
    public IModifierDungeonBuilder CreateEmptyDungeon(int width, int height)
    {
        _ctx.Map = new Map(height, width);

        for (int i = 0; i < _ctx.Map.Rows; ++i)
            for (int j = 0; j < _ctx.Map.Cols; ++j)
                _ctx.Map.TrySetTile(i,j, new FloorTile());

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _spawnableCords.Add((x, y));
        
        _ctx.InputHandler.RegisterCommand('w', new Move(_ctx, -1, 0), "Move up");
        _ctx.InputHandler.RegisterCommand('s', new Move(_ctx, 1, 0), "Move down");
        _ctx.InputHandler.RegisterCommand('a', new Move(_ctx, 0, -1), "Move left");
        _ctx.InputHandler.RegisterCommand('d', new Move(_ctx, 0, 1), "Move right");
        
        return this;
    }

    public IModifierDungeonBuilder CreateFullDungeon(int width, int height)
    {
        _ctx.Map = new Map(height, width);
        
        for (int i = 0; i < _ctx.Map.Rows; ++i)
            for (int j = 0; j < _ctx.Map.Cols; ++j)
                _ctx.Map.TrySetTile(i,j, new WallTile());
        
        _spawnableCords.Clear();
        
        _ctx.InputHandler.RegisterCommand('w', new Move(_ctx, -1, 0), "Move up");
        _ctx.InputHandler.RegisterCommand('s', new Move(_ctx, 1, 0), "Move down");
        _ctx.InputHandler.RegisterCommand('a', new Move(_ctx, 0, -1), "Move left");
        _ctx.InputHandler.RegisterCommand('d', new Move(_ctx, 0, 1), "Move right");
        
        return this;
    }

    public IModifierDungeonBuilder AddCentralRoom(int width, int height, bool secure)
    {
        int x = (_ctx.Map.Cols - width) / 2;
        int y = (_ctx.Map.Rows - height) / 2;

        var central = new Room(x, y, width, height);
        CraveRoom(central);
        _rooms.Add(central);

        var boss = new MainBoss
        {
            Position = ((_ctx.Map.Rows - 1) / 2, (_ctx.Map.Cols - 1) / 2)
        };
        if (secure) _ctx.EntityManager.AddEntity(boss);

        return this;
    }

    public IModifierDungeonBuilder AddRooms(int maxCount)
    {
        int maxAttempts = maxCount * 7;
        for (int i = 0; i < maxAttempts && _rooms.Count < maxCount; ++i)
        {
            int width = _random.Next(3, 7);
            int height = _random.Next(3, 7);
            
            int x = _random.Next(1, _ctx.Map.Cols - width - 1);
            int y = _random.Next(1, _ctx.Map.Rows - height - 1);
            
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
        var (row, col) = _ctx.Map.GetSpawningPosition();
        var spawnRoom = new Room(col, row, 2, 2);
        CraveRoom(spawnRoom);
        _rooms.Add(spawnRoom);
        return this;
    }


    public IModifierDungeonBuilder PopulateItems(IItemPool pool, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var randomCords = _spawnableCords[_random.Next(_spawnableCords.Count)];
            _ctx.Map.GetTile(randomCords.Item1, randomCords.Item2).AddItem(pool.GetItem());
        }

        _ctx.InputHandler.RegisterCommand('e', new PickUp(_ctx), "Pick item from the ground");
        _ctx.InputHandler.RegisterCommand('q', new Drop(_ctx), "Drop selected item from inventory");
        _ctx.InputHandler.RegisterCommand('f', new Equip(_ctx), "Equip selected item");
        _ctx.InputHandler.RegisterCommand('F', new Unequip(_ctx), "Unequip item (from Hands)");
        
        return this;
    }

    public IModifierDungeonBuilder PopulateEnemies(IEnemyPool pool, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var randomCords = _spawnableCords[_random.Next(_spawnableCords.Count)];

            var enemy = pool.GetEntity();
            enemy.Position = randomCords;
            _ctx.EntityManager.AddEntity(enemy);
        }
        
        _ctx.InputHandler.RegisterCommand('z', new Attack(_ctx, new NormalAttack()), "Perform -normal- attack");
        _ctx.InputHandler.RegisterCommand('x', new Attack(_ctx, new StealthAttack()), "Perform -stealth- attack");
        _ctx.InputHandler.RegisterCommand('c', new Attack(_ctx, new MagicAttack()), "Perform -magic- attack");
        
        return this;
    }

    public IModifierDungeonBuilder PlaceArtifact(Item artifact)
    {
        var randomCords = _spawnableCords[_random.Next(_spawnableCords.Count)];
        _ctx.Map.GetTile(randomCords.Item1, randomCords.Item2).AddItem(artifact);

        return this;
    }

    public ILevelContext GetLevelContext() => _ctx; 

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
            _ctx.Map.TrySetTile(y, x, new FloorTile());
            _spawnableCords.Add((y, x));
        }
    }

    private void CravePassthroughV(int y1, int y2, int x)
    {
        int start = Math.Min(y1, y2);
        int end = Math.Max(y1, y2);
        for (int y = start; y <= end; ++y)
        {
            _ctx.Map.TrySetTile(y, x, new FloorTile());
            _spawnableCords.Add((y, x));
        }
    }

    private void CraveRoom(Room room)
    {
        for (int i = room.Y; i < room.Y + room.Height; ++i)
            for (int j = room.X; j < room.X + room.Width; ++j)
            {
                if (!_ctx.Map.TrySetTile(i, j, new FloorTile()))
                    continue;
                _spawnableCords.Add((i, j));
            }
    }
}