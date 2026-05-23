using maze_runner.Model.Core;
using maze_runner.Model.Items.Models;

namespace maze_runner.Model.Dungeon.Builders;

public interface IModifierDungeonBuilder
{
    IModifierDungeonBuilder AddRooms(int maxCount);
    IModifierDungeonBuilder AddStartingRoom();
    
    IModifierDungeonBuilder AddCentralRoom(int width, int height);
    IModifierDungeonBuilder ConnectRooms();
    
    IModifierDungeonBuilder PopulateItems(IItemPool pool, int count);
    IModifierDungeonBuilder PopulateEnemies(IEnemyPool pool, int count);
    IModifierDungeonBuilder PlaceArtifact(Item artifact);
    
    ILevelContext GetLevelContext();
}