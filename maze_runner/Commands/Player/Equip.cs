using maze_runner.Commands.Core;
using maze_runner.Core;
using maze_runner.Core.Logger;

namespace maze_runner.Commands.Player;

public class Equip(ILevelContext ctx) : ICommand
{

    public void Execute()
    {
        var inventory = ctx.EntityManager.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return;
        var index = inventory.CurrentIndex;
        var item = inventory.Items[index];

        if (inventory.TryEquip(item))
        {
            GameEvents.ItemEquipped.Publish(new ItemEquippedEvent(item.Name));
        }
    }
}