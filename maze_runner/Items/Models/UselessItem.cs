using maze_runner.Entities;
using maze_runner.Entities.Player.Components;

namespace maze_runner.Items.Models;
public abstract class UselessItem : Item, IEquippable, IStorable
{
    public int RequiredHands { get; set; }

    public void ApplyStatModifiers(ref Attributes stats) { }

    public override IEquippable? GetEquippableFeature() => this;
    public override IStorable? GetStorableFeature() => this;
}