using System.Linq;
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
        Assert.Equal(GamePhase.NotStarted, engine.State.Phase);
    }

    [Fact]
    public void StartNewGameFiresStateChangedEventAndMarksStarted()
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        int eventCount = 0;
        engine.StateChanged += () => eventCount++;

        engine.StartNewGame(2);

        Assert.Equal(1, eventCount);
        Assert.True(engine.State.Started);
    }

    [Fact]
    public void StartNewGameBuildsAllDecksAndMarket()
    {
        var engine = new GameEngine(new GameConfig(), new SeededRandom(1));
        engine.StartNewGame(2);

        Assert.NotNull(engine.State.TrainCardDeck);
        Assert.NotNull(engine.State.Market);
        Assert.NotNull(engine.State.DestinationTicketDeck);

        // Market fills 5 face-ups; each of 2 players gets a starting hand of 4.
        int handTotal = engine.State.Players.Sum(p => p.TotalHandCards);
        int marketTotal = engine.State.Market!.Slots.Count(s => s is not null);
        Assert.Equal(110, engine.State.TrainCardDeck!.TotalRemaining + marketTotal + handTotal);
    }
}
