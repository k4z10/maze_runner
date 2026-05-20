namespace maze_runner.Model.Entities;

public abstract class  Entity
{
    private static int _entityId = 10;
    public int Id { get; init; } 
    public (int Row, int Col) Position { get; set; }
    public virtual string Name { get; protected set; }
    public abstract char Symbol { get; }

    public int MaxHealth { get; protected set; }
    public int Health { get; private set; }
    public bool IsAlive => Health > 0;

    public Attributes BaseStats { get; protected set; }
    public virtual Attributes CurrentStats => BaseStats;
    
    public virtual int BaseDamage { get; protected set; }
    public virtual int EffectiveDamage => BaseDamage; 
    
    public virtual int BaseDefense { get; protected set; }
    public virtual int EffectiveDefense => BaseDefense;

    protected Entity(string name, int maxHealth)
    {
        Id = Interlocked.Increment(ref _entityId);
        Name = name;
        MaxHealth = maxHealth;
        Health = maxHealth;
    }

    public int TakeDamage(int incomingDamage)
    {
        if (!IsAlive) return 0;

        int realDamage = Math.Max(0, incomingDamage - EffectiveDefense);
        Health -= realDamage;

        if (Health > 0) return realDamage;
        Health = 0;
        Die();

        return realDamage;
    }
    protected virtual void Die() { }
}

public record struct Attributes(int Strength, int Dexterity, int Resistance, int Stamina, int Luck, int Wisdom);