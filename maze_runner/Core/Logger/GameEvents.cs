using Terminal.Gui;

namespace maze_runner.Core.Logger;

public static class GameEvents
{
    public static EventTopic<WallBumpedEvent> WallBumped { get; } = new();
    public static EventTopic<ItemEquippedEvent> ItemEquipped { get; } = new();
    public static EventTopic<ItemPickedUpEvent> ItemPickedUp { get; } = new();
    public static EventTopic<AttackResolvedEvent> AttackResolved { get; } = new();
    public static EventTopic<EnemyDefeatedEvent> EnemyDefeated { get; } = new();
    public static EventTopic<UnknownInputEvent> UnknownInput { get; } = new();
    
    public static void LogBridge(IMessageLog logger)
    {
        GameEvents.WallBumped.Subscribe(_ => logger.Log("Tried to walk into wall"));
        GameEvents.ItemEquipped.Subscribe(e => logger.Log($"Equipped item: {e.ItemName}"));
        GameEvents.AttackResolved.Subscribe(e => logger.Log($"{e.Attacker} dealt {e.Damage} damage to enemy: {e.Defender}"));
        GameEvents.EnemyDefeated.Subscribe(e => logger.Log($"Enemy defeated: {e.EnemyName}"));
        GameEvents.ItemPickedUp.Subscribe(e => logger.Log($"Picked up item: {e.ItemName}"));
        GameEvents.UnknownInput.Subscribe(e => logger.Log($"Unknown input: [{e.Key}]"));
    }
}

public record WallBumpedEvent() : IEvent;
public record ItemEquippedEvent(string ItemName) : IEvent;
public record ItemPickedUpEvent(string ItemName) : IEvent;
public record AttackResolvedEvent(string Attacker, string Defender, int Damage) : IEvent;
public record EnemyDefeatedEvent(string EnemyName) : IEvent;
public record UnknownInputEvent(Key Key) : IEvent;
