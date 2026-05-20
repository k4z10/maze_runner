namespace maze_runner.Model.Entities.Combat;

public interface IAttackStrategy
{
    (int Damage, int Defense) ExecuteHeavy(int baseDamage, Attributes stats);
    (int Damage, int Defense) ExecuteLight(int baseDamage, Attributes stats);
    (int Damage, int Defense) ExecuteMagic(int baseDamage, Attributes stats);
    (int Damage, int Defense) ExecuteNonWeapon(Attributes stats);
}