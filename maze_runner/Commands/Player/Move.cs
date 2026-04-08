using maze_runner.Commands.Core;
using maze_runner.Core;

namespace maze_runner.Commands.Player;

public class Move(int dx, int dy) : ICommand
{
    public bool CanExecute(IGameContext ctx)
    {
        int targetX = ctx.EntityManager.Player.Position.Row + dx;
        int targetY = ctx.EntityManager.Player.Position.Col + dy;
        
        return ctx.CurrentMap.GetTile(targetX, targetY).TryEnter();
    }

    public void Execute(IGameContext ctx)
    {
        int newX = ctx.EntityManager.Player.Position.Row + dx;
        int newY = ctx.EntityManager.Player.Position.Col + dy;
        
        ctx.EntityManager.Player.Position = (newX, newY);
    }
}