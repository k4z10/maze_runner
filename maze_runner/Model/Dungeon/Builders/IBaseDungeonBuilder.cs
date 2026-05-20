namespace maze_runner.Model.Dungeon.Builders;

public interface IBaseDungeonBuilder
{
    IModifierDungeonBuilder CreateEmptyDungeon(int width, int height);
    IModifierDungeonBuilder CreateFullDungeon(int width, int height);
}
