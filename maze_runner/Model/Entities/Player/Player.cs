using maze_runner.Model.Entities.Player.Components;

namespace maze_runner.Model.Entities.Player;

public class Player : Entity
{
    public readonly Inventory Inventory = new();

    public Player(string name) : base(name, maxHealth: 100)
    {
        Name = name;
    }

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


