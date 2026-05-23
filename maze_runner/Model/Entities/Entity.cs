using maze_runner.Model.Core;
using maze_runner.Model.Core.Events;
using maze_runner.Model.Entities.Player.Components;

namespace maze_runner.Model.Entities;

public abstract class  Entity : IDisposable
{
    private static int _entityId = 10;
    public int Id { get; init; } 
    public (int Row, int Col) Position { get; set; }
    public string Name { get; protected set; }
    public abstract char Symbol { get; }

    public int MaxHealth { get; protected set; }
    public int Health { get; private set; }
    public bool IsAlive => Health > 0;

    public Attributes BaseStats { get; protected set; }
    public virtual Attributes CurrentStats => BaseStats;

    protected int BaseDamage { get; init; }
    public virtual int EffectiveDamage => BaseDamage;
    protected int BaseDefense { get; init; }
    public virtual int EffectiveDefense => BaseDefense;
    
    public virtual Inventory? Inventory => null;
    
    private readonly EventBus _bus; 
    private readonly Action<AcousticWavePropagate> _acousticHandler;

    protected Entity(string name, int maxHealth, EventBus bus)
    {
        Id = Interlocked.Increment(ref _entityId);
        Name = name;
        MaxHealth = maxHealth;
        Health = maxHealth;
        
        _bus = bus;
        _acousticHandler = HandleAcousticWave;
        _bus.Subscribe(_acousticHandler);
    }

    public int TakeDamage(int incomingDamage)
    {
        int realDamage = Math.Max(0, incomingDamage - EffectiveDefense);
        Health -= realDamage;

        if (Health > 0) return realDamage;
        Health = 0;

        return realDamage;
    }
    
    private void HandleAcousticWave(AcousticWavePropagate e)
    {
        if (e.Wave.TryGetValue(this.Position, out var distance))
        {
            _bus.Publish(new SoundRegisteredEvent(this, e.Origin, distance));
        }
    }
    
    public virtual void UpdateState(ILevelContext ctx, double dt) { }

    public virtual void Dispose()
    {
        _bus.Unsubscribe(_acousticHandler);
    }
}

public record struct Attributes(int Strength, int Dexterity, int Resistance, int Stamina, int Luck, int Wisdom);