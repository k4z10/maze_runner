using maze_runner.Core;
using maze_runner.Items.Visitors;

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
        var tile = ctx.Map.GetTile(x, y).PopItem();
        if (tile == null)
            return;

        var visitor = new FunctionalItemVisitor(
            onWeapon: w => ctx.Player.Inventory.Items.Add(w),
            onUseless: u => ctx.Player.Inventory.Items.Add(u),
            onCoin: c => ctx.Player.Inventory.Bundle.Coins += c.Amount,
            onGold: g => ctx.Player.Inventory.Bundle.Gold += g.Amount);
        
        tile.Accept(visitor);
    }

    public string Description { get; } = "Pick up item form current tile";
}