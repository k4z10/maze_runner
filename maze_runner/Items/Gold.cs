namespace maze_runner;
public class Gold(int amount) : Item
{
    public override string Name => "Gold";
    public override string Description => "The in-game currency";
    public int Amount { get; set; } = amount;
    public override char TileSymbol => 'g';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}