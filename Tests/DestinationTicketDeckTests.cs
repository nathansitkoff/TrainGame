using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class DestinationTicketDeckTests
{
    [Fact]
    public void StartsWithThirtyTickets()
    {
        var deck = new DestinationTicketDeck(new SeededRandom(1));
        Assert.Equal(30, deck.RemainingCount);
    }

    [Fact]
    public void DrawDecreasesCount()
    {
        var deck = new DestinationTicketDeck(new SeededRandom(1));
        deck.Draw();
        Assert.Equal(29, deck.RemainingCount);
    }

    [Fact]
    public void SameSeedProducesSameDrawOrder()
    {
        var a = new DestinationTicketDeck(new SeededRandom(7));
        var b = new DestinationTicketDeck(new SeededRandom(7));
        for (int i = 0; i < 30; i++)
        {
            Assert.Equal(a.Draw(), b.Draw());
        }
    }

    [Fact]
    public void DiscardToBottomSendsTicketToEndOfDeck()
    {
        var deck = new DestinationTicketDeck(new SeededRandom(1));
        var first = deck.Draw();
        deck.DiscardToBottom(first);
        Assert.Equal(30, deck.RemainingCount);
        // Draw the remaining 29; the discarded ticket should be the 30th (last).
        for (int i = 0; i < 29; i++)
        {
            var drawn = deck.Draw();
            Assert.NotEqual(first, drawn);
        }
        Assert.Equal(first, deck.Draw());
    }
}
