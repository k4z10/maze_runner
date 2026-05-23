namespace maze_runner.Model.Entities.Combat;

public class StealthAttack : IAttackStrategy
{
    public (int Damage, int Defense) ExecuteHeavy(int baseDamage, Attributes stats)
        => (baseDamage / 2, stats.Strength);
    public (int Damage, int Defense) ExecuteLight(int baseDamage, Attributes stats)
        => (baseDamage * 2, stats.Dexterity);
    public (int Damage, int Defense) ExecuteMagic(int baseDamage, Attributes stats)
        => (1, 0);
    public (int Damage, int Defense) ExecuteNonWeapon(Attributes stats)
        => (0, 0);
}