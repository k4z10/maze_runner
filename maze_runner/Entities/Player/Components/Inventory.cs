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
            if (LeftHand != null || RightHand != null) return false;

            LeftHand = item;
            RightHand = item;
            RemoveItemSafe(item);
            return true;
        }

        if (RightHand == null)
        {
            RightHand = item;
        }
        else if (LeftHand == null)
        {
            LeftHand = item;
        }
        else
        {
            return false;
        }

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

        if (RightHand == item)
        {
            RightHand = null;
        }
        else if (LeftHand == item)
        {
            LeftHand = null;
        }
        else
        {
            return false;
        }

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