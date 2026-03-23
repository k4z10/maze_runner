namespace maze_runner;

public class PickUpItemVisitor(Player player) : IItemVisitor
{
    public void Visit(Weapon weapon)
    {
        player.Inventory.Items.Add(weapon);
    }
    
    public void Visit(Coin coin)
    {
        player.Inventory.Bundle.Coins += coin.Amount;
    }

    public void Visit(Gold gold)
    {
        player.Inventory.Bundle.Gold += gold.Amount;
    }

    public void Visit(UselessItem uselessItem)
    {
        player.Inventory.Items.Add(uselessItem);
    }
}

