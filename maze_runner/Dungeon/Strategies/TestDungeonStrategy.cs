namespace maze_runner.Dungeon.Strategies;
using Core;
using Builders;
using Commands.Core;
using Terminal.Gui;

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
        
        inputHandler.RegisterCommand(KeyCode.W, new Move(-1, 0));
        inputHandler.RegisterCommand(KeyCode.S, new Move(1, 0));
        inputHandler.RegisterCommand(KeyCode.A, new Move(0, -1));
        inputHandler.RegisterCommand(KeyCode.D, new Move(0, 1));
        
        inputHandler.RegisterCommand(KeyCode.E, new PickUp());
        
        return new LevelContext(map, inputHandler);
    }
}