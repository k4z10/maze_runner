using maze_runner.Items;

namespace maze_runner.Entities.Combat;

public interface IAttackStrategy
{
    void ExecuteAttack(Entity target, IWeapon weapon, Attributes attackerStats);
}