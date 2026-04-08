namespace maze_runner.Dungeon.Map;
using Items.Models;
public abstract class Tile
{
    protected readonly Stack<Item> _items = new();
    public IReadOnlyCollection<Item> Items => _items.ToList().AsReadOnly();
    
    public void AddItem(Item item) => _items.Push(item);
    public Item? PopItem() => _items.Count > 0 ? _items.Pop() : null;

    public abstract bool TryEnter(Entities.Player.Player player);
    public abstract char GetTileSymbol();
}

public class FloorTile : Tile
{
    public override bool TryEnter(Entities.Player.Player player) => true;
    public override char GetTileSymbol() => _items.Count > 0 ? _items.Peek().TileSymbol : ' ';
}

public class WallTile : Tile
{
    public override bool TryEnter(Entities.Player.Player player) => false;
    public override char GetTileSymbol() => '█';
}