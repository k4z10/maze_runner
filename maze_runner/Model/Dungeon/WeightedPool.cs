namespace maze_runner.Model.Dungeon;

public class WeightedPool<T>
{
    private readonly List<(Func<T> Factory, int Weight)> _entries = new();
    private readonly Random _random = new();
    private int _totalWeight = 0;

    public void Add(Func<T> factory, int weight)
    {
        if (weight <= 0) return;
        
        _entries.Add((factory, weight));
        _totalWeight += weight;
    }

    public T Draw()
    {
        if (_totalWeight == 0)
            throw new InvalidOperationException("Pool is empty.");

        int roll = _random.Next(0, _totalWeight);

        foreach (var (factory, weight) in _entries)
        {
            if (roll < weight)
            {
                return factory();
            }
            roll -= weight;
        }

        return _entries.Last().Factory();
    }
}
