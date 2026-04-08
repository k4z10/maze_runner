namespace maze_runner.Items.Weapons;
using Models;

public class Knife() : LightWeapon(2)
{
    public override string Name { get; } = "Knife";
    public override string Description { get; } = "Light and handy weapon for every use case.";
    public override char TileSymbol { get; } = '⇀';
    public override Item Clone() => new Knife();
}