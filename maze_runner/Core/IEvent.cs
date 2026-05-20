using maze_runner.Model.Entities;

namespace maze_runner.Core;

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

public record UnknownInputEvent(char Key) : IEvent
{
    public string? LogMessage => $"Unknown input: {Key}"; 
}

public record TriggerQuit() : IEvent
{
    public string? LogMessage => null;
}

public record AcousticWavePropagate(
    Dictionary<(int Row, int Col), int> Wave,
    (int Row, int Col) Origin,
    string SourceName
    ) : IEvent
{
    public string? LogMessage => null;
}

public record SoundRegisteredEvent(
    Entity Enemy,
    (int Row, int Col) Origin,
    int Distance
    ) : IEvent
{
    public string? LogMessage => $"[{Enemy.Position.Row}, {Enemy.Position.Col}] {Enemy.Name} registered sound from [{Origin.Row}, {Origin.Col}] (Distance: {Distance})";
}