using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class PlayerStateTests
{
    [Fact]
    public void NewPlayerHasExpectedDefaults()
    {
        var p = new PlayerState(id: 0, PlayerColor.Red, startingTrains: 45);
        Assert.Equal(0, p.Id);
        Assert.Equal(PlayerColor.Red, p.Color);
        Assert.Equal(45, p.TrainsRemaining);
        Assert.Equal(0, p.Score);
        Assert.Empty(p.Hand);
        Assert.Empty(p.Tickets);
        Assert.Empty(p.PendingStartingTickets);
    }

    [Fact]
    public void AddCardIncrementsCountInHand()
    {
        var p = new PlayerState(0, PlayerColor.Red, 45);
        p.AddCard(TrainColor.Red);
        p.AddCard(TrainColor.Red);
        p.AddCard(TrainColor.Blue);
        Assert.Equal(2, p.Hand[TrainColor.Red]);
        Assert.Equal(1, p.Hand[TrainColor.Blue]);
        Assert.Equal(3, p.TotalHandCards);
    }
}
