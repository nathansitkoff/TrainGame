using System;
using System.Linq;
using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class StartingSetupTests
{
    private static GameEngine NewEngineWith(int playerCount)
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        engine.StartNewGame(playerCount);
        return engine;
    }

    [Fact]
    public void StartNewGameCreatesRequestedNumberOfPlayers()
    {
        var engine = NewEngineWith(3);
        Assert.Equal(3, engine.State.Players.Count);
        Assert.Equal(PlayerColor.Red, engine.State.Players[0].Color);
        Assert.Equal(PlayerColor.Blue, engine.State.Players[1].Color);
        Assert.Equal(PlayerColor.Green, engine.State.Players[2].Color);
    }

    [Fact]
    public void EachPlayerReceivesStartingHandAndTrains()
    {
        var cfg = new GameConfig();
        var engine = NewEngineWith(2);
        foreach (var player in engine.State.Players)
        {
            Assert.Equal(cfg.TrainsPerPlayer, player.TrainsRemaining);
            Assert.Equal(cfg.StartingHandSize, player.TotalHandCards);
            Assert.Equal(cfg.StartingTicketsDealt, player.PendingStartingTickets.Count);
            Assert.Empty(player.Tickets);
        }
    }

    [Fact]
    public void PhaseIsChoosingStartingTicketsAfterDeal()
    {
        var engine = NewEngineWith(2);
        Assert.Equal(GamePhase.ChoosingStartingTickets, engine.State.Phase);
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
    }

    [Fact]
    public void TrainDeckPlusMarketPlusHandsEqualsFullDeck()
    {
        var engine = NewEngineWith(4);
        var deck = engine.State.TrainCardDeck!;
        var market = engine.State.Market!;
        int handTotal = engine.State.Players.Sum(p => p.TotalHandCards);
        int marketTotal = market.Slots.Count(s => s is not null);
        Assert.Equal(110, deck.TotalRemaining + marketTotal + handTotal);
    }

    [Fact]
    public void TicketDeckShrinksByDealtStartingTickets()
    {
        var engine = NewEngineWith(3);
        // 30 - 3 players * 3 starting tickets = 21
        Assert.Equal(21, engine.State.DestinationTicketDeck!.RemainingCount);
    }

    [Fact]
    public void KeepStartingTicketsMovesSelectedToPlayerAndDiscardsRest()
    {
        var engine = NewEngineWith(2);
        var player0 = engine.State.Players[0];
        var pending = player0.PendingStartingTickets.ToList();
        var keep = new[] { pending[0].Id, pending[1].Id };

        engine.KeepStartingTickets(playerId: 0, keep);

        Assert.Empty(player0.PendingStartingTickets);
        Assert.Equal(2, player0.Tickets.Count);
        Assert.Contains(pending[0], player0.Tickets);
        Assert.Contains(pending[1], player0.Tickets);
        Assert.DoesNotContain(pending[2], player0.Tickets);
    }

    [Fact]
    public void KeepStartingTicketsAdvancesCurrentPlayer()
    {
        var engine = NewEngineWith(2);
        var p0 = engine.State.Players[0];
        engine.KeepStartingTickets(0, p0.PendingStartingTickets.Take(2).Select(t => t.Id).ToArray());
        Assert.Equal(1, engine.State.CurrentPlayerIndex);
        Assert.Equal(GamePhase.ChoosingStartingTickets, engine.State.Phase);
    }

    [Fact]
    public void AllPlayersChoosingTransitionsToPlayerTurns()
    {
        var engine = NewEngineWith(2);
        for (int i = 0; i < 2; i++)
        {
            var p = engine.State.Players[i];
            var keep = p.PendingStartingTickets.Take(2).Select(t => t.Id).ToArray();
            engine.KeepStartingTickets(i, keep);
        }
        Assert.Equal(GamePhase.PlayerTurns, engine.State.Phase);
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
    }

    [Fact]
    public void KeepingFewerThanMinimumThrows()
    {
        var engine = NewEngineWith(2);
        var p0 = engine.State.Players[0];
        var tooFew = new[] { p0.PendingStartingTickets[0].Id };
        Assert.Throws<InvalidOperationException>(
            () => engine.KeepStartingTickets(0, tooFew));
    }

    [Fact]
    public void KeepingOutOfTurnThrows()
    {
        var engine = NewEngineWith(2);
        var p1 = engine.State.Players[1];
        var keep = p1.PendingStartingTickets.Take(2).Select(t => t.Id).ToArray();
        Assert.Throws<InvalidOperationException>(
            () => engine.KeepStartingTickets(1, keep));
    }

    [Fact]
    public void RejectedStartingTicketsGoToBottomOfDeck()
    {
        var engine = NewEngineWith(2);
        var p0 = engine.State.Players[0];
        var rejected = p0.PendingStartingTickets[2];
        var keep = new[] { p0.PendingStartingTickets[0].Id, p0.PendingStartingTickets[1].Id };
        engine.KeepStartingTickets(0, keep);

        // Drain the remaining tickets; the rejected one should be the very last.
        var ticketDeck = engine.State.DestinationTicketDeck!;
        DestinationTicket? last = null;
        while (ticketDeck.RemainingCount > 0) last = ticketDeck.Draw();
        Assert.Equal(rejected, last);
    }

    [Fact]
    public void StartNewGameWithTwoPlayersUsesDefault()
    {
        // Back-compat with phase 3 tests that called StartNewGame() with no args.
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        engine.StartNewGame();
        Assert.Equal(2, engine.State.Players.Count);
    }
}
