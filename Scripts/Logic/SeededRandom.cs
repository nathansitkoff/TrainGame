using System;

namespace TrainGame.Logic;

public sealed class SeededRandom : IRandom
{
    private readonly Random _random;

    public SeededRandom(int seed)
    {
        _random = new Random(seed);
    }

    public int Next(int maxExclusive) => _random.Next(maxExclusive);
}
