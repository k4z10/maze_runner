using maze_runner.Core;

namespace maze_runner.Entities.Mobs;

public class Goblin : Mob
{
    private readonly GoblinTribe _myTribe;
    private int _fearModifier = 0;

    public override char Symbol => 'G';
    public override int EffectiveDefense => Math.Max(0, BaseDefense + _fearModifier);

    public Goblin(GoblinTribe tribe, IEventPublisher ep, IEventSubscriber es) : base("Goblin", maxHealth: 20, ep: ep, es: es)
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

    public void Frighten()
    {
        _fearModifier -= 1;
    }
}