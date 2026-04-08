namespace maze_runner.Items.Weapons;
using Models;

public class Cubix() : MagicWeapon(4)
{
    public override string Name { get; } = "CubiX";
    public override string Description { get; } = "A magic cube which any Magician shall not underestimate!";
    public override char TileSymbol { get; } = '*';
    public override Item Clone() => new Cubix();
}