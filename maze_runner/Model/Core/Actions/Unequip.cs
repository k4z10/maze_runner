using maze_runner.Core;
using maze_runner.Model.Core;

namespace maze_runner.Commands.Player;

public class Unequip : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var inventory = player.Inventory;
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