using maze_runner.Items.Models;

namespace maze_runner.Items.Visitors;

public interface IItemVisitor
{
    void Visit(Weapon weapon);
    void Visit(Coin coin);
    void Visit(Gold gold);
    void Visit(UselessItem uselessItem);
}
