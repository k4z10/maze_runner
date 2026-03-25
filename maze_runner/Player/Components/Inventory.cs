namespace maze_runner.Player.Components;
using Items.Models;

public class Inventory
{
    public List<Item> Items = new();
    public int CurrentIndex = 0;
    public Account Bundle = new();
    
    public Item? LeftHand = null;
    public Item? RightHand = null;

    public bool TryEquip(Weapon weapon)
    {
        if (weapon.LightOrHeavy == Weapon.Weight.Heavy)
        {
            if (LeftHand != null ||  RightHand != null)
                return false;
            LeftHand = weapon;
            RightHand = weapon;
            Items.Remove(weapon);
            return true;
        }
        if (RightHand != null)
            return false;
        RightHand = weapon;
        Items.Remove(weapon);
        return true;
    }

    public bool TryUnequip(Weapon weapon)
    {
        if (weapon.LightOrHeavy == Weapon.Weight.Heavy)
        {
            if (LeftHand != weapon || RightHand != weapon)
                return false;
            LeftHand = null;
            RightHand = null;
            Items.Add(weapon);
            return true;
        }
        if (RightHand != weapon)
            return false;
        RightHand = null;
        Items.Add(weapon);
        return true;
    }

    public bool TryEquip(UselessItem uselessItem)
    {
        if (LeftHand != null)
            return false;
        LeftHand = uselessItem;
        Items.Remove(uselessItem);
        return true;
    }

    public bool TryUnequip(UselessItem uselessItem)
    {
        if (LeftHand == null)
            return false;
        LeftHand = null;
        Items.Add(uselessItem);
        return true;
    }

    public class Account
    {
        public int Gold { get; set; }
        public int Coins { get; set; }

        public bool TryPayGold(int amount)
        {
            if (Gold < amount)
                return false;
            Gold -= amount;
            return true;
        }

        public bool TryPayCoin(int amount)
        {
            if (Coins < amount)
                return false;
            Coins -= amount;
            return true;
        }
    }
}