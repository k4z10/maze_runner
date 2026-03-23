namespace maze_runner.Dungeon.Builders;
using Map;
public interface IModifierDungeonBuilder
{
    IModifierDungeonBuilder AddRooms(int maxCount);
    IModifierDungeonBuilder AddCentralRoom(int width, int height);
    IModifierDungeonBuilder AddUselessItems(int count);
    IModifierDungeonBuilder AddWeapons(int count);
    IModifierDungeonBuilder ConnectRooms();
    Map Build();
}