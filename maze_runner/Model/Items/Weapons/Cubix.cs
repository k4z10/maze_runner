using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.Weapons;

public class Cubix() : MagicWeapon(4)
{
    public override string Name { get; } = "CubiX";
    public override string Description { get; } = "A magic cube which any Magician shall not underestimate!";
    public override char TileSymbol { get; } = '*';
    public override Item Clone() => new Cubix();
}