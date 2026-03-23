namespace maze_runner.Dungeon.Map;
using System.Text;
public class Map(int rows, int cols)
{
    public readonly int Rows = rows;
    public readonly int Cols = cols;
    
    private readonly Tile[,] _tiles = new Tile[rows, cols];
    

    private static readonly WallTile OutOfBounds = new WallTile();
    public Tile GetTile(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols)
            return OutOfBounds;
        return _tiles[row, col];
    }

    public bool TrySetTile(int row, int col, Tile tile)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols) return false;
        _tiles[row, col] = tile;
        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
                sb.Append(_tiles[i,j].GetTileSymbol());
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public struct Room(int x, int y, int width, int height)
{
    public int X = x, Y = y, Width = width, Height = height;

    public readonly int CenterX => X + Width / 2;
    public readonly int CenterY =>  Y + Height / 2;

    public readonly bool Intersects(Room other)
    {
        return X <= other.X + other.Width + 1 &&
               Y <= other.Y + other.Height + 1 &&
               X + Width + 1 >= other.X &&
               Y + Height + 1 >= other.Y;
    }
}

// public class EmptyMap(int rows, int cols) : Map(rows, cols)
// {
//     public override void GenerateMaze()
//     {
//         for (int i = 0; i < Rows; i++)
//             for (int j = 0; j < Cols; j++)
//                 Tiles[i, j] = new FloorTile();
//     }
// }
//
// public class FullMap(int rows, int cols) : Map(rows, cols)
// {
//     public override void GenerateMaze()
//     { 
//         for (int i = 0; i < Rows; i++)
//             for (int j = 0; j < Cols; j++)
//                 Tiles[i, j] = new FloorTile();
//     }
// }
//
// public class FromFileMap(int rows, int cols, string fileName) : Map(rows, cols)
// {
//     public override void GenerateMaze()
//     {
//         using StreamReader reader = new(fileName);
//         for (int i = 0; i < Rows; i++)
//         {
//             var line = reader.ReadLine();
//             for (int j = 0; j < Cols; j++)
//             {
//                 if (line == null || line.Length <= j || line[j] == '#' )
//                     Tiles[i, j] = new WallTile();
//                 else
//                     Tiles[i, j] = new FloorTile();
//             }
//         }
//     }
// }