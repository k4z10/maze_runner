namespace maze_runner;
public class Coin : Item
{
    public override string Name => "Coin";
    public override string Description => "The in-game currency";
    public int Amount { get; set; }
    public override char TileSymbol => 'c';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
}
