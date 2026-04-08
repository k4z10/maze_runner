using maze_runner.Entities;
using maze_runner.Items.Models;

namespace maze_runner.Items.Modifiers;

public class KnowledgeModifier(Item item) : ItemModifier(item)
{
    public override string Name => $"{_innerItem.Name} (Knowledge)";
    public override string Description => $"{_innerItem.Description}";

    public override IEquippable? GetEquippableFeature()
    {
        var baseEquippable = _innerItem.GetEquippableFeature();
        return baseEquippable == null ? null : new KnowledgeEquippableDecorator(baseEquippable);
    }

    private class KnowledgeEquippableDecorator(IEquippable inner) : IEquippable
    {
        public int RequiredHands { get => inner.RequiredHands; set => inner.RequiredHands = value; }

        public void ApplyStatModifiers(ref Attributes stats)
        {
            inner.ApplyStatModifiers(ref stats);
            stats.Wisdom += 6;
        }
    }
}
