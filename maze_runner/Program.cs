namespace maze_runner;

static class Program
{
    static void Main(string[] args)
    {
        // EmptyMaze map = new EmptyMaze(20, 40);
        // var map = new FromFileMaze(20, 40, "maze1.txt"); 
        var map = new EmptyMaze(20, 40);
        // var map = new RandomTiles(20, 40);
        map.GenerateMaze();
        
        // Dodawanie Item do Planszy
        // TODO: delete
        map.Tiles[10,10].AddItem(new Coin(10));
        map.Tiles[10,10].AddItem(new Gold(10));
        map.Tiles[11,11].AddItem(new Feather());
        map.Tiles[10,11].AddItem(new Stick());
        map.Tiles[9,11].AddItem(new Bottle());
        for (var i = 0; i < 20; i++)
        {
            map.Tiles[3,3].AddItem(new Knife());
        }
        map.Tiles[1,1].AddItem(new Sword());
        map.Tiles[1,2].AddItem(new Knife());
        map.Tiles[1,3].AddItem(new LongSword());
        
        var engine = new GameEngine(map);
        engine.Run();
    }
}