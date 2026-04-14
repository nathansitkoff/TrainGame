using System.Collections.Generic;
using TrainGame.Logic;

namespace TrainGame.Model;

/// <summary>
/// The full game state. Built fresh each time <see cref="GameEngine.StartNewGame"/>
/// runs. The deck/market/player objects inside are mutable; GameState itself
/// just bundles them so the View can pull everything from one place.
/// </summary>
public sealed class GameState
{
    public bool Started { get; set; }
    public GamePhase Phase { get; set; } = GamePhase.NotStarted;
    public int CurrentPlayerIndex { get; set; }
    public List<PlayerState> Players { get; set; } = new();
    public TrainCardDeck? TrainCardDeck { get; set; }
    public Market? Market { get; set; }
    public DestinationTicketDeck? DestinationTicketDeck { get; set; }
}
