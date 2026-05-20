namespace maze_runner.Model.Items.Models;

public abstract class Item
{
    private static long _id = 0;
    public long Id { get; } = Interlocked.Increment(ref _id);
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract char TileSymbol { get; }

    public virtual IEquippable? GetEquippableFeature() => null;
    public virtual IStorable? GetStorableFeature() => null;
    public virtual ICurrency? GetCurrencyFeature() => null;
    public virtual IWeapon? GetWeaponFeature() => null;

    public override string ToString() => $"({TileSymbol}) {Name}";

    public abstract Item Clone();
}