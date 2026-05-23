using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record GameStateSnapshotDto
{
    [JsonPropertyName("entities")] public List<EntityDto> Entities { get; init; } = new();
    [JsonPropertyName("map")] public MapDto Map { get; init; } = new();
    [JsonPropertyName("lvl_meta")] public LevelMetaDto? LevelMeta { get; init; }
    [JsonPropertyName("logs")] public List<string>? NewLogs { get; init; }
}