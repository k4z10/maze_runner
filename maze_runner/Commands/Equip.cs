using maze_runner.Core;
using maze_runner.Items.Visitors;

namespace maze_runner.Commands;
using Core;

public class Equip : ICommand
{
    public string Description { get; } = "Equip an item";

    public bool CanExecute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Bundle.Coins <= 0 && inventory.Bundle.Gold <= 0) return false;
        return true;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        var index = inventory.CurrentIndex;
        var visitor = new FunctionalItemVisitor(
            onWeapon: w => inventory.TryEquip(w),
            onUseless: u => inventory.TryEquip(u)
        );
        
        var item = inventory.Items[index];
        item.Accept(visitor);
    }
}