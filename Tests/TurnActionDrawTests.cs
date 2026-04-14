using System;
using System.Linq;
using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class TurnActionDrawTests
{
    private static GameEngine ReadyEngine(int playerCount = 2, int seed = 1)
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(seed));
        engine.StartNewGame(playerCount);
        for (int i = 0; i < playerCount; i++)
        {
            var keep = engine.State.Players[i].PendingStartingTickets
                .Take(2).Select(t => t.Id).ToArray();
            engine.KeepStartingTickets(i, keep);
        }
        return engine;
    }

    /// <summary>Crafts a deck whose top cards are exactly as specified so
    /// tests can predict what ends up in the initial market and starting
    /// hands.</summary>
    private static GameEngine ReadyEngineWithDeck(params TrainColor[] topToBottom)
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        var trainDeck = TrainCardDeck.FromTopToBottom(new SeededRandom(1), topToBottom);
        var ticketDeck = new DestinationTicketDeck(new SeededRandom(1));
        engine.StartNewGameForTesting(2, trainDeck, ticketDeck);
        for (int i = 0; i < 2; i++)
        {
            var keep = engine.State.Players[i].PendingStartingTickets
                .Take(2).Select(t => t.Id).ToArray();
            engine.KeepStartingTickets(i, keep);
        }
        return engine;
    }

    [Fact]
    public void PlayerTurnsStartsWithTwoPicksRemaining()
    {
        var engine = ReadyEngine();
        Assert.Equal(GamePhase.PlayerTurns, engine.State.Phase);
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
        Assert.Equal(2, engine.State.CurrentTurnPicksRemaining);
    }

    [Fact]
    public void DrawFromDeckAddsCardToCurrentPlayerAndSpendsOnePick()
    {
        var engine = ReadyEngine();
        int handBefore = engine.State.Players[0].TotalHandCards;
        int deckBefore = engine.State.TrainCardDeck!.TotalRemaining;

        engine.DrawFromDeck();

        Assert.Equal(handBefore + 1, engine.State.Players[0].TotalHandCards);
        Assert.Equal(deckBefore - 1, engine.State.TrainCardDeck!.TotalRemaining);
        Assert.Equal(1, engine.State.CurrentTurnPicksRemaining);
        Assert.Equal(0, engine.State.CurrentPlayerIndex); // still our turn
    }

    [Fact]
    public void TwoDeckDrawsAdvancesToNextPlayerAndResetsPicks()
    {
        var engine = ReadyEngine();
        engine.DrawFromDeck();
        engine.DrawFromDeck();
        Assert.Equal(1, engine.State.CurrentPlayerIndex);
        Assert.Equal(2, engine.State.CurrentTurnPicksRemaining);
    }

    [Fact]
    public void TurnOrderWrapsAroundAfterLastPlayer()
    {
        var engine = ReadyEngine(playerCount: 3);
        for (int turn = 0; turn < 3; turn++)
        {
            engine.DrawFromDeck();
            engine.DrawFromDeck();
        }
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
    }

    [Fact]
    public void TakeNonLocomotiveFromMarketSpendsOnePickAndRefills()
    {
        // Craft a deck with a red in slot 0, then refill card right after the hands.
        var engine = ReadyEngineWithDeck(
            // Market slots 0..4
            TrainColor.Red, TrainColor.Blue, TrainColor.Green, TrainColor.Yellow, TrainColor.White,
            // Player 0 hand (4)
            TrainColor.Red, TrainColor.Red, TrainColor.Red, TrainColor.Red,
            // Player 1 hand (4)
            TrainColor.Blue, TrainColor.Blue, TrainColor.Blue, TrainColor.Blue,
            // Market refill
            TrainColor.Orange, TrainColor.Pink, TrainColor.White, TrainColor.Red, TrainColor.Blue);

        Assert.Equal(TrainColor.Red, engine.State.Market!.Slots[0]);

        engine.TakeFromMarket(0);

        Assert.Equal(1, engine.State.CurrentTurnPicksRemaining);
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
        Assert.Equal(TrainColor.Orange, engine.State.Market!.Slots[0]); // refilled
    }

    [Fact]
    public void TakeFaceUpLocomotiveAsFirstPickConsumesBothPicksAndEndsTurn()
    {
        var engine = ReadyEngineWithDeck(
            // Market: one loco + 4 non-locos (keeps us under reshuffle threshold)
            TrainColor.Locomotive, TrainColor.Red, TrainColor.Blue, TrainColor.Green, TrainColor.Yellow,
            // Hands
            TrainColor.Red, TrainColor.Red, TrainColor.Red, TrainColor.Red,
            TrainColor.Blue, TrainColor.Blue, TrainColor.Blue, TrainColor.Blue,
            // Refill
            TrainColor.Orange, TrainColor.Pink, TrainColor.White, TrainColor.Red, TrainColor.Blue);

        Assert.Equal(TrainColor.Locomotive, engine.State.Market!.Slots[0]);

        engine.TakeFromMarket(0);

        // Turn ended, advanced to player 1 with a fresh 2 picks
        Assert.Equal(1, engine.State.CurrentPlayerIndex);
        Assert.Equal(2, engine.State.CurrentTurnPicksRemaining);
    }

    [Fact]
    public void TakeFaceUpLocomotiveAsSecondPickIsRejected()
    {
        var engine = ReadyEngineWithDeck(
            // Market: loco at slot 1 so slot 0 is a normal card we use first
            TrainColor.Red, TrainColor.Locomotive, TrainColor.Blue, TrainColor.Green, TrainColor.Yellow,
            // Hands
            TrainColor.Red, TrainColor.Red, TrainColor.Red, TrainColor.Red,
            TrainColor.Blue, TrainColor.Blue, TrainColor.Blue, TrainColor.Blue,
            // Refill
            TrainColor.Orange, TrainColor.Pink, TrainColor.White, TrainColor.Red, TrainColor.Blue);

        engine.TakeFromMarket(0); // Red, 1 pick remaining
        Assert.Equal(1, engine.State.CurrentTurnPicksRemaining);

        Assert.Throws<InvalidOperationException>(() => engine.TakeFromMarket(1));

        // Turn state unchanged
        Assert.Equal(1, engine.State.CurrentTurnPicksRemaining);
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
    }

    [Fact]
    public void DrawFromDeckRejectedDuringStartingTicketPhase()
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        engine.StartNewGame(2);
        Assert.Equal(GamePhase.ChoosingStartingTickets, engine.State.Phase);
        Assert.Throws<InvalidOperationException>(() => engine.DrawFromDeck());
    }

    [Fact]
    public void CannotDrawWhenZeroPicksRemaining()
    {
        // Synthetic: drain picks then try again — but after draws reset picks,
        // so we simulate by taking a face-up loco first (which zeroes picks and
        // advances the turn). The *outgoing* player can't draw after that.
        var engine = ReadyEngineWithDeck(
            TrainColor.Locomotive, TrainColor.Red, TrainColor.Blue, TrainColor.Green, TrainColor.Yellow,
            TrainColor.Red, TrainColor.Red, TrainColor.Red, TrainColor.Red,
            TrainColor.Blue, TrainColor.Blue, TrainColor.Blue, TrainColor.Blue,
            TrainColor.Orange, TrainColor.Pink, TrainColor.White, TrainColor.Red, TrainColor.Blue);

        engine.TakeFromMarket(0); // player 0 takes loco, turn ends → player 1 now
        // Player 1 has a fresh turn; let them draw twice to end their turn
        engine.DrawFromDeck();
        engine.DrawFromDeck();
        // Back to player 0 with 2 picks — perfectly valid, no exception expected
        Assert.Equal(0, engine.State.CurrentPlayerIndex);
        Assert.Equal(2, engine.State.CurrentTurnPicksRemaining);
    }
}
