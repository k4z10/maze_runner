namespace maze_runner.Items.Modifiers;
using Models;

public class SharpnessModifier(Item item) : ItemModifier(item)
{
    public override string Name => $"{_innerItem.Name} (Sharpness)";
    public override string Description => $"{_innerItem.Description}";

    public override IWeapon? GetWeaponFeature()
    {
        var baseFeature = _innerItem.GetWeaponFeature();
        return baseFeature == null ? null : new SharpnessWeaponDecorator(baseFeature);
    }

    private class SharpnessWeaponDecorator(IWeapon inner) : IWeapon
    {
        public int Damage => inner.Damage + 5;
        public int RequiredHands { get => inner.RequiredHands; set => inner.RequiredHands = value; }
    }
}