using maze_runner.Commands.Core;
using maze_runner.Core;
namespace maze_runner.Commands.Player;

public class Unequip : ICommand
{
    public bool CanExecute(IGameContext context)
    {
        var rightHand = context.Player.Inventory.RightHand;
        var leftHand = context.Player.Inventory.LeftHand;
        
        return rightHand != null || leftHand != null;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.Player.Inventory;

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