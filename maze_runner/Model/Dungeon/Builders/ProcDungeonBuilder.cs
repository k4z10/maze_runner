using maze_runner.Commands.Player;
using maze_runner.Core;
using maze_runner.Model.Core.Actions;
using maze_runner.Model.Dungeon.Map;
using maze_runner.Model.Entities.Combat;
using maze_runner.Model.Items.Models;
using maze_runner.Model.Items.UselessItems;
using maze_runner.Model.Items.Weapons;

namespace maze_runner.Model.Dungeon.Builders;

public class ProcDungeonBuilder : IBaseDungeonBuilder, IModifierDungeonBuilder
{
    private readonly List<Room> _rooms = [];
    private readonly Random _random = new();
    private readonly List<(int, int)> _spawnableCords = [];
    private readonly LevelContext _ctx = new(); 

    public IModifierDungeonBuilder CreateEmptyDungeon(int width, int height)
    {
        _ctx.Map = new Map.Map(height, width);

        for (int i = 0; i < _ctx.Map.Rows; ++i)
            for (int j = 0; j < _ctx.Map.Cols; ++j)
                _ctx.Map.TrySetTile(i,j, new FloorTile());

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _spawnableCords.Add((x, y));
        
        _ctx.CommandRegistry.RegisterCommand('w',"MOVE_U", new Move(-1, 0), "Move up");
        _ctx.CommandRegistry.RegisterCommand('s',"MOVE_D", new Move(1, 0), "Move down");
        _ctx.CommandRegistry.RegisterCommand('a',"MOVE_L", new Move(0, -1), "Move left");
        _ctx.CommandRegistry.RegisterCommand('d',"MOVE_R", new Move(0, 1), "Move right");
        
        return this;
    }

    public IModifierDungeonBuilder CreateFullDungeon(int width, int height)
    {
        _ctx.Map = new Map.Map(height, width);
        
        for (int i = 0; i < _ctx.Map.Rows; ++i)
            for (int j = 0; j < _ctx.Map.Cols; ++j)
                _ctx.Map.TrySetTile(i,j, new WallTile());
        
        _spawnableCords.Clear();
        
        _ctx.CommandRegistry.RegisterCommand('w',"MOVE_U", new Move(-1, 0), "Move up");
        _ctx.CommandRegistry.RegisterCommand('s',"MOVE_D", new Move(1, 0), "Move down");
        _ctx.CommandRegistry.RegisterCommand('a',"MOVE_L", new Move(0, -1), "Move left");
        _ctx.CommandRegistry.RegisterCommand('d',"MOVE_R", new Move(0, 1), "Move right");
        
        return this;
    }

    public IModifierDungeonBuilder AddCentralRoom(int width, int height)
    {
        int x = (_ctx.Map.Cols - width) / 2;
        int y = (_ctx.Map.Rows - height) / 2;

        var central = new Room(x, y, width, height);
        CraveRoom(central);
        _rooms.Add(central);

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

        _ctx.CommandRegistry.RegisterCommand('e', "PICKUP", new PickUp(), "Pick item from the ground");
        _ctx.CommandRegistry.RegisterCommand('q', "DROP", new Drop(), "Drop selected item from inventory");
        _ctx.CommandRegistry.RegisterCommand('f', "EQUIP", new Equip(), "Equip selected item");
        _ctx.CommandRegistry.RegisterCommand('F', "UNEQUIP", new Unequip(), "Unequip item (from Hands)");
        
        return this;
    }

    public IModifierDungeonBuilder PopulateEnemies(IEnemyPool pool, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            var randomCords = _spawnableCords[_random.Next(_spawnableCords.Count)];

            var enemy = pool.GetEntity((IEventPublisher)_ctx.EventBus, (IEventSubscriber)_ctx.EventBus);
            enemy.Position = randomCords;
            _ctx.EntityManager.AddEntity(enemy);
        }
        
        _ctx.CommandRegistry.RegisterCommand('z', "ATTACK_N", new Attack(new NormalAttack()), "Perform -normal- attack");
        _ctx.CommandRegistry.RegisterCommand('x', "ATTACK_S", new Attack(new StealthAttack()), "Perform -stealth- attack");
        _ctx.CommandRegistry.RegisterCommand('c', "ATTACK_M", new Attack(new MagicAttack()), "Perform -magic- attack");
        
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