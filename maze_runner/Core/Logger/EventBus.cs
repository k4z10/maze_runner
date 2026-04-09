using Terminal.Gui;

namespace maze_runner.Core.Logger;

public class EventBus
{
    public EventTopic<WallBumpedEvent> WallBumped { get; } = new();
    public EventTopic<ItemEquippedEvent> ItemEquipped { get; } = new();
    public EventTopic<ItemPickedUpEvent> ItemPickedUp { get; } = new();
    public EventTopic<AttackResolvedEvent> AttackResolved { get; } = new();
    public EventTopic<EnemyDefeatedEvent> EnemyDefeated { get; } = new();
    public EventTopic<UnknownInputEvent> UnknownInput { get; } = new();
}

public record WallBumpedEvent() : IEvent;
public record ItemEquippedEvent(string ItemName) : IEvent;
public record ItemPickedUpEvent(string ItemName) : IEvent;

public record AttackResolvedEvent(string Attacker, string Defender, int Damage) : IEvent;
public record EnemyDefeatedEvent(string EnemyName) : IEvent;
public record UnknownInputEvent(Key Key) : IEvent;
