using maze_runner.Core;
using maze_runner.Model.Core;

namespace maze_runner.Commands.Player;

public class Equip : ICommand
{

    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var inventory = player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return;
        var index = inventory.CurrentIndex;
        var item = inventory.Items[index];

        if (inventory.TryEquip(item))
        {
            ctx.EventBus.Publish(new ItemEquippedEvent(item.Name));
        }
    }
}