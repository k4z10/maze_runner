namespace maze_runner.Dungeon.Strategies;
using Terminal.Gui;
using Builders;
using Core;
using Commands.Core;
using Commands;

public class EasyDungeonStrategy : IDungeonGenerationStrategy
{
    public LevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var ctx = builder.CreateFullDungeon(width, height)
            .AddCentralRoom(5, 3)
            .AddStartingRoom()
            .AddRooms(10)
            .ConnectRooms()
            .AddWeapons(10)
            .AddUselessItems(10)
            .Build();

        string levelDescription = "This is easy level.";
        
        return new LevelContext(ctx.Item1, ctx.Item2, levelDescription, "Easy");
    }
}