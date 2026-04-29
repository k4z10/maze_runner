using System.Runtime.CompilerServices;
using maze_runner.Core.Logger;

namespace maze_runner.Core;

public interface IEventPublisher
{
    void Publish<T>(T gameEvent) where T : IEvent;
}

public interface IEventSubscriber
{
    void Subscribe<T>(Action<T> action) where T : IEvent;
    void Unsubscribe<T>(Action<T> action) where T : IEvent;
}

public class EventBus : IEventPublisher, IEventSubscriber
{
    public void Publish<T>(T gameEvent) where T : IEvent
    => EventRouter<T>.GetTopic(this).Publish(gameEvent);

    public void Subscribe<T>(Action<T> action) where T : IEvent
    => EventRouter<T>.GetTopic(this).Subscribe(action);

    public void Unsubscribe<T>(Action<T> action) where T : IEvent
    => EventRouter<T>.GetTopic(this).Unsubscribe(action);
}

internal class EventTopic<T> where T : IEvent
{
    private readonly List<Action<T>> _subscribers = new();

    public void Subscribe(Action<T> handler) => _subscribers.Add(handler);
    public void Unsubscribe(Action<T> handler) => _subscribers.Remove(handler);

    public void Publish(T gameEvent)
    {
        foreach (var handler in _subscribers.ToArray())
        {
            handler(gameEvent);
        }

        if (gameEvent.LogMessage != null)
            UniversalLogChannel.Publish(gameEvent.LogMessage);
    }
}

internal static class EventRouter<T> where T : IEvent
{
    private static readonly ConditionalWeakTable<EventBus, EventTopic<T>> _topics = new();

    public static EventTopic<T> GetTopic(EventBus bus)
    {
        return _topics.GetValue(bus, _ => new EventTopic<T>());
    }
}