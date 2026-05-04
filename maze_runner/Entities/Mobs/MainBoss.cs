namespace maze_runner.Entities.Mobs;

public class MainBoss : Entity
{
    public MainBoss(int maxHealth, (int, int) pos) : base("Boss", maxHealth)
    {
        BaseDamage = 10;
        BaseDefense = 0;
        Position = pos;
    }

    public override char Symbol => '&';
}