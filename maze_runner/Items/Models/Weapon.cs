namespace maze_runner.Items.Models;
using Visitors;
public abstract class Weapon : Item
{
    public abstract int Damage { get; }
    public abstract Weight LightOrHeavy { get; } // Ile rąk potrzeba do trzymania broni
    public abstract Weapon Clone();
    public enum Weight
    {
        Light,
        Heavy
    }
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}