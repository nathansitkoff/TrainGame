namespace TrainGame.Model;

/// <summary>
/// The full mutable game state. Currently a stub; phases 2+ will add
/// players, decks, board ownership, etc.
/// </summary>
public sealed record GameState
{
    public bool Started { get; init; }
}
