namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;
using Commands.Core;
using Terminal.Gui;
using Commands;

public class TestDungeonStrategy : IDungeonGenerationStrategy
{
    public LevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var map = builder.CreateEmptyDungeon(width, height)
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
        
        string levelDescription = "This is test level.";
        
        return new LevelContext(map, inputHandler, levelDescription);
    }
}