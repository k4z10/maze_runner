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
        
        inputHandler.RegisterCommand(KeyCode.W, new Move(-1, 0));
        inputHandler.RegisterCommand(KeyCode.S, new Move(1, 0));
        inputHandler.RegisterCommand(KeyCode.A, new Move(0, -1));
        inputHandler.RegisterCommand(KeyCode.D, new Move(0, 1));
        
        inputHandler.RegisterCommand(KeyCode.E, new PickUp());
        inputHandler.RegisterCommand(KeyCode.Q, new Drop());
        inputHandler.RegisterCommand(KeyCode.F, new Equip());
        inputHandler.RegisterCommand(KeyCode.ShiftMask | KeyCode.F, new Unequip());
        
        string levelDescription = "This is easy level.";
        
        return new LevelContext(map, inputHandler, levelDescription, "Easy");
    }
}