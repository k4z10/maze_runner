using maze_runner.Model.Core.Events;

namespace maze_runner.Model.Entities.Hostile;

public class Skeleton : Entity
{
    private readonly SkeletonTribe _myTribe;
    private int _rageModifer = 0;

    public override char Symbol { get; set; } = 'S';
    public override int EffectiveDamage => Math.Max(0, BaseDamage + _rageModifer);

    public Skeleton(SkeletonTribe tribe, EventBus bus) : base("Skeleton", maxHealth: 10, bus: bus)
    {
        BaseDefense = 5;
        BaseDamage = 1;
        
        _myTribe = tribe;
        _myTribe.Register(this);
    }

    public override void Dispose()
    {
        base.Dispose();
        _myTribe.ReportDeath(this);
    }

    public void Enrage() => _rageModifer += 1;
}