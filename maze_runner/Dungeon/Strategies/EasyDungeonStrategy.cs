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
        var map = builder.CreateFullDungeon(width, height)
            .AddCentralRoom(5, 3)
            .AddStartingRoom()
            .AddRooms(10)
            .ConnectRooms()
            .AddWeapons(10)
            .AddUselessItems(10)
            .Build();

        var inputHandler = new InputHandler();
        
        inputHandler.RegisterCommand(Key.W, new Move(-1, 0));
        inputHandler.RegisterCommand(Key.S, new Move(1, 0));
        inputHandler.RegisterCommand(Key.A, new Move(0, -1));
        inputHandler.RegisterCommand(Key.D, new Move(0, 1));
        
        inputHandler.RegisterCommand(Key.E, new PickUp());
        inputHandler.RegisterCommand(Key.Q, new Drop());
        inputHandler.RegisterCommand(Key.F, new Equip());
        inputHandler.RegisterCommand(Key.F.WithShift, new Unequip());
        
        string levelDescription = "This is easy level.";
        
        return new LevelContext(map, inputHandler, levelDescription, "Easy");
    }
}