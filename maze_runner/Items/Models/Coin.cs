namespace maze_runner.Items.Models;
public class Coin(int amount) : Item, ICurrency, IStorable
{
    public override string Name => "Coin";
    public override string Description => "The in-game currency";
    public int Amount { get; set; } = amount;
    public override char TileSymbol => 'c';
    public override ICurrency GetCurrencyFeature() => this;
    public override IStorable GetStorableFeature() => this;
}
