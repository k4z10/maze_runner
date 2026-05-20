using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record ItemDto
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("type")] public string Type { get; init; } = string.Empty;
    [JsonPropertyName("n")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("dmg")] public int? Damage { get; init; } 
    [JsonPropertyName("amt")] public int? Amount { get; init; }
    [JsonPropertyName("sym")] public char Symbol { get; init; }
    [JsonPropertyName("attr_mod")] public AttributesDto? StatModifiers { get; init; }
}