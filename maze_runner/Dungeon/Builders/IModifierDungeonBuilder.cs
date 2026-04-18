using maze_runner.Core;

namespace maze_runner.Dungeon.Builders;
using Map;
public interface IModifierDungeonBuilder
{
    IModifierDungeonBuilder AddRooms(int maxCount);
    IModifierDungeonBuilder AddStartingRoom();
    
    IModifierDungeonBuilder AddCentralRoom(int width, int height, bool secure);
    IModifierDungeonBuilder AddUselessItems(int count);
    IModifierDungeonBuilder AddWeapons(int count);
    IModifierDungeonBuilder ConnectRooms();
    ILevelContext Build();
}