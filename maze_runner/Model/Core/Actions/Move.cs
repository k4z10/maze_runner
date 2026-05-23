using maze_runner.Model.Core.Events;

namespace maze_runner.Model.Core.Actions;

public class Move(int dRow, int dCol) : ICommand
{
    public void Execute(ILevelContext ctx, int playerId)
    {
        var player = ctx.EntityManager.Entities.FirstOrDefault(p => p.Id == playerId);
        if (player == null || !player.IsAlive) return;
        
        int targetX = player.Position.Row + dRow;
        int targetY = player.Position.Col + dCol;

        if (ctx.Map.GetTile(targetX, targetY).IsWalkable)
        {
            player.Position = (targetX, targetY);
        }
        else
        {
            ctx.EventBus.Publish(new WallBumpedEvent(player.Name));
        }
    }
}