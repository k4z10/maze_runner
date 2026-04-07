using maze_runner.Player.Components;

namespace maze_runner.Items.Models;
public abstract class UselessItem : Item, IEquippable, IStorable
{
    public abstract UselessItem Clone();
    public int RequiredHands { get; set; }
    public bool TryEquip(Inventory inventory)
    {
        if (inventory.LeftHand != null)
            return false;
        inventory.LeftHand = this;
        inventory.Items.Remove(this);
        return true;
    }
    
    public bool TryUnequip(Inventory inventory)
    {
        if (inventory.LeftHand == null)
            return false;
        inventory.LeftHand = null;
        inventory.Items.Add(this);
        return true;
    }

    public override IEquippable? GetEquippableFeature() => this;
    public override IStorable? GetStorableFeature() => this;
}