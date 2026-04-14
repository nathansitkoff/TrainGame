using System.Collections.Generic;

namespace TrainGame.Model;

/// <summary>
/// Tunable game constants. Loaded from Data/game_config.json at startup.
/// Any value omitted from the JSON falls back to the default below.
/// </summary>
public sealed record GameConfig
{
    public int TrainsPerPlayer { get; init; } = 45;
    public int StartingHandSize { get; init; } = 4;
    public int TrainCardsPerColor { get; init; } = 12;
    public int LocomotiveCardCount { get; init; } = 14;
    public int MarketSize { get; init; } = 5;
    public int MarketLocomotiveReshuffleThreshold { get; init; } = 3;
    public int StartingTicketsDealt { get; init; } = 3;
    public int StartingTicketsKeepMin { get; init; } = 2;
    public int LongestRouteBonus { get; init; } = 10;

    public IReadOnlyDictionary<int, int> RouteScoring { get; init; } = new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 4 },
        { 4, 7 },
        { 5, 10 },
        { 6, 15 }
    };
}
