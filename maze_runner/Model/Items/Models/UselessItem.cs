using maze_runner.Model.Entities;

namespace maze_runner.Model.Items.Models;
public abstract class UselessItem : Item, IEquippable, IStorable
{
    public int RequiredHands { get; set; }

    public void ApplyStatModifiers(ref Attributes stats) { }

    public override IEquippable? GetEquippableFeature() => this;
    public override IStorable? GetStorableFeature() => this;
}