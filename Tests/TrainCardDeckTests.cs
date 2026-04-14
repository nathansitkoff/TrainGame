using System.Collections.Generic;
using System.Linq;
using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class TrainCardDeckTests
{
    [Fact]
    public void ShuffledDeckHasExpectedComposition()
    {
        var deck = TrainCardDeck.BuildShuffled(new GameConfig(), new SeededRandom(1));
        // 8 colors * 12 + 14 locomotives = 110
        Assert.Equal(110, deck.DrawPileCount);
        Assert.Equal(0, deck.DiscardPileCount);

        var drawn = DrainAll(deck);
        Assert.Equal(110, drawn.Count);
        Assert.Equal(14, drawn.Count(c => c == TrainColor.Locomotive));
        foreach (var color in new[]
        {
            TrainColor.Pink, TrainColor.White, TrainColor.Blue, TrainColor.Yellow,
            TrainColor.Orange, TrainColor.Black, TrainColor.Red, TrainColor.Green
        })
        {
            Assert.Equal(12, drawn.Count(c => c == color));
        }
    }

    [Fact]
    public void SameSeedProducesSameDrawOrder()
    {
        var a = TrainCardDeck.BuildShuffled(new GameConfig(), new SeededRandom(42));
        var b = TrainCardDeck.BuildShuffled(new GameConfig(), new SeededRandom(42));
        Assert.Equal(DrainAll(a), DrainAll(b));
    }

    [Fact]
    public void DrawAutoReshufflesFromDiscardWhenDrawPileEmpty()
    {
        var cards = new[]
        {
            TrainColor.Red, TrainColor.Blue, TrainColor.Green
        };
        var deck = TrainCardDeck.FromTopToBottom(new SeededRandom(1), cards);

        // Draw all three and discard them
        for (int i = 0; i < 3; i++)
        {
            deck.Discard(deck.Draw());
        }
        Assert.Equal(0, deck.DrawPileCount);
        Assert.Equal(3, deck.DiscardPileCount);

        // Next draw should trigger reshuffle from discard
        var next = deck.Draw();
        Assert.Contains(next, cards);
        Assert.Equal(2, deck.DrawPileCount);
        Assert.Equal(0, deck.DiscardPileCount);
    }

    [Fact]
    public void FromTopToBottomDrawsInSpecifiedOrder()
    {
        var deck = TrainCardDeck.FromTopToBottom(
            new SeededRandom(1),
            new[] { TrainColor.Red, TrainColor.Blue, TrainColor.Green });
        Assert.Equal(TrainColor.Red, deck.Draw());
        Assert.Equal(TrainColor.Blue, deck.Draw());
        Assert.Equal(TrainColor.Green, deck.Draw());
    }

    private static List<TrainColor> DrainAll(TrainCardDeck deck)
    {
        var list = new List<TrainColor>();
        while (deck.DrawPileCount > 0) list.Add(deck.Draw());
        return list;
    }
}
