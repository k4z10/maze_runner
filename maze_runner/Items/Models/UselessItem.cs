namespace maze_runner.Items.Models;
using Visitors;
public abstract class UselessItem : Item
{
    public override void Accept(IItemVisitor visitor) => visitor.Visit(this);
    public abstract UselessItem Clone();
}