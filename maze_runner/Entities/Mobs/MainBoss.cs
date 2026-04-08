namespace maze_runner.Entities.Mobs;

public class MainBoss : Entity
{
    public override int MaxHealth { get; protected set; } = 100;
    public override char Symbol => '&';
}