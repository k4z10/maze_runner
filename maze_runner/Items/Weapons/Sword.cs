using maze_runner.Items.Models;
using maze_runner.Items.Visitors;

namespace maze_runner.Items.Weapons;

public class Sword : Weapon
{
    public override int Damage => 6;
    public override Weight LightOrHeavy => Weight.Light;

    public override string Name { get; } = "Sword";
    public override string Description { get; } = "Light-weight and effective war weapon.";
    public override char TileSymbol { get; } = '!';
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
    public override Weapon Clone() => new Sword();
}
