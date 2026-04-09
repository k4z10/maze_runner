using maze_runner.Commands.Core;
using maze_runner.Core;

namespace maze_runner.Commands.Player;

public class Move(int dx, int dy) : ICommand
{
    public bool CanExecute(IGameContext ctx)
    {
        int targetX = ctx.CurrentLevel.EntityManager.Player.Position.Row + dx;
        int targetY = ctx.CurrentLevel.EntityManager.Player.Position.Col + dy;
        
        return ctx.CurrentLevel.Map.GetTile(targetX, targetY).TryEnter();
    }

    public void Execute(IGameContext ctx)
    {
        int newX = ctx.CurrentLevel.EntityManager.Player.Position.Row + dx;
        int newY = ctx.CurrentLevel.EntityManager.Player.Position.Col + dy;
        
        ctx.CurrentLevel.EntityManager.Player.Position = (newX, newY);
    }
}