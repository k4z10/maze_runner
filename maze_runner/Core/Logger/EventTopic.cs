namespace maze_runner.Core.Logger;

public class EventTopic<T> where T : IEvent
{
    private readonly List<Action<T>> _subscribers = new();

    public void Subscribe(Action<T> handler)
    {
        _subscribers.Add(handler); 
    }

    public void Publish(T gameEvent)
    {
        foreach (var handler in _subscribers)
            handler(gameEvent);
    }
}