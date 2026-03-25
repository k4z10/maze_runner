using maze_runner.Core;
using maze_runner.Items.Visitors;

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
        var rightHand = inventory.RightHand;
        var leftHand = inventory.LeftHand;
        var visitor = new FunctionalItemVisitor(
            onWeapon: w => inventory.TryUnequip(w),
            onUseless: u => inventory.TryUnequip(u)
        );

        if (rightHand != null)
        {
            rightHand.Accept(visitor);
            return;
        }

        if (leftHand != null)
        {
            leftHand.Accept(visitor);
        }
    }
}