using maze_runner.Model.Items.Models;
using maze_runner.Model.Items.Modifiers;
using maze_runner.Model.Items.Weapons;

namespace maze_runner.Model.Dungeon.Themes.Cave;

public class CaveItemPool : IItemPool
{
    private WeightedPool<Item> _pool = new();

    public CaveItemPool()
    {
        _pool.Add(() => new Sword(), 10);
        _pool.Add(() => new SharpnessModifier(new LongSword()), 1);
        _pool.Add(() => new Gold(1), 1);
    }

    public Item GetItem()
    {
        return _pool.Draw();
    }
}