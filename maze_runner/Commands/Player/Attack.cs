using maze_runner.Commands.Core;
using maze_runner.Core;
using maze_runner.Core.Logger;
using maze_runner.Entities.Combat;

namespace maze_runner.Commands.Player;

public class Attack(ILevelContext ctx, IAttackStrategy attackType) : ICommand
{
    public void Execute()
    {
        var player = ctx.EntityManager.Player;
        var enemy = ctx.EntityManager.GetAnyEntityExceptPlayerAt(player.Position.Row, player.Position.Col);
        if (enemy == null) return;
        
        var weapon = player.Inventory.RightHand?.GetWeaponFeature();
        int finalDamage, finalDefense;

        if (weapon != null)
        {
            (finalDamage, finalDefense) = weapon.ResolveCombat(weapon.Damage, attackType, player.CurrentStats);
        }
        else
            (finalDamage, finalDefense) = attackType.ExecuteNonWeapon(player.CurrentStats);
        
        enemy.TakeDamage(finalDamage, enemy.BaseDefense);
        ctx.EventBus.Publish(new AttackResolvedEvent(player.Name, enemy.Name, finalDamage));
        if (enemy.IsAlive)
        {
            int damageTaken = player.TakeDamage(enemy.BaseDamage, finalDefense);
            ctx.EventBus.Publish(new AttackResolvedEvent(enemy.Name, player.Name, damageTaken));
        }

        ctx.EntityManager.RemoveDeadEntities();
    }
}