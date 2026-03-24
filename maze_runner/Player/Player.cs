namespace maze_runner.Player;
using Dungeon.Map;
using Components;
public class Player
{
    public (int X, int Y) Position { get; set; }
    public Attributes Attributes { get; private set; }
    public Inventory Inventory = new();
}
public readonly record struct Attributes(int Strength, int Health, int Resistance, int Stamina, int Luck, int Wisdom);


