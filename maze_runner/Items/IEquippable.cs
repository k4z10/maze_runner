using maze_runner.Entities;

namespace maze_runner.Items;

public interface IEquippable
{
    int RequiredHands { get; set; }
    void ApplyStatModifiers(ref Attributes stats);
}