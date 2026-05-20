namespace maze_runner.Network.DTOs.GameState;
using System.Text.Json.Serialization;

public record LevelMetaDto
{
    [JsonPropertyName("name")] public string Name { get; init; }
    [JsonPropertyName("description")] public string Description { get; init; } = string.Empty;
    [JsonPropertyName("cmd")] public List<CommandBindingDto> Commands { get; init; } = [];
}