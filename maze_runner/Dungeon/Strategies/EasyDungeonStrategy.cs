namespace maze_runner.Dungeon.Strategies;
using Terminal.Gui;
using Builders;
using Core;
using Commands.Core;
using Commands;

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