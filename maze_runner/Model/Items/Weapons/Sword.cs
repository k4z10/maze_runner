using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.Weapons;

public class Sword() : LightWeapon(5)
{
    public override string Name { get; } = "Sword";
    public override string Description { get; } = "Light-weight and effective war weapon.";
    public override char TileSymbol { get; } = '!';
    public override Weapon Clone() => new Sword();
}
