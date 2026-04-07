using maze_runner.Core;
namespace maze_runner.Commands;
using Core;

public class PickUp : ICommand
{
    public bool CanExecute(IGameContext ctx)
    {
        var (x, y) = ctx.Player.Position;
        return ctx.Map.GetTile(x, y).Items.Count > 0;
    }

    public void Execute(IGameContext ctx)
    {
        var (x, y) = ctx.Player.Position;
        var item = ctx.Map.GetTile(x, y).PopItem();
        if (item == null)
            return;

        // var visitor = new FunctionalItemVisitor(
        //     onWeapon: w => ctx.Player.Inventory.Items.Add(w),
        //     onUseless: u => ctx.Player.Inventory.Items.Add(u),
        //     onCoin: c => ctx.Player.Inventory.Bundle.Coins += c.Amount,
        //     onGold: g => ctx.Player.Inventory.Bundle.Gold += g.Amount);
        //
        // tile.Accept(visitor);
        
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

    public string Description { get; } = "Pick up item form current tile";
}