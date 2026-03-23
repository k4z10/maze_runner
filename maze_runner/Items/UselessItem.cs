namespace maze_runner;

public abstract class UselessItem : Item
{
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
    public abstract UselessItem Clone();
}

public class Stick : UselessItem
{
    public override string Name => "Stick";
    public override string Description => "The stickiest stick in the whole universe";
    public override char TileSymbol { get; } = '|';
    public override UselessItem Clone() => new Stick();
}

public class Bottle : UselessItem
{
    public override string Name => "Bottle";
    public override string Description => "A bottle without water. Water is gone.";
    public override char TileSymbol { get; } = '⛣';
    public override UselessItem Clone() => new Bottle();
}

public class Feather : UselessItem
{
    public override string Name => "Feather";
    public override string Description => "Light-weight but useless.";
    public override char TileSymbol { get; } = '⟆';
    public override UselessItem Clone() => new Feather();
}