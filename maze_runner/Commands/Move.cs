namespace maze_runner.Commands;
using Core;
using maze_runner.Core;

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
        int targetX = ctx.Player.Position.Row + _dx;
        int targetY = ctx.Player.Position.Col + _dy;
        
        return ctx.Map.GetTile(targetX, targetY).TryEnter(ctx.Player);
    }

    public void Execute(IGameContext ctx)
    {
        int newX = ctx.Player.Position.Row + _dx;
        int newY = ctx.Player.Position.Col + _dy;
        
        ctx.Player.Position = (newX, newY);
    }

    public string Description { get; } = "Move player";
}