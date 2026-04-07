using maze_runner.Items.Models;
namespace maze_runner.Items.Weapons;

public class Sword : Weapon
{
    public override int Damage => 6;
    public override int RequiredHands { get; set; } = 1;

    public override string Name { get; } = "Sword";
    public override string Description { get; } = "Light-weight and effective war weapon.";
    public override char TileSymbol { get; } = '!';
    public override Weapon Clone() => new Sword();
}
