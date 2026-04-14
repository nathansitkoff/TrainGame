using System.Linq;
using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class MarketTests
{
    private static readonly TrainColor L = TrainColor.Locomotive;
    private static readonly TrainColor R = TrainColor.Red;
    private static readonly TrainColor B = TrainColor.Blue;
    private static readonly TrainColor G = TrainColor.Green;
    private static readonly TrainColor Y = TrainColor.Yellow;
    private static readonly TrainColor W = TrainColor.White;
    private static readonly TrainColor P = TrainColor.Pink;
    private static readonly TrainColor O = TrainColor.Orange;

    [Fact]
    public void InitialMarketFillsFiveSlots()
    {
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { R, B, G, Y, W, R, B, G, Y, W });
        var market = new Market(deck, new GameConfig());
        Assert.Equal(5, market.Slots.Count);
        Assert.All(market.Slots, s => Assert.NotNull(s));
        Assert.Equal(5, deck.DrawPileCount); // 10 - 5 consumed
    }

    [Fact]
    public void TakeFaceUpRemovesCardAndRefillsFromDeck()
    {
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { R, B, G, Y, W, O });
        var market = new Market(deck, new GameConfig());
        var taken = market.TakeFaceUp(0);
        Assert.Equal(R, taken);
        Assert.Equal(O, market.Slots[0]); // refilled
        Assert.Equal(0, deck.DrawPileCount);
    }

    [Fact]
    public void ThreeLocomotivesOnInitialFillTriggersReshuffle()
    {
        // Top 5 cards are 3 locos + 2 reds → triggers. Next 5 are non-locos.
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { L, L, L, R, R, B, G, Y, W, O });
        var market = new Market(deck, new GameConfig());

        // After reshuffle the market should have 5 non-loco cards
        Assert.Equal(5, market.Slots.Count(s => s is not null));
        Assert.Equal(0, market.Slots.Count(s => s == L));
        // 5 discarded (3L+2R), 5 drawn replacement → deck empty
        Assert.Equal(0, deck.DrawPileCount);
        Assert.Equal(5, deck.DiscardPileCount);
    }

    [Fact]
    public void ChainReshuffleFiresWhenReplacementsAlsoHaveThreeLocos()
    {
        // First fill: 3L + 2R → reshuffle.
        // Second fill: 3L + 2B → reshuffle again.
        // Third fill: 5 non-locos → stop.
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { L, L, L, R, R, L, L, L, B, B, G, Y, W, P, O });
        var market = new Market(deck, new GameConfig());

        Assert.Equal(0, market.Slots.Count(s => s == L));
        Assert.Equal(5, market.Slots.Count(s => s is not null));
        Assert.Equal(0, deck.DrawPileCount);
        Assert.Equal(10, deck.DiscardPileCount); // 3L+2R + 3L+2B
    }

    [Fact]
    public void TwoLocomotivesDoesNotTriggerReshuffle()
    {
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { L, L, R, G, B });
        var market = new Market(deck, new GameConfig());

        Assert.Equal(2, market.Slots.Count(s => s == L));
        Assert.Equal(0, deck.DiscardPileCount);
    }

    [Fact]
    public void TakingFaceUpCardThatCausesThreeLocosTriggersReshuffle()
    {
        // Initial: R, L, L, G, Y. Top of deck next: L, then 5 non-locos.
        // Player takes slot 0 (R). Refill pulls L → market becomes L,L,L,G,Y → reshuffle.
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { R, L, L, G, Y, L, B, W, P, O, R });
        var market = new Market(deck, new GameConfig());
        Assert.Equal(2, market.Slots.Count(s => s == L)); // pre-condition

        market.TakeFaceUp(0);

        Assert.Equal(0, market.Slots.Count(s => s == L));
        Assert.Equal(5, market.Slots.Count(s => s is not null));
    }
}
