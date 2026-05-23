using System.Text.Json.Serialization;

namespace maze_runner.Network.DTOs.GameState;

public record EntityDto
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("n")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("sym")] public char Symbol { get; init; }
    
    [JsonPropertyName("r")] public int Row { get; init; }
    [JsonPropertyName("c")] public int Col { get; init; }
    
    [JsonPropertyName("hp")] public int Health { get; init; }
    [JsonPropertyName("mhp")] public int MaxHealth { get; init; }
    [JsonPropertyName("alv")] public bool IsAlive { get; init; }

    [JsonPropertyName("dmg")] public int Damage { get; init; }
    [JsonPropertyName("def")] public int Defense { get; init; }

    [JsonPropertyName("attr")] public AttributesDto Stats { get; init; } = null!;
    [JsonPropertyName("inv")] public InventoryDto? Inventory { get; init; }
}

public record AttributesDto
{
    [JsonPropertyName("s")] public int Strength { get; init; }
    [JsonPropertyName("d")] public int Dexterity { get; init; }
    [JsonPropertyName("r")] public int Resistance { get; init; }
    [JsonPropertyName("st")] public int Stamina { get; init; }
    [JsonPropertyName("l")] public int Luck { get; init; }
    [JsonPropertyName("w")] public int Wisdom { get; init; }
}