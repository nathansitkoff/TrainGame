using System;
using System.Text.Json;
using TrainGame.Model;

namespace TrainGame.Logic;

public static class GameConfigLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static GameConfig FromJson(string json)
    {
        var config = JsonSerializer.Deserialize<GameConfig>(json, Options);
        if (config is null)
        {
            throw new InvalidOperationException("Game config JSON deserialized to null.");
        }
        return config;
    }
}
