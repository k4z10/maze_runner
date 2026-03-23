namespace maze_runner;
public interface IBaseDungeonBuilder
{
    IModifierDungeonBuilder CreateEmptyDungeon(int width, int height);
    IModifierDungeonBuilder CreateFullDungeon(int width, int height);
}

public interface IModifierDungeonBuilder
{
    IModifierDungeonBuilder AddRooms(int maxCount);
    IModifierDungeonBuilder AddCentralRoom(int width, int height);
    IModifierDungeonBuilder AddUselessItems(int count);
    IModifierDungeonBuilder AddWeapons(int count);
    IModifierDungeonBuilder ConnectRooms();
    Map Build();
}