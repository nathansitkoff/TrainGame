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

    [Fact]
    public void StartNewGameBuildsAllDecksAndMarket()
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        engine.StartNewGame();

        Assert.NotNull(engine.State.TrainCardDeck);
        Assert.NotNull(engine.State.Market);
        Assert.NotNull(engine.State.DestinationTicketDeck);

        // Market has pulled 5 face-ups from the 110-card train deck.
        // (Could be fewer than 105 remaining if a 3-loco reshuffle fired.)
        Assert.Equal(5, engine.State.Market!.Slots.Count);
        Assert.Equal(110, engine.State.TrainCardDeck!.TotalRemaining + 5);
        Assert.Equal(30, engine.State.DestinationTicketDeck!.RemainingCount);
    }
}
