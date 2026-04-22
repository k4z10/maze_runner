using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace maze_runner.Entities;

public abstract class Entity(int maxHealth)
{
    public (int Row, int Col) Position { get; set; }
    public int Health { get; set; } = maxHealth;
    public virtual int MaxHealth { get; protected set; } = maxHealth;
    public Attributes BaseStats { get; protected set; }
    public virtual Attributes CurrentStats => BaseStats;
    public virtual int BaseDamage { get; protected set; }
    public virtual int BaseDefense { get; protected set; }
    public bool IsAlive => Health > 0;
    public abstract char Symbol { get; }
    public virtual string Name { get; set; } = "Entity";

    public int TakeDamage(int damage, int defense)
    {
        int realDamage = Math.Max(0, damage - defense);
        Health -= realDamage;
        if (Health < 0) Health = 0;
        return realDamage;
    }
}

public record struct Attributes(int Strength, int Dexterity, int Resistance, int Stamina, int Luck, int Wisdom);