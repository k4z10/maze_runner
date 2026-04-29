using maze_runner.Commands.Core;
using maze_runner.Core;
using maze_runner.Core.Logger;

namespace maze_runner.Commands.Player;

public class PickUp(ILevelContext ctx) : ICommand
{
    public void Execute()
    {
        var (x, y) = ctx.EntityManager.Player.Position;
        if (ctx.Map.GetTile(x, y).Items.Count <= 0) return;
        var item = ctx.Map.GetTile(x, y).PopItem();
        if (item == null)
            return;
        
        var storableFeature = item.GetStorableFeature();
        if (storableFeature == null) return;
        
        var currencyFeature = item.GetCurrencyFeature();
        if (currencyFeature != null)
        {
            ctx.EntityManager.Player.Inventory.Coins += currencyFeature.Amount;
        }
        else
        {
            ctx.EntityManager.Player.Inventory.Items.Add(item);
        }
        ctx.EventBus.Publish(new ItemPickedUpEvent(item.Name));
    }
}