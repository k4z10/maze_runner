namespace maze_runner;

public class Inventory
{
    public List<Item> Items = new();
    public Account Bundle = new();
    
    private Item? leftHand = null;
    private Item? rightHand = null;

    public bool TryEquip(Weapon weapon)
    {
        if (weapon.LightOrHeavy == Weapon.Weight.Heavy)
        {
            if (leftHand != null ||  rightHand != null)
                return false;
            leftHand = weapon;
            rightHand = weapon;
            return true;
        }
        if (rightHand != null)
            return false;
        rightHand = weapon;
        return true;
    }

    public bool TryEquip(UselessItem uselessItem, bool toLeftHand)
    {
        if (toLeftHand)
        {
            if (leftHand == null)
                return false;
            leftHand = uselessItem;
            return true;
        }
        if (rightHand == null)
            return false;
        rightHand = uselessItem;
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

public class EquipItemVisitor(Player player, bool toLeftHand) : IItemVisitor
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
        player.Inventory.TryEquip(uselessItem, toLeftHand);
    }  
}

public class PickUpItemVisitor(Player player, Tile tile)
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
