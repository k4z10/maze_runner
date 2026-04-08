using maze_runner.Entities;
using maze_runner.Entities.Combat;

namespace maze_runner.Items.Weapons;
using Models;

public abstract class LightWeapon(int damage) : Weapon
{
    public override int BaseDamage => damage;
    public override int RequiredHands { get; set; } = 1;
    public override (int, int) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats)
        => strategy.ExecuteLight(effectiveDamage, stats);
}