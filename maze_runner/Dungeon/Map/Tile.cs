namespace maze_runner.Dungeon.Map;
using Items.Models;
using Items.Visitors;
using Player;

public abstract class Tile
{
    protected readonly Stack<Item> _items = new();
    public IReadOnlyCollection<Item> Items => _items.ToList().AsReadOnly();
    
    public void AddItem(Item item) => _items.Push(item);
    public Item? PopItem() => _items.Count > 0 ? _items.Pop() : null;

    public void Interact(IItemVisitor visitor)
    {
        if (_items.TryPeek(out var item))
            item.Accept(visitor);
    }

    public abstract bool TryEnter(Player player);
    public abstract char GetTileSymbol();
}

public class FloorTile : Tile
{
    public override bool TryEnter(Player player) => true;
    public override char GetTileSymbol() => _items.Count > 0 ? _items.Peek().TileSymbol : ' ';
}

public class WallTile : Tile
{
    public override bool TryEnter(Player player) => false;
    public override char GetTileSymbol() => '█';
}