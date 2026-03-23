namespace maze_runner;
public interface IItemVisitor
{
    void Visit(Weapon weapon);
    void Visit(Coin coin);
    void Visit(Gold gold);
    void Visit(UselessItem uselessItem);
}
public abstract class Item
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract char TileSymbol { get; }
    
    public abstract void Accept(IItemVisitor visitor);
}