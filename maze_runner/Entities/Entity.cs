namespace maze_runner.Entities;

public abstract class Entity
{
    public (int Row, int Col) Position { get; set; }
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int Armor { get; protected set; }
    public Attributes BaseStats { get; protected set; }
    public virtual Attributes CurrentStats => BaseStats;
    public bool IsAlive => Health > 0;

    public void TakeDamage(int damage)
    {
        int realDamage = Math.Max(0, damage - Armor);
        Health -= realDamage;
        if (Health < 0) Health = 0;
    }
}

public record struct Attributes(int Strength, int Dexterity, int Resistance, int Stamina, int Luck, int Wisdom);