using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Combat;

namespace maze_runner.Model.Items;

public interface IWeapon
{
    int Damage { get; }
    int RequiredHands { get; set; }
    int AcousticFootprint { get; }
    (int Damage, int Defense) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats);
}