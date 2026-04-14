using System;
using System.Collections.Generic;
using TrainGame.Model;

namespace TrainGame.Logic;

/// <summary>
/// The 5-card face-up market next to the train card deck. Enforces the
/// "3+ locomotives triggers full reshuffle" rule, including chain
/// reshuffles. Null slots mean the deck + discard are exhausted.
/// </summary>
public sealed class Market
{
    private const int MaxReshuffleChain = 10; // safety cap against pathological pools

    private readonly TrainCardDeck _deck;
    private readonly int _size;
    private readonly int _locoReshuffleThreshold;
    private readonly TrainColor?[] _slots;

    public IReadOnlyList<TrainColor?> Slots => _slots;
    public int Size => _size;

    public Market(TrainCardDeck deck, GameConfig config)
    {
        _deck = deck;
        _size = config.MarketSize;
        _locoReshuffleThreshold = config.MarketLocomotiveReshuffleThreshold;
        _slots = new TrainColor?[_size];
        RefillAndCheckReshuffle();
    }

    /// <summary>
    /// Player takes the face-up card in <paramref name="slot"/>. Refills
    /// from deck and re-checks the locomotive reshuffle rule.
    /// </summary>
    public TrainColor TakeFaceUp(int slot)
    {
        if (slot < 0 || slot >= _size)
        {
            throw new ArgumentOutOfRangeException(nameof(slot));
        }
        if (_slots[slot] is not TrainColor card)
        {
            throw new InvalidOperationException($"Slot {slot} is empty.");
        }
        _slots[slot] = null;
        RefillAndCheckReshuffle();
        return card;
    }

    private void RefillAndCheckReshuffle()
    {
        FillEmptySlots();
        int iterations = 0;
        while (CountLocomotives() >= _locoReshuffleThreshold
               && _deck.TotalRemaining > 0
               && iterations < MaxReshuffleChain)
        {
            // Discard every face-up and redraw from the deck.
            for (int i = 0; i < _size; i++)
            {
                if (_slots[i] is TrainColor c)
                {
                    _deck.Discard(c);
                    _slots[i] = null;
                }
            }
            FillEmptySlots();
            iterations++;
        }
    }

    private void FillEmptySlots()
    {
        for (int i = 0; i < _size; i++)
        {
            if (_slots[i] is null && _deck.TotalRemaining > 0)
            {
                _slots[i] = _deck.Draw();
            }
        }
    }

    private int CountLocomotives()
    {
        int count = 0;
        for (int i = 0; i < _size; i++)
        {
            if (_slots[i] == TrainColor.Locomotive) count++;
        }
        return count;
    }
}
