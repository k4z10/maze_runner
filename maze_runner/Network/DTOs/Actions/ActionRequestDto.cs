using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.Actions;

public record ActionRequestDto
{
    [JsonPropertyName("pid")] public int PlayerId { get; init; } 
    [JsonPropertyName("cmd")] public string CommandId { get; init; } = string.Empty;
}