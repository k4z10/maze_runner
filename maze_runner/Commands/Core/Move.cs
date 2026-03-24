using maze_runner.Core;

namespace maze_runner.Commands.Core;

public class Move : ICommand
{
    private readonly int _dx;
    private readonly int _dy;

    public Move(int dx, int dy)
    {
        _dx = dx;
        _dy = dy;
    }

    public bool CanExecute(IGameContext ctx)
    {
        int targetX = ctx.Player.Position.X + _dx;
        int targetY = ctx.Player.Position.Y + _dy;
        
        return ctx.Map.GetTile(targetX, targetY).TryEnter(ctx.Player);
    }

    public void Execute(IGameContext ctx)
    {
        int newX = ctx.Player.Position.X + _dx;
        int newY = ctx.Player.Position.Y + _dy;
        
        ctx.Player.Position = (newX, newY);
    }
}