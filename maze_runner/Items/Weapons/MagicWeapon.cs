namespace maze_runner.Items.Weapons;
using Models;

public abstract class MagicWeapon(int damage) : Weapon
{
    public override int Damage { get; } = damage;
    public override int RequiredHands { get; set; } = 1;
}