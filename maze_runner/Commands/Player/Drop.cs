using maze_runner.Core;
using maze_runner.Items.Models;
namespace maze_runner.Commands.Player;
using Core;

public class Drop(ILevelContext ctx) : ICommand
{
    public void Execute()
    {
        var inventory = ctx.EntityManager.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return;
        var currentTile = ctx.Map.GetTile(ctx.EntityManager.Player.Position.Row, ctx.EntityManager.Player.Position.Col);
        
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
                var item = inventory.Items[ctx.EntityManager.Player.Inventory.CurrentIndex];
                inventory.Items.Remove(item);
                currentTile.AddItem(item);
                return;
            }
        }
    }
}