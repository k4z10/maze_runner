using System.Text;

namespace maze_runner;
public abstract class Map(int rows, int cols)
{
    public readonly int Rows = rows;
    public readonly int Cols = cols;
    
    protected readonly Tile[,] Tiles = new Tile[rows, cols];
    public abstract void GenerateMaze();

    private static readonly WallTile OutOfBounds = new WallTile();
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= Rows || y < 0 || y >= Cols)
        {
            return OutOfBounds;
        }
        return Tiles[x, y];
    }

    private string? _mapString;
    public override string ToString()
    {
        if (_mapString != null)
            return _mapString;
        StringBuilder sb = new();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                sb.Append(Tiles[i,j].GetTileSymbol());
            }
            sb.AppendLine();
        }
        _mapString = sb.ToString();
        return _mapString;
    }
}

public class EmptyMaze(int rows, int cols) : Map(rows, cols)
{
    public override void GenerateMaze()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                Tiles[i,j] = new FloorTile();
            }
        }
    }
}

public class FromFileMaze(int rows, int cols, string fileName) : Map(rows, cols)
{
    public override void GenerateMaze()
    {
        using StreamReader reader = new(fileName);
        for (int i = 0; i < Rows; i++)
        {
            var line = reader.ReadLine();
            for (int j = 0; j < Cols; j++)
            {
                if (line == null || line.Length <= j || line[j] == '#' )
                    Tiles[i, j] = new WallTile();
                else
                    Tiles[i, j] = new FloorTile();
            }
        }
    }
}

public class RandomTiles(int rows, int cols) : Map(rows, cols)
{
    public override void GenerateMaze()
    {
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                Tiles[i,j] = Random.Shared.Next(0, 2) == 1 ? new FloorTile() : new WallTile();
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