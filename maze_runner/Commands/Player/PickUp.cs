using maze_runner.Commands.Core;
using maze_runner.Core;

namespace maze_runner.Commands.Player;

public class PickUp : ICommand
{
    public bool CanExecute(IGameContext ctx)
    {
        var (x, y) = ctx.CurrentLevel.EntityManager.Player.Position;
        return ctx.CurrentLevel.Map.GetTile(x, y).Items.Count > 0;
    }

    public void Execute(IGameContext ctx)
    {
        var (x, y) = ctx.CurrentLevel.EntityManager.Player.Position;
        var item = ctx.CurrentLevel.Map.GetTile(x, y).PopItem();
        if (item == null)
            return;
        
        var storableFeature = item.GetStorableFeature();
        if (storableFeature == null) return;
        
        var currencyFeature = item.GetCurrencyFeature();
        if (currencyFeature != null)
        {
            ctx.CurrentLevel.EntityManager.Player.Inventory.Coins += currencyFeature.Amount;
        }
        else
        {
            ctx.CurrentLevel.EntityManager.Player.Inventory.Items.Add(item);
        }
    }
}