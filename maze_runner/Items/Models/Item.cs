using maze_runner.Items.Visitors;

namespace maze_runner.Items.Models;

public abstract class Item
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract char TileSymbol { get; }
    
    public abstract void Accept(IItemVisitor visitor);
}