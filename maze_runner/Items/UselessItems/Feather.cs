namespace maze_runner.Items.UselessItems;
using Models;
public class Feather : UselessItem
{
    public override string Name => "Feather";
    public override string Description => "Light-weight but useless.";
    public override char TileSymbol { get; } = '⟆';
    public override UselessItem Clone() => new Feather();
}