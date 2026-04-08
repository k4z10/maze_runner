namespace maze_runner.Entities.Mobs;

public class MainBoss() : Entity(100)
{
    public override char Symbol => '&';
    public override int BaseDefense { get; protected set; } = 0;
    public override int BaseDamage { get; protected set; } = 10;
}