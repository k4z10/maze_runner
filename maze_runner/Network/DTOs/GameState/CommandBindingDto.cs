using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record CommandBindingDto
{
    [JsonPropertyName("cmd")] public string CommandId { get; init; } = string.Empty;
    [JsonPropertyName("desc")] public string Description { get; init; } = string.Empty;
}