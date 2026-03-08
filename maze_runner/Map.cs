using System.Text;

namespace maze_runner;
public abstract class Map(int sizeX, int sizeY)
{
    protected readonly Tile[,] Tiles = new Tile[sizeX, sizeY];
    public abstract void GenerateMaze();

    private static readonly WallTile OutOfBounds = new WallTile();
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= sizeX || y < 0 || y >= sizeY)
        {
            return OutOfBounds;
        }
        return Tiles[x, y];
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                sb.Append(Tiles[i,j].GetTileSymbol());
            }
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
}

public class EmptyMaze(int sizeX, int sizeY) : Map(sizeX, sizeY)
{
    public override void GenerateMaze()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (j == 0 || i == 0 || i == sizeX - 1 || j == sizeY - 1)
                    Tiles[i,j] = new WallTile();
                else
                    Tiles[i,j] = new FloorTile();
            }
        }
    }
}

public class FromFileMaze(int sizeX, int sizeY, string fileName) : Map(sizeX, sizeY)
{
    public override void GenerateMaze()
    {
        using StreamReader reader = new(fileName);
        for (int i = 0; i < sizeX; i++)
        {
            var line = reader.ReadLine();
            for (int j = 0; j < sizeY; j++)
            {
                if (line == null || line.Length <= j || line[j] == '#' )
                    Tiles[i, j] = new WallTile();
                else
                    Tiles[i, j] = new FloorTile();
            }
        }
    }
}

public abstract class Tile
{
    protected Stack<Item> _items = new();
    public IReadOnlyCollection<Item> Items => _items.ToList().AsReadOnly();

    public abstract bool TryEnter(Player player);
    public abstract char GetTileSymbol();
    
    public void AddItem(Item item) => _items.Push(item);
    public Item PopItem() => _items.Pop();
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