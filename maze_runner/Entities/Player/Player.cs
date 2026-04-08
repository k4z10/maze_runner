using maze_runner.Entities.Player.Components;

namespace maze_runner.Entities.Player;

public class Player() : Entity(100)
{
    public readonly Inventory Inventory = new();

    public override Attributes CurrentStats
    {
        get
        {
            var s = BaseStats;
            
            Inventory.LeftHand?.GetEquippableFeature()?.ApplyStatModifiers(ref s);
            
            if (Inventory.RightHand != null && Inventory.RightHand != Inventory.LeftHand)
                Inventory.RightHand.GetEquippableFeature()?.ApplyStatModifiers(ref s);

            return s;
        }
    }
    public override char Symbol => '@';
}


