using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record InventoryDto
{
    [JsonPropertyName("lh")] public ItemDto? LeftHand { get; init; }
    [JsonPropertyName("rh")] public ItemDto? RightHand { get; init; }
    [JsonPropertyName("items")] public List<ItemDto> Items { get; init; } = new();
    [JsonPropertyName("gld")] public int Gold { get; init; }
    [JsonPropertyName("gem")] public int Coins { get; init; }
}