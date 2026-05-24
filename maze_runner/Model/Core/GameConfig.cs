using System.Text.Json.Serialization;

namespace maze_runner.Model.Core;

public record GameConfig()
{
    [JsonPropertyName("playerName")] public string PlayerName { get; init; } = "Unknown Runner";
    [JsonPropertyName("logDirectoryPath")] public string LogDirectoryPath { get; init; } = "./logs/";
}