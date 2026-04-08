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

    public Item? LeftHand { get; private set; }
    public Item? RightHand { get; private set; }


    public bool TryEquip(Item item)
    {
        var equippable = item.GetEquippableFeature();
        if (equippable == null) return false;
        
        if (equippable.RequiredHands == 2)
        {
            var tmp = RightHand?.GetEquippableFeature();
            if (tmp != null)
            {
                // Swap mechanic for 2-hand weapons
                if (tmp.RequiredHands == 2)
                {
                    TryUnequip(RightHand!);
                }
                else
                {
                    TryUnequip(RightHand!);
                    if (LeftHand != null)
                        TryUnequip(LeftHand);
                }
            }

            LeftHand = item;
            RightHand = item;
            RemoveItemSafe(item);
            return true;
        }
        
        // Weapon always in right hand
        var weapon = item.GetWeaponFeature();
        if (weapon != null)
        {
            if (RightHand == null)
                RightHand = item;
            else
            {
                var tmp = RightHand;
                TryUnequip(tmp);
                RightHand = item;
            }
            RemoveItemSafe(item);
            return true;
        }
        
        // For anything else (first slot available)
        if (RightHand == null)
        {
            RightHand = item;
            RemoveItemSafe(item);
            return true;     
        }
        if (LeftHand != null) return false;
        LeftHand = item;
        RemoveItemSafe(item);
        return true;
    }

    public bool TryUnequip(Item item)
    {
        var equippable = item.GetEquippableFeature();
        if (equippable == null) return false;

        if (equippable.RequiredHands == 2)
        {
            if (LeftHand != item || RightHand != item) return false;

            LeftHand = null;
            RightHand = null;
            Items.Add(item);
            return true;
        }
        
        if (LeftHand == item)
            LeftHand = null;
        if (RightHand == item)
            RightHand = null;
        
        Items.Add(item);
        return true;
    }

    private void RemoveItemSafe(Item item)
    {
        int index = Items.IndexOf(item);
        if (index != -1)
        {
            Items.RemoveAt(index);
            if (CurrentIndex >= Items.Count && CurrentIndex > 0)
            {
                CurrentIndex--;
            }
        }
    }
}