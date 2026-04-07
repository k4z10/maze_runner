namespace maze_runner.Items.Weapons;
using Models;
public class LongSword : Weapon, IEquippable
{
    public override int Damage => 10;
    public override int RequiredHands { get; set; } = 2;
    public override string Name { get; } = "Long Sword";
    public override string Description { get; } = "Heavy, long sword for the biggest targets.";
    public override char TileSymbol { get; } = '⸸';
    public override Weapon Clone() => new LongSword();
}