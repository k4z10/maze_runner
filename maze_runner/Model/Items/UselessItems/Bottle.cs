using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.UselessItems;

public class Bottle : UselessItem
{
    public override string Name => "Bottle";
    public override string Description => "A bottle without water. Water is gone.";
    public override char TileSymbol { get; } = '⛣';
    public override UselessItem Clone() => new Bottle();
}