namespace maze_runner.Items.Weapons;
using Models;

public abstract class LightWeapon(int damage) : Weapon
{
    public override int Damage => damage;
    public override int RequiredHands { get; set; } = 1;
}