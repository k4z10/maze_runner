namespace maze_runner.Items.Visitors;
using Items.Models;

public class FunctionalItemVisitor(
    Action<Weapon> onWeapon = null,
    Action<UselessItem> onUseless = null,
    Action<Gold> onGold = null,
    Action<Coin> onCoin = null)
    : IItemVisitor
{
    private readonly Action<Weapon> _onWeapon = onWeapon ?? (_ => { });
    private readonly Action<UselessItem> _onUseless = onUseless ?? (_ => { });
    private readonly Action<Gold> _onGold = onGold ?? (_ => { });
    private readonly Action<Coin> _onCoin = onCoin ?? (_ => { });

    public void Visit(Weapon weapon) => _onWeapon(weapon);
    public void Visit(UselessItem uselessItem) => _onUseless(uselessItem);
    public void Visit(Gold gold) => _onGold(gold);
    public void Visit(Coin coin) => _onCoin(coin);
}