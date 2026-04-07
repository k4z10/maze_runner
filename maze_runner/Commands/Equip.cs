using maze_runner.Core;
namespace maze_runner.Commands;
using Core;

public class Equip : ICommand
{
    public string Description { get; } = "Equip an item";

    public bool CanExecute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        if (inventory.Items.Count <= 0 && inventory.Coins <= 0 && inventory.Gold <= 0) return false;
        return true;
    }

    public void Execute(IGameContext context)
    {
        var inventory = context.Player.Inventory;
        var index = inventory.CurrentIndex;
        
        var item = inventory.Items[index];
        
        var storableFeature = item.GetEquippableFeature();
        storableFeature?.TryEquip(inventory);
    }
}