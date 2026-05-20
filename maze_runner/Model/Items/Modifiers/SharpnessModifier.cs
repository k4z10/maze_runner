using maze_runner.Model.Entities;
using maze_runner.Model.Entities.Combat;
using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.Modifiers;

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
        public int AcousticFootprint => inner.AcousticFootprint; 
        public (int Damage, int Defense) ResolveCombat(int effectiveDamage, IAttackStrategy strategy, Attributes stats)
            => inner.ResolveCombat(effectiveDamage, strategy, stats);
        public void ApplyStatModifiers(Attributes stats) { }
    }
}