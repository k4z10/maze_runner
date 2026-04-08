using maze_runner.Entities;
using maze_runner.Entities.Player.Components;

namespace maze_runner.Items;

public interface IEquippable
{
    int RequiredHands { get; set; }
}