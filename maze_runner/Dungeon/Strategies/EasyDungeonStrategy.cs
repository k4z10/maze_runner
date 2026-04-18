namespace maze_runner.Dungeon.Strategies;
using Terminal.Gui;
using Builders;
using Core;
using Commands.Core;
using Commands;

public class EasyDungeonStrategy : IDungeonGenerationStrategy
{
    public ILevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var ctx = builder.CreateFullDungeon(width, height)
            .AddCentralRoom(5, 3, true)
            .AddStartingRoom()
            .AddRooms(10)
            .ConnectRooms()
            .AddWeapons(10)
            .AddUselessItems(10)
            .Build();

        ctx.Description = "This is easy level.";
        ctx.LevelName = "Easy";

        return ctx;
    }
}