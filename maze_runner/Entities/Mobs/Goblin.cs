namespace maze_runner.Entities.Mobs;

class Goblin(string name, int maxHealth = 20, int baseDamage = 1, int baseDefense = 0) : Entity(maxHealth), IGoblin
{
    public override char Symbol => 'g';
    public override string Name => name;
    public override int BaseDamage => baseDamage;
    public override int BaseDefense => baseDefense;

    public override IGoblin? GetGoblin() => this;
}