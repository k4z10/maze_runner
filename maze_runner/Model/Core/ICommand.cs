using maze_runner.Core;

namespace maze_runner.Model.Core;

public interface ICommand
{
    void Execute(ILevelContext ctx, int playerId);
}