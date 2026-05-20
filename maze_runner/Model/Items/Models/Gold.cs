namespace maze_runner.Model.Items.Models;
public class Gold(int amount) : Item, ICurrency, IStorable
{
    public override string Name => "Gold";
    public override string Description => "The in-game currency";
    public int Amount { get; set; } = amount;
    public override char TileSymbol => 'g';
    public override ICurrency GetCurrencyFeature() => this;
    public override IStorable GetStorableFeature() => this;
    public override Item Clone() => new Gold(Amount);
}