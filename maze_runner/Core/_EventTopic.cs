using maze_runner.Core.Logger;

namespace maze_runner.Core;

public static class _EventTopic<T> where T : IEvent
{
    private static readonly List<Action<T>> Subscribers = new();
    public static void Subscribe(Action<T> handler) => Subscribers.Add(handler); 

    public static void Publish(T gameEvent)
    {
        foreach (var handler in Subscribers)
            handler(gameEvent);

        if (gameEvent.LogMessage != null)
        {
            UniversalLogChannel.Publish(gameEvent.LogMessage);
        }
    }
}