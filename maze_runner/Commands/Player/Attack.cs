using maze_runner.Commands.Core;
using maze_runner.Core;
using maze_runner.Entities.Combat;
using maze_runner.Items;

namespace maze_runner.Commands.Player;

public class Attack(IAttackStrategy attackType) : ICommand
{
    public bool CanExecute(IGameContext context) => true;
    
    public void Execute(IGameContext context)
    {
        var player = context.Player;
        var enemy = context.CurrentMap.GetEntity(player);
        if (enemy == null) return;
        
        var weapon = player.Inventory.RightHand?.GetWeaponFeature();
        int finalDamage, finalDefense;

        if (weapon != null)
            (finalDamage, finalDefense) = weapon.ResolveCombat(weapon.Damage, attackType, player.CurrentStats);
        else
            (finalDamage, finalDefense) = attackType.ExecuteNonWeapon(player.CurrentStats);
        
        enemy.TakeDamage(finalDamage, enemy.BaseDefense);
        if (!enemy.IsAlive)
        {
            context.CurrentMap.RemoveEntity(enemy);
            return;
        }
        player.TakeDamage(enemy.BaseDamage, finalDefense);
    }
}