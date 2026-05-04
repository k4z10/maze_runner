using maze_runner.Core;

namespace maze_runner.Entities.Mobs;

public class Skeleton : Mob
{
    private readonly SkeletonTribe _myTribe;
    private int _rageModifer = 0;

    public override char Symbol => 'S';
    public override int EffectiveDamage => Math.Max(0, BaseDamage + _rageModifer);

    public Skeleton(SkeletonTribe tribe, IEventPublisher ep, IEventSubscriber es) : base("Skeleton", maxHealth: 10, ep: ep, es: es)
    {
        BaseDefense = 5;
        BaseDamage = 1;
        
        _myTribe = tribe;
        _myTribe.Register(this);
    }

    protected override void Die()
    {
        _myTribe.ReportDeath(this);
    }

    public void Enrage()
    {
        _rageModifer += 1;
    }
}