using maze_runner.Core;
using maze_runner.Items.Models;
namespace maze_runner.Commands.Player;
using Core;

public class Drop : ICommand
{
    public bool CanExecute(IGameContext context)
    {
        var inventory = context.EntityManager.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return false;
        return true;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.EntityManager.Player.Inventory;
        var currentTile = context.CurrentMap.GetTile(context.EntityManager.Player.Position.Row, context.EntityManager.Player.Position.Col);
        
        switch (inventory.CurrentIndex)
        {
            case -1:
            {
                int amount = 1;
                var coin = new Coin(amount);
                inventory.Coins -= amount;
                currentTile.AddItem(coin);
                return;
            }
            case -2:
            {
                int amount = 1;
                var gold = new Gold(amount);
                inventory.Gold -= amount;
                currentTile.AddItem(gold);
                return;
            }
            default:
            {
                var item = inventory.Items[context.EntityManager.Player.Inventory.CurrentIndex];
                inventory.Items.Remove(item);
                currentTile.AddItem(item);
                return;
            }
        }
    }
}