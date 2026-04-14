using TrainGame.Logic;

namespace TrainGame.Model;

/// <summary>
/// The full game state. Built fresh each time <see cref="GameEngine.StartNewGame"/>
/// runs. The deck/market objects inside are mutable; the GameState record itself
/// just bundles them so the View can grab everything via a single property.
/// </summary>
public sealed record GameState
{
    public bool Started { get; init; }
    public TrainCardDeck? TrainCardDeck { get; init; }
    public Market? Market { get; init; }
    public DestinationTicketDeck? DestinationTicketDeck { get; init; }
}
