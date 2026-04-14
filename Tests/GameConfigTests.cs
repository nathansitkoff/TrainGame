using System.IO;
using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class GameConfigTests
{
    [Fact]
    public void DefaultsMatchTicketToRideUsRules()
    {
        var c = new GameConfig();
        Assert.Equal(45, c.TrainsPerPlayer);
        Assert.Equal(4, c.StartingHandSize);
        Assert.Equal(12, c.TrainCardsPerColor);
        Assert.Equal(14, c.LocomotiveCardCount);
        Assert.Equal(5, c.MarketSize);
        Assert.Equal(3, c.MarketLocomotiveReshuffleThreshold);
        Assert.Equal(3, c.StartingTicketsDealt);
        Assert.Equal(2, c.StartingTicketsKeepMin);
        Assert.Equal(10, c.LongestRouteBonus);
        Assert.Equal(1, c.RouteScoring[1]);
        Assert.Equal(2, c.RouteScoring[2]);
        Assert.Equal(4, c.RouteScoring[3]);
        Assert.Equal(7, c.RouteScoring[4]);
        Assert.Equal(10, c.RouteScoring[5]);
        Assert.Equal(15, c.RouteScoring[6]);
    }

    [Fact]
    public void LoadsOverridesFromJsonString()
    {
        const string json = """
            {
              "trainsPerPlayer": 40,
              "startingHandSize": 5,
              "longestRouteBonus": 15
            }
            """;
        var c = GameConfigLoader.FromJson(json);
        Assert.Equal(40, c.TrainsPerPlayer);
        Assert.Equal(5, c.StartingHandSize);
        Assert.Equal(15, c.LongestRouteBonus);
        // Unspecified values fall back to defaults
        Assert.Equal(12, c.TrainCardsPerColor);
    }

    [Fact]
    public void LoadsActualProjectConfigFile()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "game_config.json");
        Assert.True(File.Exists(path), $"Config file not found at {path}");
        var c = GameConfigLoader.FromJson(File.ReadAllText(path));
        Assert.Equal(45, c.TrainsPerPlayer);
        Assert.Equal(10, c.LongestRouteBonus);
    }
}
