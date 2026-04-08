using maze_runner.Entities.Player.Components;

namespace maze_runner.Entities.Player;

public class Player : Entity
{
    public readonly Inventory Inventory = new();
    public override int Defense()
    {
        throw new NotImplementedException();
    }

    public override int AttackPower()
    {
        var weapon = Inventory.RightHand?.GetWeaponFeature();
        if (weapon == null) return 0;

        return weapon.Damage;
    }
}


