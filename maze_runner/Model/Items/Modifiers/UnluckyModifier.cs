using maze_runner.Model.Entities;
using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.Modifiers;

public class UnluckyModifier(Item item) : ItemModifier(item)
{
    public override string Name => $"{_innerItem.Name} (Unlucky)";
    public override string Description => $"{_innerItem.Description}";

    public override IEquippable? GetEquippableFeature()
    {
        var baseEquippable = _innerItem.GetEquippableFeature();
        return baseEquippable == null ? null : new UnluckyEquippableDecorator(baseEquippable);
    }

    private class UnluckyEquippableDecorator(IEquippable inner) : IEquippable
    {
        public int RequiredHands { get => inner.RequiredHands; set => inner.RequiredHands = value; }

        public void ApplyStatModifiers(ref Attributes stats)
        {
            inner.ApplyStatModifiers(ref stats);
            stats.Luck -= 5;
        }
    }
}