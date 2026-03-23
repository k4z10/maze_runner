namespace maze_runner.Items.UselessItems;
using Models;
public class Bottle : UselessItem
{
    public override string Name => "Bottle";
    public override string Description => "A bottle without water. Water is gone.";
    public override char TileSymbol { get; } = '⛣';
    public override UselessItem Clone() => new Bottle();
}