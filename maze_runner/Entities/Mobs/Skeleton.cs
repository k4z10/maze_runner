namespace maze_runner.Entities.Mobs;

class Skeleton(string name = "Skeleton", int maxHealth = 15, int baseDamage = 2, int baseDefense = 2) : Entity(maxHealth), ISkeleton
{
    public override char Symbol => 's';
    public override string Name => name;
    public override int BaseDamage => baseDamage;
    public override int BaseDefense => baseDefense;

    public override ISkeleton? GetSkeleton() => this;
}