namespace maze_runner.Model.Dungeon;

public class GuaranteedWeightedPool<T>
{
    private readonly List<(Func<T> Factory, int Weight)> _entries = new();
    private readonly Random _random = new();
    private int _totalWeight = 0;

    private Queue<Func<T>> _mandatoryQueue = new();
    private bool _isInitialized = false;

    public void Add(Func<T> factory, int weight)
    {
        if (weight <= 0) return;
        
        _entries.Add((factory, weight));
        _totalWeight += weight;
        
        _isInitialized = false; 
    }

    private void InitializeMandatoryQueue()
    {
        var mandatoryList = _entries.Select(e => e.Factory).ToList();
        
        int n = mandatoryList.Count;
        while (n > 1)
        {
            n--;
            int k = _random.Next(n + 1);
            (mandatoryList[k], mandatoryList[n]) = (mandatoryList[n], mandatoryList[k]);
        }
        
        _mandatoryQueue = new Queue<Func<T>>(mandatoryList);
        _isInitialized = true;
    }

    public T Draw()
    {
        if (_totalWeight == 0)
            throw new InvalidOperationException("Pool is empty.");

        if (!_isInitialized)
            InitializeMandatoryQueue();

        if (_mandatoryQueue.Count > 0)
        {
            return _mandatoryQueue.Dequeue()();
        }

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

    public void ResetGuarantee()
    {
        _isInitialized = false;
    }
}