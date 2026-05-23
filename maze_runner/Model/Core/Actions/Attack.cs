using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities.Combat;

namespace maze_runner.Model.Core.Actions;

public class Attack(IAttackStrategy attackType) : ICommand
{
    public void Execute(ILevelContext ctx, int attackerId)
    {
        var attacker = ctx.EntityManager.Entities.FirstOrDefault(e => e.Id == attackerId);
        if (attacker == null || !attacker.IsAlive) return;
        
        var enemy = ctx.EntityManager.Entities.FirstOrDefault(e => e.Position == attacker.Position && e.Id != attacker.Id);
        if (enemy == null || !enemy.IsAlive) return;
        
        var weapon = attacker.Inventory?.RightHand?.GetWeaponFeature();
        int finalDamage, finalDefense;
        
        if (weapon != null)
        {
            (finalDamage, finalDefense) = weapon.ResolveCombat(weapon.Damage, attackType, attacker.CurrentStats);
        }
        else
            (finalDamage, finalDefense) = attackType.ExecuteNonWeapon(attacker.CurrentStats);
        
        enemy.TakeDamage(finalDamage);
        ctx.EventBus.Publish(new AttackResolvedEvent(attacker.Name, enemy.Name, finalDamage));
        if (enemy.IsAlive)
        {
            int damageTaken = attacker.TakeDamage(Math.Max(0, enemy.EffectiveDamage - finalDefense));
            ctx.EventBus.Publish(new AttackResolvedEvent(enemy.Name, attacker.Name, damageTaken));
        }

        ctx.EntityManager.RemoveDeadEntities();
    }
}