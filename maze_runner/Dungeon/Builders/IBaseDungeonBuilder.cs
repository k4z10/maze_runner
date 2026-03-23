namespace maze_runner.Dungeon.Builders;

public interface IBaseDungeonBuilder
{
    IModifierDungeonBuilder CreateEmptyDungeon(int width, int height);
    IModifierDungeonBuilder CreateFullDungeon(int width, int height);
}
