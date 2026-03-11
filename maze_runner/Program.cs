namespace maze_runner;

static class Program
{
    static void Main(string[] args)
    {
        // EmptyMaze map = new EmptyMaze(20, 40);
         var map = new FromFileMaze(20, 40, "maze1.txt"); 
        // var map = new EmptyMaze(20, 40);
        // var map = new RandomTiles(20, 40);
        map.GenerateMaze();

        Console.WriteLine(Environment.CurrentDirectory);

        // Dodawanie Item do Planszy
        // TODO: delete
        map.Tiles[1,1].AddItem(new Coin(10));
        map.Tiles[1,2].AddItem(new Gold(10));
        map.Tiles[0,1].AddItem(new Feather());
        map.Tiles[0,2].AddItem(new Stick());
        map.Tiles[0,3].AddItem(new Bottle());
        for (var i = 0; i < 20; i++)
        {
            Item[] potential = [new Knife(), new Sword(), new LongSword(), new Coin(1), new Gold(1), new Feather(), new Bottle(), new Stick()];
            map.Tiles[3,3].AddItem(potential[Random.Shared.Next(0, potential.Length)]);
        }
        map.Tiles[1,1].AddItem(new Sword());
        map.Tiles[1,2].AddItem(new Knife());
        map.Tiles[1,3].AddItem(new LongSword());
        
        var engine = new GameEngine(map);
        engine.Run();
    }
}