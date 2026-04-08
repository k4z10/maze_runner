using maze_runner.Entities;
using maze_runner.Entities.Combat;

namespace maze_runner.Items.Models;
public abstract class Weapon : Item, IWeapon, IEquippable, IStorable
{
    public abstract int BaseDamage { get; }
    public abstract int RequiredHands { get; set; }
    public abstract (int, int) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats);
    public virtual void ApplyStatModifiers(ref Attributes stats) { }

    public override IStorable GetStorableFeature() => this;
    public override IEquippable GetEquippableFeature() => this;
    public override IWeapon GetWeaponFeature() => this;
}