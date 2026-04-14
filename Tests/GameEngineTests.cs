using TrainGame.Logic;
using TrainGame.Model;
using Xunit;

namespace TrainGame.Tests;

public class GameEngineTests
{
    [Fact]
    public void NewEngineHasUnstartedState()
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        Assert.False(engine.State.Started);
    }

    [Fact]
    public void StartNewGameFiresStateChangedEventAndMarksStarted()
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        int eventCount = 0;
        engine.StateChanged += () => eventCount++;

        engine.StartNewGame();

        Assert.Equal(1, eventCount);
        Assert.True(engine.State.Started);
    }
}
