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
    /// <summary>
    /// How many card "picks" the current player has left this turn.
    /// Reset to 2 at the start of each player's turn; a non-locomotive
    /// pick spends 1, a face-up locomotive pick spends 2.
    /// </summary>
    public int CurrentTurnPicksRemaining { get; set; }
    public List<PlayerState> Players { get; set; } = new();
    public TrainCardDeck? TrainCardDeck { get; set; }
    public Market? Market { get; set; }
    public DestinationTicketDeck? DestinationTicketDeck { get; set; }
}
