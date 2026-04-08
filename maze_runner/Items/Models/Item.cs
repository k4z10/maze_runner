namespace maze_runner.Items.Models;

public abstract class Item
{
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