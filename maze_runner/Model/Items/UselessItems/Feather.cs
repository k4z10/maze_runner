using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Items.UselessItems;

public class Feather : UselessItem
{
    public override string Name => "Feather";
    public override string Description => "Light-weight but useless.";
    public override char TileSymbol { get; } = '⟆';
    public override UselessItem Clone() => new Feather();
}