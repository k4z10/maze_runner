namespace maze_runner.Model.Items.Models;
using Entities;

public abstract class ItemModifier : Item
{
    protected readonly Item _innerItem;
    protected ItemModifier(Item item) =>  _innerItem = item;

    public override char TileSymbol => _innerItem.TileSymbol;
    
    public override IEquippable? GetEquippableFeature() => _innerItem.GetEquippableFeature();
    public override IStorable? GetStorableFeature() => _innerItem.GetStorableFeature();
    public override ICurrency? GetCurrencyFeature() => _innerItem.GetCurrencyFeature();
    public override IWeapon? GetWeaponFeature() => _innerItem.GetWeaponFeature();

    public override Item Clone() => _innerItem.Clone();
}