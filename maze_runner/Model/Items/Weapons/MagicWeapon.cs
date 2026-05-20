using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Combat;
using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.Weapons;

public abstract class MagicWeapon(int damage) : Weapon
{
    public override int Damage => damage;
    public override int RequiredHands { get; set; } = 1;
    public override int AcousticFootprint => 5;

    public override (int, int) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats)
        => strategy.ExecuteMagic(effectiveDamage, stats);
}