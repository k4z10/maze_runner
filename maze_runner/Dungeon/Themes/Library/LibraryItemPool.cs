using maze_runner.Items.Models;
using maze_runner.Items.Modifiers;
using maze_runner.Items.UselessItems;
using maze_runner.Items.Weapons;

namespace maze_runner.Dungeon.Themes.Library;

public class LibraryItemPool : IItemPool
{
    private readonly WeightedPool<Item> _pool = new();

    public LibraryItemPool()
    {
        _pool.Add(() => new Feather(), 70);
        _pool.Add(() => new KnowledgeModifier(new Feather()), 20);
    }
    
    public Item GetItem() => _pool.Draw();
}