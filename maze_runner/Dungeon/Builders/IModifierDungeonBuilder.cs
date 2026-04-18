using maze_runner.Core;
using maze_runner.Items.Models;

namespace maze_runner.Dungeon.Builders;
using Map;
public interface IModifierDungeonBuilder
{
    IModifierDungeonBuilder AddRooms(int maxCount);
    IModifierDungeonBuilder AddStartingRoom();
    
    IModifierDungeonBuilder AddCentralRoom(int width, int height, bool secure);
    IModifierDungeonBuilder ConnectRooms();
    
    IModifierDungeonBuilder PopulateItems(IItemPool pool, int count);
    IModifierDungeonBuilder PopulateEnemies(IEnemyPool pool, int count);
    IModifierDungeonBuilder PlaceArtifact(Item artifact);
    
    ILevelContext GetLevelContext();
}