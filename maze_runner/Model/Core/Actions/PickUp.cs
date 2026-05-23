using maze_runner.Model.Core.Events;

namespace maze_runner.Model.Core.Actions;

public class PickUp : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Entities.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        var (row, col) = player.Position;
        if (ctx.Map.GetTile(row, col).Items.Count <= 0) return;
        
        var item = ctx.Map.GetTile(row, col).PopItem();
        if (item == null) return;
        
        var inventory = player.Inventory;
        if (inventory == null) return;

        var storableFeature = item.GetStorableFeature();
        if (storableFeature == null) return;
        
        var currencyFeature = item.GetCurrencyFeature();
        if (currencyFeature != null)
        {
            inventory.Coins += currencyFeature.Amount;
        }
        else
        {
            inventory.Items.Add(item);
        }

        var weaponFeature = item.GetWeaponFeature();
        if (weaponFeature != null)
        {
            var wave = ctx.Map.CalculateAcousticWave((row, col), weaponFeature.AcousticFootprint);
            ctx.EventBus.Publish(new AcousticWavePropagate(wave, (row, col), item.Name));
        }
        
        ctx.EventBus.Publish(new ItemPickedUpEvent(player.Name, item.Name));
    }
}