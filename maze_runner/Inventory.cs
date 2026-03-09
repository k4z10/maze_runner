using System.Text;

namespace maze_runner;

public class Inventory
{
    public List<Item> Items = new();
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

public class EquipItemVisitor(Player player) : IItemVisitor
{
    public void Visit(Weapon weapon)
    {
        player.Inventory.TryEquip(weapon);
    }

    public void Visit(Coin coin)
    {
    }

    public void Visit(Gold gold)
    {
    }

    public void Visit(UselessItem uselessItem)
    {
        player.Inventory.TryEquip(uselessItem);
    }  
}

public class PickUpItemVisitor(Player player) : IItemVisitor
{
    public void Visit(Weapon weapon)
    {
        player.Inventory.Items.Add(weapon);
    }
    
    public void Visit(Coin coin)
    {
        player.Inventory.Bundle.Coins += coin.Amount;
    }

    public void Visit(Gold gold)
    {
        player.Inventory.Bundle.Gold += gold.Amount;
    }

    public void Visit(UselessItem uselessItem)
    {
        player.Inventory.Items.Add(uselessItem);
    }
}

public class DropItemVisitor(Player player, Tile currentTile) : IItemVisitor
{
    public void Visit(Weapon weapon)
    {
        player.Inventory.TryUnequip(weapon);
        player.Inventory.Items.Remove(weapon);
        currentTile.AddItem(weapon);
    }

    public void Visit(UselessItem uselessItem)
    {
        player.Inventory.TryUnequip(uselessItem);
        player.Inventory.Items.Remove(uselessItem);
        currentTile.AddItem(uselessItem);
    }

    public void Visit(Coin coin)
    {
        player.Inventory.Bundle.Coins--;
        var newCoin = new Coin(1);
        currentTile.AddItem(newCoin);
    }

    public void Visit(Gold gold)
    {
        player.Inventory.Bundle.Gold--;
        var newGold = new Gold(1);
        currentTile.AddItem(newGold);
    }
}
