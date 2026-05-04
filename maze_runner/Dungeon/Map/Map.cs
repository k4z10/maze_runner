using System.Data;

namespace maze_runner.Dungeon.Map;
using Entities;
using Entities.Player;
using System.Text;
public class Map(int rows = 0, int cols = 0)
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

    public (int row, int col) GetSpawningPosition() => (0, 0);
    
    public Dictionary<(int Row, int Col), int> CalculateAcousticWave((int Row, int Col) origin, int range)
    {
        var wave = new Dictionary<(int Row, int Col), int>();
        var queue = new Queue<((int Row, int Col) Pos, int Distance)>();

        queue.Enqueue((origin, 0));
        wave[origin] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Distance >= range) continue;

            foreach (var neighbor in GetWalkableNeighbors(current.Pos))
            {
                if (!wave.ContainsKey(neighbor))
                {
                    int newDistance = current.Distance + 1;
                    wave[neighbor] = newDistance;
                    queue.Enqueue((neighbor, newDistance));
                }
            }
        }

        return wave;
    }

    private IEnumerable<(int Row, int Col)> GetWalkableNeighbors((int Row, int Col) origin)
    {
        List<(int dRow, int dCol)> vectors =
        [
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1)
        ];
        foreach (var vector in vectors)
        {
            var nextRow = origin.Row + vector.dRow;
            var nextCol = origin.Col + vector.dCol;
            var targetTile = GetTile(nextRow, nextCol);

            if (targetTile.IsWalkable)
                yield return (nextRow, nextCol);
        }
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