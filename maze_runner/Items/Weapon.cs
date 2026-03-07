namespace maze_runner;
public abstract class Weapon : Item
{
    public abstract int Damage { get; }
    public abstract Weight LightOrHeavy { get; } // Ile rąk potrzeba do trzymania broni
    public enum Weight
    {
        Light,
        Heavy
    }
}

public class Sword : Weapon
{
    public override int Damage => 6;
    public override Weight LightOrHeavy => Weight.Light;

    public override string Name { get; } = "Sword";
    public override string Description { get; } = "Light-weight and effective war weapon.";
    public override char TileSymbol { get; } = '†';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}
