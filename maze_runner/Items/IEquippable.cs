namespace maze_runner.Items;
using Player.Components;

public interface IEquippable
{
    int RequiredHands { get; set; }
    bool TryEquip(Inventory inventory);
    bool TryUnequip(Inventory inventory);
}