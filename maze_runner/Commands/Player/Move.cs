using maze_runner.Commands.Core;
using maze_runner.Core;
using maze_runner.Core.Logger;

namespace maze_runner.Commands.Player;

public class Move(ILevelContext ctx, int dx, int dy) : ICommand
{
    public void Execute()
    {
        int targetX = ctx.EntityManager.Player.Position.Row + dx;
        int targetY = ctx.EntityManager.Player.Position.Col + dy;

        if (ctx.Map.GetTile(targetX, targetY).TryEnter())
        {
            ctx.EntityManager.Player.Position = (targetX, targetY);
        }
        else
        {
            ctx.EventBus.Publish(new WallBumpedEvent());
        }
    }
}