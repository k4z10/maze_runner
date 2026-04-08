namespace maze_runner.Entities;

public abstract class Entity
{
    public (int Row, int Col) Position { get; set; }
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int Armor { get; protected set; }
    public Attributes Stats { get; protected set; }
    
    public bool IsAlive => Health > 0;
    public abstract int Defense();
    public abstract int AttackPower();

    public void TakeDamage(int damage)
    {
        int realDamage = Math.Max(0, damage - Armor);
        Health -= realDamage;
        if (Health < 0) Health = 0;
    }
}
public readonly record struct Attributes(int Strength, int Health, int Resistance, int Stamina, int Luck, int Wisdom);
