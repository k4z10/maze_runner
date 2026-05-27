using maze_runner.Network.DTOs.GameState;

namespace maze_runner.View;
public interface IGameFrontend
{
    void Run();
    void RenderSnapshot(GameStateSnapshotDto snapshot);
}