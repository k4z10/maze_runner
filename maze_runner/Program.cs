namespace maze_runner;

static class Program
{
    static void Main(string[] args)
    {
        // EmptyMaze map = new EmptyMaze(20, 40);
        var map = new FromFileMaze(20, 40, "maze1.txt"); 
        map.GenerateMaze();
        var engine = new GameEngine(map);
        engine.Run();
    }
}