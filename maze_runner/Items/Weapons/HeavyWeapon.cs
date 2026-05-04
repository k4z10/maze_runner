using maze_runner.Entities;
using maze_runner.Entities.Combat;

namespace maze_runner.Items.Weapons;
using Models;

public abstract class HeavyWeapon(int damage) : Weapon
{
    public override int Damage => damage;
    public override int RequiredHands { get; set; } = 2;
    public override int AcousticFootprint => 8;

    public override (int, int) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats)
        => strategy.ExecuteHeavy(effectiveDamage, stats);
}