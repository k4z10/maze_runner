using maze_runner.Model.Core.Events;

namespace maze_runner.Model.Core.Actions;

public class Equip : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Entities.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var inventory = player.Inventory;
        if (inventory == null || inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return;
        
        var item = inventory.Items.FirstOrDefault();
        if (item == null) return;

        if (inventory.TryEquip(item))
        {
            ctx.EventBus.Publish(new ItemEquippedEvent(player.Name, item.Name));
        }
    }
}