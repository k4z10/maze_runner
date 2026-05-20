using maze_runner.Model.Core;
using maze_runner.Model.Items.Models;

namespace maze_runner.Commands.Player;
using Core;

public class Drop : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var inventory = player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return;
        var currentTile = ctx.Map.GetTile(player.Position.Row, player.Position.Col);
        
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
                var item = inventory.Items[player.Inventory.CurrentIndex];
                inventory.Items.Remove(item);
                currentTile.AddItem(item);
                return;
            }
        }
    }
}