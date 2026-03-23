namespace maze_runner.Items.Visitors;
using Models;
using Player;

public class EquipItemVisitor(Player player) : IItemVisitor
{
    public void Visit(Weapon weapon)
    {
        player.Inventory.TryEquip(weapon);
    }

    public void Visit(Coin coin)
    {
    }

    public void Visit(Gold gold)
    {
    }

    public void Visit(UselessItem uselessItem)
    {
        player.Inventory.TryEquip(uselessItem);
    }  
}
