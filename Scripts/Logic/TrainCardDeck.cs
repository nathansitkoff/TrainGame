using System.Collections.Generic;
using TrainGame.Model;

namespace TrainGame.Logic;

/// <summary>
/// The draw + discard piles of train car cards. Deterministic given an
/// injected <see cref="IRandom"/>. Draw auto-reshuffles the discard back
/// into the draw pile when the draw pile is empty.
/// </summary>
public sealed class TrainCardDeck
{
    private static readonly TrainColor[] CardColors =
    {
        TrainColor.Pink, TrainColor.White, TrainColor.Blue, TrainColor.Yellow,
        TrainColor.Orange, TrainColor.Black, TrainColor.Red, TrainColor.Green,
    };

    private readonly IRandom _random;
    private readonly List<TrainColor> _draw;
    private readonly List<TrainColor> _discard = new();

    public int DrawPileCount => _draw.Count;
    public int DiscardPileCount => _discard.Count;
    public int TotalRemaining => _draw.Count + _discard.Count;

    /// <summary>
    /// Constructs a deck from an arbitrary draw-pile list. The LAST
    /// element of <paramref name="drawPile"/> is the top of the pile
    /// (drawn first). Use <see cref="BuildShuffled"/> for real games or
    /// <see cref="FromTopToBottom"/> for deterministic tests.
    /// </summary>
    public TrainCardDeck(IEnumerable<TrainColor> drawPile, IRandom random)
    {
        _random = random;
        _draw = new List<TrainColor>(drawPile);
    }

    public static TrainCardDeck BuildShuffled(GameConfig config, IRandom random)
    {
        var cards = new List<TrainColor>();
        foreach (var color in CardColors)
        {
            for (int i = 0; i < config.TrainCardsPerColor; i++)
            {
                cards.Add(color);
            }
        }
        for (int i = 0; i < config.LocomotiveCardCount; i++)
        {
            cards.Add(TrainColor.Locomotive);
        }
        Shuffle(cards, random);
        return new TrainCardDeck(cards, random);
    }

    /// <summary>
    /// Test helper: builds a deck where <paramref name="topToBottom"/>[0]
    /// is drawn first, [1] second, and so on.
    /// </summary>
    public static TrainCardDeck FromTopToBottom(IRandom random, TrainColor[] topToBottom)
    {
        var list = new List<TrainColor>(topToBottom.Length);
        for (int i = topToBottom.Length - 1; i >= 0; i--)
        {
            list.Add(topToBottom[i]);
        }
        return new TrainCardDeck(list, random);
    }

    public TrainColor Draw()
    {
        if (_draw.Count == 0)
        {
            ReshuffleDiscardIntoDraw();
        }
        if (_draw.Count == 0)
        {
            throw new System.InvalidOperationException("Both piles are empty.");
        }
        var top = _draw[^1];
        _draw.RemoveAt(_draw.Count - 1);
        return top;
    }

    public void Discard(TrainColor card)
    {
        _discard.Add(card);
    }

    private void ReshuffleDiscardIntoDraw()
    {
        if (_discard.Count == 0) return;
        _draw.AddRange(_discard);
        _discard.Clear();
        Shuffle(_draw, _random);
    }

    private static void Shuffle(List<TrainColor> list, IRandom random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
