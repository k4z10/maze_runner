namespace maze_runner.Entities.Combat;

public class MagicAttack : IAttackStrategy
{
    public (int Damage, int Defense) ExecuteHeavy(int baseDamage, Attributes stats)
        => (1, stats.Luck);
    public (int Damage, int Defense) ExecuteLight(int baseDamage, Attributes stats)
        => (1, stats.Luck);

    public (int Damage, int Defense) ExecuteMagic(int baseDamage, Attributes stats)
        => (baseDamage, stats.Wisdom * 2);
    public (int Damage, int Defense) ExecuteNonWeapon(Attributes stats)
        => (0,  stats.Luck);
}