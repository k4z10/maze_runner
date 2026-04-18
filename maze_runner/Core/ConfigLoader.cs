using System.Text.Json;

namespace maze_runner.Core;

public static class ConfigLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive =  true,
    };

    public static GameConfig Load(string configPath)
    {
        if (!File.Exists(configPath))
        {
            var defaultConfig = new GameConfig();
            Save(configPath, defaultConfig);
            return defaultConfig;
        }

        try
        {
            string jsonString = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<GameConfig>(jsonString, JsonOptions) ?? new GameConfig();
        }
        catch (JsonException e)
        {
            return new GameConfig();
        }
    }

    private static void Save(string configPath, GameConfig config)
    {
        var dir = Path.GetDirectoryName(configPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        
        string jsonString = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(configPath, jsonString);
    }
}