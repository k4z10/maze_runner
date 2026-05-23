using maze_runner.Model.Entities;

namespace maze_runner.Model.Core.Events;

public interface IEvent
{
    string? LogMessage { get; }
}

public record WallBumpedEvent(string PlayerName) : IEvent
{
    public string? LogMessage => $"`{PlayerName}` tried walk into a wall";
}

public record ItemEquippedEvent(string PlayerName, string ItemName) : IEvent
{
    public string? LogMessage => $"`{PlayerName}` equipped {ItemName}";
}

public record ItemPickedUpEvent(string PlayerName, string ItemName) : IEvent
{
    public string? LogMessage => $"`{PlayerName}` picked up {ItemName}";
}

public record AttackResolvedEvent(string Attacker, string Defender, int Damage) : IEvent
{
    public string? LogMessage => $"`{Attacker}` attacked `{Defender}`, dealing {Damage} HP";
}

public record EntityDefeatedEvent(string EntityName) : IEvent
{
    public string? LogMessage => $"`{EntityName}` was defeated";
}

public record UnknownInputEvent(char Key) : IEvent
{
    public string? LogMessage => $"Unknown input: {Key}"; 
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
    Entity Entity,
    (int Row, int Col) Origin,
    int Distance
    ) : IEvent
{
    public string? LogMessage => $"[{Entity.Position.Row}, {Entity.Position.Col}] {Entity.Name} registered sound from [{Origin.Row}, {Origin.Col}] (Distance: {Distance})";
}