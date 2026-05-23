using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities.Player.Components;

namespace maze_runner.Model.Entities.Player;

public class Player(string name, EventBus bus) : Entity(name, maxHealth: 100, bus: bus)
{
    private readonly Inventory _inventory = new();
    public override Inventory? Inventory => _inventory;

    public override Attributes CurrentStats
    {
        get
        {
            var s = BaseStats;
            
            Inventory?.LeftHand?.GetEquippableFeature()?.ApplyStatModifiers(ref s);
            
            if (Inventory != null && Inventory.RightHand != null && Inventory.RightHand != Inventory.LeftHand)
                Inventory.RightHand.GetEquippableFeature()?.ApplyStatModifiers(ref s);

            return s;
        }
    }
    public override char Symbol => '@';
}


