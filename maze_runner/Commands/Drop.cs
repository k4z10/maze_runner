using System.Runtime.CompilerServices;
using maze_runner.Core;
using maze_runner.Items.Models;
using maze_runner.Items.Visitors;

namespace maze_runner.Commands;
using Core;

public class Drop : ICommand
{
    public string Description { get; } = "Drop item from inventory";

    public bool CanExecute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Bundle.Coins <= 0 && inventory.Bundle.Gold <= 0) return false;
        return true;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        var currentTile = context.Map.GetTile(context.Player.Position.Row, context.Player.Position.Col);
        var visitor = new FunctionalItemVisitor(
            onWeapon: w =>
            {
                inventory.Items.Remove(w);
                currentTile.AddItem(w);
            },
            onUseless: u =>
            {
                inventory.Items.Remove(u);
                currentTile.AddItem(u);
            });
        
        if (inventory.CurrentIndex == -1)
        {
            int amount = 1;
            var coin = new Coin(amount);
            inventory.Bundle.Coins -= amount;
            currentTile.AddItem(coin);
        }
        else if (inventory.CurrentIndex == -2)
        {
            int amount = 1;
            var gold = new Gold(amount);
            inventory.Bundle.Gold -= amount;
            currentTile.AddItem(gold);
        }
        
        var item = inventory.Items[context.Player.Inventory.CurrentIndex];
        item.Accept(visitor);
    }
}