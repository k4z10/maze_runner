namespace maze_runner.Model.Entities.Combat;

public class NormalAttack : IAttackStrategy
{
    public (int Damage, int Defense) ExecuteHeavy(int baseDamage, Attributes stats)
        => (baseDamage, stats.Strength + stats.Luck);
    public (int Damage, int Defense) ExecuteLight(int baseDamage, Attributes stats)
        => (baseDamage, stats.Dexterity + stats.Luck);
    public (int Damage, int Defense) ExecuteMagic(int baseDamage, Attributes stats)
        => (1, stats.Dexterity + stats.Luck);
    public (int Damage, int Defense) ExecuteNonWeapon(Attributes stats)
        => (0, stats.Dexterity);
}