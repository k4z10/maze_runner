using maze_runner.Model.Dungeon.Builders;

namespace maze_runner.Model.Dungeon.Strategies;

public class EasyDungeonStrategy : IDungeonGenerationStrategy
{
    public IModifierDungeonBuilder GenerateTopology(IBaseDungeonBuilder baseBuilder, int width, int height)
    {
        return baseBuilder.CreateFullDungeon(width, height)
            .AddCentralRoom(5, 3)
            .AddStartingRoom()
            .AddRooms(10)
            .ConnectRooms();
    }
}