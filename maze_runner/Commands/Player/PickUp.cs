using maze_runner.Commands.Core;
using maze_runner.Core;

namespace maze_runner.Commands.Player;

public class PickUp : ICommand
{
    public bool CanExecute(IGameContext ctx)
    {
        var (x, y) = ctx.Player.Position;
        return ctx.CurrentMap.GetTile(x, y).Items.Count > 0;
    }

    public void Execute(IGameContext ctx)
    {
        var (x, y) = ctx.Player.Position;
        var item = ctx.CurrentMap.GetTile(x, y).PopItem();
        if (item == null)
            return;
        
        var storableFeature = item.GetStorableFeature();
        if (storableFeature == null) return;
        
        var currencyFeature = item.GetCurrencyFeature();
        if (currencyFeature != null)
        {
            ctx.Player.Inventory.Coins += currencyFeature.Amount;
        }
        else
        {
            ctx.Player.Inventory.Items.Add(item);
        }
    }
}