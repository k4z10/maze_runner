namespace maze_runner.Dungeon.Strategies;
using Core;
using Terminal.Gui;
using Commands.Core;
using Builders;
using Commands;

public class InitialDungeonStrategy : IDungeonGenerationStrategy
{
    public LevelContext Generate(int width, int height)
    {
        var builder = new ProcDungeonBuilder();
        var map = builder.CreateEmptyDungeon(width, height).Build();

        var inputHandler = new InputHandler();
        
        inputHandler.RegisterCommand(KeyCode.W, new Move(-1, 0));
        inputHandler.RegisterCommand(KeyCode.S, new Move(1, 0));
        inputHandler.RegisterCommand(KeyCode.A, new Move(0, -1));
        inputHandler.RegisterCommand(KeyCode.D, new Move(0, 1));
        
        string levelDescription = "Initial level.";
        
        return new LevelContext(map, inputHandler, levelDescription);
    }
}