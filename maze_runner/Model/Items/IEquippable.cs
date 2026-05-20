using maze_runner.Model.Entities;

namespace maze_runner.Model.Items;

public interface IEquippable
{
    int RequiredHands { get; set; }
    void ApplyStatModifiers(ref Attributes stats);
}