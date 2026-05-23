using System.Data;
using System.Reflection.Metadata;
using maze_runner.Model.Core;
using maze_runner.Model.Core.Events;

namespace maze_runner.Model.Entities.Mobs;

public class Goblin : Entity
{
    private readonly GoblinTribe _myTribe;
    private int _fearModifier = 0;

    public override char Symbol => 'G';
    public override int EffectiveDefense => Math.Max(0, BaseDefense + _fearModifier);

    public Goblin(GoblinTribe tribe, EventBus bus) : base("Goblin", maxHealth: 20, bus: bus)
    {
        BaseDefense = 5;
        BaseDamage = 1;
        
        _myTribe = tribe;
        _myTribe.Register(this);
    }
    public void Frighten() => _fearModifier -= 1;

    public override void UpdateState(ILevelContext ctx, double dt)
    {
        base.UpdateState(ctx, dt);
        
        if (Random.Shared.NextDouble() >= 0.03) return;
        
        var (dRow, dCol) = (Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2)); 
        var (tRow, tCol) = (Position.Row + dRow, Position.Col + dCol);
        
        if (!ctx.Map.GetTile(tRow, tCol).IsWalkable) return;
        
        Position = (tRow, tCol);
    }

    public override void Dispose()
    {
        base.Dispose();
        _myTribe.ReportDeath(this);
    }

}