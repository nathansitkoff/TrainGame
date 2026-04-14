using TrainGame.Logic;
using Xunit;

namespace TrainGame.Tests;

public class SeededRandomTests
{
    [Fact]
    public void SameSeedProducesSameSequence()
    {
        var a = new SeededRandom(42);
        var b = new SeededRandom(42);
        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(a.Next(1000), b.Next(1000));
        }
    }

    [Fact]
    public void DifferentSeedsProduceDifferentSequences()
    {
        var a = new SeededRandom(1);
        var b = new SeededRandom(2);
        bool anyDifferent = false;
        for (int i = 0; i < 50; i++)
        {
            if (a.Next(1_000_000) != b.Next(1_000_000))
            {
                anyDifferent = true;
                break;
            }
        }
        Assert.True(anyDifferent);
    }
}
