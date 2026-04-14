using System.Collections.Generic;

namespace TrainGame.Model;

/// <summary>
/// Mutable per-player state: hand, score, trains remaining, tickets.
/// Lives inside <see cref="GameState.Players"/>; the engine mutates it
/// in place as actions happen.
/// </summary>
public sealed class PlayerState
{
    public int Id { get; }
    public PlayerColor Color { get; }
    public int TrainsRemaining { get; set; }
    public int Score { get; set; }

    /// <summary>Card counts by color.</summary>
    public Dictionary<TrainColor, int> Hand { get; } = new();

    /// <summary>Destination tickets this player has committed to.</summary>
    public List<DestinationTicket> Tickets { get; } = new();

    /// <summary>
    /// Starting tickets the player has been dealt but not yet committed to
    /// (must keep at least <c>GameConfig.StartingTicketsKeepMin</c>).
    /// Empty once the starting-ticket choice has been made.
    /// </summary>
    public List<DestinationTicket> PendingStartingTickets { get; } = new();

    public PlayerState(int id, PlayerColor color, int startingTrains)
    {
        Id = id;
        Color = color;
        TrainsRemaining = startingTrains;
    }

    public void AddCard(TrainColor color)
    {
        Hand.TryGetValue(color, out int n);
        Hand[color] = n + 1;
    }

    public int TotalHandCards
    {
        get
        {
            int sum = 0;
            foreach (var n in Hand.Values) sum += n;
            return sum;
        }
    }
}
