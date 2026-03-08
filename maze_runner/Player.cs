using System.Runtime.CompilerServices;
using System.Text;

namespace maze_runner;
public class Player
{
    public (int X, int Y) Position { get; private set;}
    public Attributes Attributes { get; private set; }
    public Inventory Inventory = new();
    
    public void Move(int dx, int dy, Map map)
    {
        int newX = Position.X + dx;
        int newY = Position.Y + dy;
        if (map.GetTile(newX, newY).TryEnter(this))
        {
            Position = (newX, newY);
        }
    }
}
public readonly record struct Attributes(int Strength, int Health, int Resistance, int Stamina, int Luck, int Wisdom);


