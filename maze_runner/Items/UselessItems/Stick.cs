namespace maze_runner.Items.UselessItems;
using Models;
public class Stick : UselessItem
{
    public override string Name => "Stick";
    public override string Description => "The stickiest stick in the whole universe";
    public override char TileSymbol { get; } = '|';
    public override UselessItem Clone() => new Stick();
}