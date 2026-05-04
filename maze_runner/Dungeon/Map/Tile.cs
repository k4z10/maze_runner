using maze_runner.Entities;

namespace maze_runner.Dungeon.Map;
using Items.Models;
public abstract class Tile
{
    protected readonly Stack<Item> _items = new();
    public IReadOnlyCollection<Item> Items => _items.ToList().AsReadOnly();
    
    public void AddItem(Item item) => _items.Push(item);
    public Item? PopItem() => _items.Count > 0 ? _items.Pop() : null;

    public abstract bool IsWalkable { get; }
    public abstract char GetTileSymbol();
}

public class FloorTile : Tile
{
    public override bool IsWalkable => true;
    public override char GetTileSymbol() => _items.Count > 0 ? _items.Peek().TileSymbol : ' ';
}

public class WallTile : Tile
{
    public override bool IsWalkable => false;
    public override char GetTileSymbol() => '█';
}