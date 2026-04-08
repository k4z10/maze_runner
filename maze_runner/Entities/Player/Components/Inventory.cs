using System.Collections.ObjectModel;
using maze_runner.Items;
using maze_runner.Items.Models;

namespace maze_runner.Entities.Player.Components;

public class Inventory
{
    public ObservableCollection<Item> Items = new();
    public int CurrentIndex = 0;
    
    public int Gold { get; set; }
    public int Coins { get; set; }

    public Item? LeftHand = null;
    public Item? RightHand = null;

    public bool TryEquip(Item item)
    {
        var equippable = item.GetEquippableFeature();
        if (equippable == null) return false;

        if (equippable.RequiredHands == 2)
        {
            if (LeftHand != null || RightHand != null) return false;

            LeftHand = item;
            RightHand = item;
            Items.Remove(item);
            return true;
        }
        if (RightHand != null) return false;
        
        RightHand = item;
        Items.Remove(item);
        return true;
    }
    
    public bool TryUnequip(Item item)
    {
        var equippable = item.GetEquippableFeature();
        if (equippable == null) return false;
        
        if (equippable.RequiredHands == 2)
        {
            if (LeftHand != item || RightHand != item)
                return false;
            LeftHand = null;
            RightHand = null;
            Items.Add(item);
            return true;
        }
        if (RightHand != item)
            return false;
        
        RightHand = null;
        Items.Add(item);
        return true;
    }
}