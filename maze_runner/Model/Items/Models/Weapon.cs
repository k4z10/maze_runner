using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Combat;

namespace maze_runner.Model.Items.Models;
public abstract class Weapon : Item, IWeapon, IEquippable, IStorable
{
    public abstract int Damage { get; }
    public abstract int RequiredHands { get; set; }
    public abstract int AcousticFootprint { get; }
    public abstract (int, int) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats);
    public virtual void ApplyStatModifiers(ref Attributes stats) { }

    public override IStorable GetStorableFeature() => this;
    public override IEquippable GetEquippableFeature() => this;
    public override IWeapon GetWeaponFeature() => this;
}