namespace maze_runner;

public class DropItemVisitor(Player player, Tile currentTile) : IItemVisitor
{
    public void Visit(Weapon weapon)
    {
        player.Inventory.TryUnequip(weapon);
        player.Inventory.Items.Remove(weapon);
        currentTile.AddItem(weapon);
    }

    public void Visit(UselessItem uselessItem)
    {
        player.Inventory.TryUnequip(uselessItem);
        player.Inventory.Items.Remove(uselessItem);
        currentTile.AddItem(uselessItem);
    }

    public void Visit(Coin coin)
    {
        player.Inventory.Bundle.Coins--;
        var newCoin = new Coin(1);
        currentTile.AddItem(newCoin);
    }

    public void Visit(Gold gold)
    {
        player.Inventory.Bundle.Gold--;
        var newGold = new Gold(1);
        currentTile.AddItem(newGold);
    }
}