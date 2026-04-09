using maze_runner.Commands.Core;
using maze_runner.Core;
using maze_runner.Core.Logger;

namespace maze_runner.Commands.Player;

public class Equip : ICommand
{
    public bool CanExecute(IGameContext context)
    {
        var inventory = context.CurrentLevel.EntityManager.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return false;
        return true;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.CurrentLevel.EntityManager.Player.Inventory;
        var index = inventory.CurrentIndex;
        var item = inventory.Items[index];

        if (inventory.TryEquip(item))
        {
            context.EventBus.ItemEquipped.Publish(new ItemEquippedEvent(item.Name));
        }
    }
}