using maze_runner.Entities;
using maze_runner.Entities.Player.Components;

namespace maze_runner.Items.Models;
public abstract class Weapon : Item, IWeapon, IEquippable, IStorable
{
    public abstract int Damage { get; }
    public abstract int RequiredHands { get; set; }

    public override IStorable GetStorableFeature() => this;
    public override IEquippable GetEquippableFeature() => this;
    public override IWeapon GetWeaponFeature() => this;
}