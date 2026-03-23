namespace maze_runner.Items.Weapons;
using Models;
public class Knife : Weapon
{
    public override int Damage => 4;
    public override Weight LightOrHeavy => Weight.Light;
    public override string Name { get; } = "Knife";
    public override string Description { get; } = "Light and handy weapon for every use case.";
    public override char TileSymbol { get; } = '⇀';
    public override Weapon Clone() => new Knife();
}