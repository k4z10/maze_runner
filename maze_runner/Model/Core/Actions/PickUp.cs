using maze_runner.Core;
using maze_runner.Model.Core;

namespace maze_runner.Commands.Player;

public class PickUp : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var (row, col) = player.Position;
        if (ctx.Map.GetTile(row, col).Items.Count <= 0) return;
        var item = ctx.Map.GetTile(row, col).PopItem();
        if (item == null) return;

        var storableFeature = item.GetStorableFeature();
        if (storableFeature == null) return;
        
        var currencyFeature = item.GetCurrencyFeature();
        if (currencyFeature != null)
        {
            player.Inventory.Coins += currencyFeature.Amount;
        }
        else
        {
            player.Inventory.Items.Add(item);
        }

        var weaponFeature = item.GetWeaponFeature();
        if (weaponFeature != null)
        {
            var wave = ctx.Map.CalculateAcousticWave((row, col), weaponFeature.AcousticFootprint);
            ctx.EventBus.Publish(new AcousticWavePropagate(wave, (row, col), item.Name));
        }
        
        ctx.EventBus.Publish(new ItemPickedUpEvent(item.Name));
    }
}