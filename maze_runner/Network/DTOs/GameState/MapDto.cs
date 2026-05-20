using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record MapDto
{
    [JsonPropertyName("w")] public int Width { get; init; }
    [JsonPropertyName("h")] public int Height { get; init; }
    [JsonPropertyName("top")] public string Topology { get; init; } = string.Empty;
    [JsonPropertyName("items")] public List<DroppedItemDto> DroppedItems { get; init; } = new();
}

public record DroppedItemDto
{
    [JsonPropertyName("r")] public int Row { get; init; }
    [JsonPropertyName("c")] public int Col { get; init; }
    [JsonPropertyName("it")] public ItemDto Item { get; init; } = null!;
}