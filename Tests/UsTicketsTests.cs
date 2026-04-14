using System.Linq;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class UsTicketsTests
{
    [Fact]
    public void HasThirtyTickets()
    {
        Assert.Equal(30, UsTickets.All.Count);
    }

    [Fact]
    public void AllTicketsReferenceKnownCities()
    {
        foreach (var ticket in UsTickets.All)
        {
            Assert.True(UsMap.CityPositions.ContainsKey(ticket.CityA));
            Assert.True(UsMap.CityPositions.ContainsKey(ticket.CityB));
            Assert.NotEqual(ticket.CityA, ticket.CityB);
        }
    }

    [Fact]
    public void TicketPointsAreAllPositive()
    {
        Assert.All(UsTickets.All, t => Assert.InRange(t.Points, 1, 30));
    }

    [Fact]
    public void TicketIdsMatchIndex()
    {
        for (int i = 0; i < UsTickets.All.Count; i++)
        {
            Assert.Equal(i, UsTickets.All[i].Id);
        }
    }

    [Fact]
    public void LongestTicketIsSeattleNewYorkWorth22()
    {
        var longest = UsTickets.All.OrderByDescending(t => t.Points).First();
        Assert.Equal(22, longest.Points);
        Assert.True(
            (longest.CityA == City.Seattle && longest.CityB == City.NewYork) ||
            (longest.CityA == City.NewYork && longest.CityB == City.Seattle));
    }
}
