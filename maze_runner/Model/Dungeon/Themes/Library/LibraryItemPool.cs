using maze_runner.Model.Items.Models;
using maze_runner.Model.Items.Modifiers;
using maze_runner.Model.Items.UselessItems;

namespace maze_runner.Model.Dungeon.Themes.Library;

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