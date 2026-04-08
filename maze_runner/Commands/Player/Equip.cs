using maze_runner.Commands.Core;
using maze_runner.Core;

namespace maze_runner.Commands.Player;

public class Equip : ICommand
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
        var index = inventory.CurrentIndex;
        var item = inventory.Items[index];
        
        inventory.TryEquip(item); 
    }
}