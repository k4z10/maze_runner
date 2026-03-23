namespace maze_runner;

public interface IDungeonGenerationStrategy
{
    Map Generate(IBaseDungeonBuilder builder, int width, int height);
}

public class EasyDungeonStrategy : IDungeonGenerationStrategy
{
    public Map Generate(IBaseDungeonBuilder builder, int width, int height)
    {
        return builder.CreateFullDungeon(width, height)
            .AddRooms(10)
            .ConnectRooms()
            .AddWeapons(15)
            .Build();
    }
}