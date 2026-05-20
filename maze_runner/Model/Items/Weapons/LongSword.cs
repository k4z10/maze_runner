using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.Weapons;

public class LongSword() : HeavyWeapon(10)
{
    public override string Name { get; } = "Long Sword";
    public override string Description { get; } = "Heavy, long sword for the biggest targets.";
    public override char TileSymbol { get; } = '⸸';
    public override Item Clone() => new LongSword();
}