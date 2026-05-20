using maze_runner.Core;
using maze_runner.Model.Entities.Combat;

namespace maze_runner.Model.Core.Actions;

public class Attack(IAttackStrategy attackType) : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        var enemy = ctx.EntityManager.GetMobAt(player.Position.Row, player.Position.Col);
        if (enemy == null) return;
        
        var weapon = player.Inventory.RightHand?.GetWeaponFeature();
        int finalDamage, finalDefense;
        
        if (weapon != null)
        {
            (finalDamage, finalDefense) = weapon.ResolveCombat(weapon.Damage, attackType, player.CurrentStats);
        }
        else
            (finalDamage, finalDefense) = attackType.ExecuteNonWeapon(player.CurrentStats);
        
        enemy.TakeDamage(finalDamage);
        ctx.EventBus.Publish(new AttackResolvedEvent(player.Name, enemy.Name, finalDamage));
        if (enemy.IsAlive)
        {
            int damageTaken = player.TakeDamage(Math.Max(0, enemy.EffectiveDamage - finalDefense));
            ctx.EventBus.Publish(new AttackResolvedEvent(enemy.Name, player.Name, damageTaken));
        }

        ctx.EntityManager.RemoveDeadEntities();
    }
}