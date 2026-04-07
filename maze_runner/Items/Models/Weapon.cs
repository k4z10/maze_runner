using maze_runner.Player.Components;

namespace maze_runner.Items.Models;
public abstract class Weapon : Item, IWeapon, IEquippable, IStorable
{
    public abstract int Damage { get; }
    public abstract int RequiredHands { get; set; }
    public abstract Weapon Clone();
    public bool TryEquip(Inventory inventory)
    {
        if (this.RequiredHands == 2)
        {
            if (inventory.LeftHand != null ||  inventory.RightHand != null)
                return false;
            inventory.LeftHand = this;
            inventory.RightHand = this;
            inventory.Items.Remove(this);
            return true;
        }
        if (inventory.RightHand != null)
            return false;
        inventory.RightHand = this;
        inventory.Items.Remove(this);
        return true;
    }

    public bool TryUnequip(Inventory inventory)
    {
        if (this.RequiredHands == 2)
        {
            if (inventory.LeftHand != this || inventory.RightHand != this)
                return false;
            inventory.LeftHand = null;
            inventory.RightHand = null;
            inventory.Items.Add(this);
            return true;
        }
        if (inventory.RightHand != this)
            return false;
        inventory.RightHand = null;
        inventory.Items.Add(this);
        return true;
    }

    public override IStorable GetStorableFeature() => this;
    public override IEquippable GetEquippableFeature() => this;
    public override IWeapon? GetWeaponFeature() => this;
}