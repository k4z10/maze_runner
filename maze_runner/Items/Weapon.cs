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
    public override char TileSymbol { get; } = '!';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}

public class LongSword : Weapon
{
    public override int Damage => 10;
    public override Weight LightOrHeavy => Weight.Heavy;
    public override string Name { get; } = "Long Sword";
    public override string Description { get; } = "Heavy, long sword for the biggest targets.";
    public override char TileSymbol { get; } = '⸸';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}

public class Knife : Weapon
{
    public override int Damage => 4;
    public override Weight LightOrHeavy => Weight.Light;
    public override string Name { get; } = "Knife";
    public override string Description { get; } = "Light and handy weapon for every use case.";
    public override char TileSymbol { get; } = '⇀';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}