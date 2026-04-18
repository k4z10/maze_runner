using maze_runner.Commands.Core;
using maze_runner.Core;
namespace maze_runner.Commands.Player;

public class Unequip(ILevelContext ctx) : ICommand
{
    public void Execute()
    {
        var inventory = ctx.EntityManager.Player.Inventory;

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