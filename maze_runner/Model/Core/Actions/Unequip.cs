namespace maze_runner.Model.Core.Actions;

public class Unequip : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Entities.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var inventory = player.Inventory;
        if (inventory == null) return;
        
        if (inventory.LeftHand != null)
        {
            inventory.TryUnequip(inventory.LeftHand);
            return;
        }
        if (inventory.RightHand != null)
        {
            inventory.TryUnequip(inventory.RightHand);
        }
    }
}