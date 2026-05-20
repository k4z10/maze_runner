using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record GameStateSnapshotDto
{
    [JsonPropertyName("entities")] public List<EntityDto> Entities { get; init; }
    [JsonPropertyName("players")] public List<PlayerDto> Players { get; init; }
    [JsonPropertyName("map")] public MapDto Map { get; init; }
    [JsonPropertyName("lvl_meta")] public LevelMetaDto LevelMeta { get; init; }
    [JsonPropertyName("logs")] public string RecentLogs { get; init; } = string.Empty;
}