using Terminal.Gui;

namespace maze_runner.Core.Logger;

public interface IEvent
{
    string? LogMessage { get; }
}

public record WallBumpedEvent() : IEvent
{
    public string? LogMessage => "Tried walk into wall";
}

public record ItemEquippedEvent(string ItemName) : IEvent
{
    public string? LogMessage => $"Equipped {ItemName}";
}

public record ItemPickedUpEvent(string ItemName) : IEvent
{
    public string? LogMessage => $"Picked up {ItemName}";
}

public record AttackResolvedEvent(string Attacker, string Defender, int Damage) : IEvent
{
    public string? LogMessage => $"{Attacker} attacked {Defender}, dealing {Damage} HP";
}

public record EnemyDefeatedEvent(string EnemyName) : IEvent
{
    public string? LogMessage => $"{EnemyName} was defeated";
}

public record UnknownInputEvent(Key Key) : IEvent
{
    public string? LogMessage => $"Unknown input: {Key}"; 
}