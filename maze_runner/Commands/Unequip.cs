using maze_runner.Core;
namespace maze_runner.Commands;
using Core;
public class Unequip : ICommand
{
    public string Description { get; } = "Unequips item from hand";

    public bool CanExecute(IGameContext context)
    {
        var rightHand = context.Player.Inventory.RightHand;
        var leftHand = context.Player.Inventory.LeftHand;
        
        return rightHand != null || leftHand != null;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        var rightHand = inventory.RightHand?.GetEquippableFeature();
        var leftHand = inventory.LeftHand?.GetEquippableFeature();
        
        if (rightHand != null)
        {
            rightHand.TryUnequip(inventory);
            return;
        }
        leftHand?.TryUnequip(inventory);
    }
}