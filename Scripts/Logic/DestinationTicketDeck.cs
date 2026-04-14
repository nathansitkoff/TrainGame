using System.Collections.Generic;
using TrainGame.Model;

namespace TrainGame.Logic;

/// <summary>
/// The destination ticket draw pile. Shuffled on construction.
/// Rejected tickets (kept by nobody) are returned to the bottom of the pile.
/// </summary>
public sealed class DestinationTicketDeck
{
    private readonly List<DestinationTicket> _pile;

    public int RemainingCount => _pile.Count;

    public DestinationTicketDeck(IRandom random)
    {
        _pile = new List<DestinationTicket>(UsTickets.All);
        Shuffle(_pile, random);
    }

    public DestinationTicket Draw()
    {
        if (_pile.Count == 0)
        {
            throw new System.InvalidOperationException("Destination ticket deck is empty.");
        }
        var top = _pile[^1];
        _pile.RemoveAt(_pile.Count - 1);
        return top;
    }

    public void DiscardToBottom(DestinationTicket ticket)
    {
        _pile.Insert(0, ticket);
    }

    private static void Shuffle(List<DestinationTicket> list, IRandom random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
