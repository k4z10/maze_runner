using maze_runner.Entities.Combat;

namespace maze_runner.Items;
using Entities;

public interface IWeapon
{
    int BaseDamage { get; }
    int RequiredHands { get; set; }
    (int Damage, int Defense) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats);
}