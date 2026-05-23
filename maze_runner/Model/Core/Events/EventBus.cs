using System.Runtime.CompilerServices;
using maze_runner.Model.Core.Logger;

namespace maze_runner.Model.Core.Events;

public interface IEventPublisher
{
    void Publish<T>(T gameEvent) where T : IEvent;
}

public interface IEventSubscriber
{
    void Subscribe<T>(Action<T> action) where T : IEvent;
    void Unsubscribe<T>(Action<T> action) where T : IEvent;
}

internal static class EventTopic<T> where T : IEvent
{
    private static Action<T>[] _subscribers = [];
    private static readonly Lock _lock = new();

    public static void Subscribe(Action<T> handler)
    {
        lock (_lock)
        {
            var oldList = _subscribers.ToList();
            oldList.Add(handler);
            _subscribers = oldList.ToArray();
        }
    }

    public static void Unsubscribe(Action<T> handler)
    {
        lock (_lock)
        {
            var oldList = _subscribers.ToList();
            oldList.Remove(handler);
            _subscribers = oldList.ToArray();
        }
    }

    public static void Publish(T gameEvent)
    {
        var currentSubscribers = _subscribers;
        
        foreach (var handler in currentSubscribers)
        {
            handler(gameEvent);
        }
    }
}

public class EventBus : IEventPublisher, IEventSubscriber
{
    public Action<string>? OnLogGenerated { get; set; }

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        EventTopic<T>.Subscribe(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        EventTopic<T>.Unsubscribe(handler);
    }

    public void Publish<T>(T gameEvent) where T : IEvent
    {
        EventTopic<T>.Publish(gameEvent);

        if (gameEvent.LogMessage != null)
        {
            OnLogGenerated?.Invoke(gameEvent.LogMessage);
        }
    }
}