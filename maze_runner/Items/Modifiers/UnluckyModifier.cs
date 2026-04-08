using maze_runner.Entities;
using maze_runner.Entities.Player;
using maze_runner.Items.Models;

namespace maze_runner.Items.Modifiers;

public class UnluckyModifier(Item item) : ItemModifier(item)
{
    public override string Name => $"{_innerItem.Name} (Unlucky)";
    public override string Description => $"{_innerItem.Description}";


    private class UnluckyEquippableDecorator(IEquippable equippable) : IEquippable
    {
        public int RequiredHands { get => equippable.RequiredHands; set => equippable.RequiredHands = value; }

        public void ApplyStatModifiers(Player player)
        {
            
        }
    }
}