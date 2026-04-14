using System;
using System.Collections.Generic;
using System.Linq;
using TrainGame.Model;

namespace TrainGame.Logic;

/// <summary>
/// The Controller in MVC. Owns the GameState, validates and applies actions
/// from the View, and raises StateChanged so views can redraw.
/// Pure C#: no Godot references — fully unit-testable.
/// </summary>
public sealed class GameEngine
{
    private const int PicksPerTurn = 2;

    private static readonly PlayerColor[] PlayerColorOrder =
    {
        PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green,
        PlayerColor.Yellow, PlayerColor.Black,
    };

    private readonly IRandom _random;

    public GameConfig Config { get; }
    public GameState State { get; private set; } = new();

    public event Action? StateChanged;

    public GameEngine(GameConfig config, IRandom random)
    {
        Config = config;
        _random = random;
    }

    public void StartNewGame(int playerCount = 2)
    {
        var trainDeck = TrainCardDeck.BuildShuffled(Config, _random);
        var ticketDeck = new DestinationTicketDeck(_random);
        BuildInitialState(playerCount, trainDeck, ticketDeck);
    }

    /// <summary>
    /// Test entry point: lets a unit test inject a deterministic
    /// <see cref="TrainCardDeck"/> (e.g. via
    /// <see cref="TrainCardDeck.FromTopToBottom"/>) so it can predict
    /// the initial market and starting hands.
    /// </summary>
    public void StartNewGameForTesting(int playerCount, TrainCardDeck trainDeck, DestinationTicketDeck ticketDeck)
    {
        BuildInitialState(playerCount, trainDeck, ticketDeck);
    }

    private void BuildInitialState(int playerCount, TrainCardDeck trainDeck, DestinationTicketDeck ticketDeck)
    {
        if (playerCount < 2 || playerCount > PlayerColorOrder.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(playerCount),
                $"Player count must be between 2 and {PlayerColorOrder.Length}.");
        }

        var market = new Market(trainDeck, Config);

        var players = new List<PlayerState>(playerCount);
        for (int i = 0; i < playerCount; i++)
        {
            var player = new PlayerState(i, PlayerColorOrder[i], Config.TrainsPerPlayer);
            for (int c = 0; c < Config.StartingHandSize; c++)
            {
                player.AddCard(trainDeck.Draw());
            }
            for (int t = 0; t < Config.StartingTicketsDealt; t++)
            {
                player.PendingStartingTickets.Add(ticketDeck.Draw());
            }
            players.Add(player);
        }

        State = new GameState
        {
            Started = true,
            Phase = GamePhase.ChoosingStartingTickets,
            CurrentPlayerIndex = 0,
            CurrentTurnPicksRemaining = PicksPerTurn,
            Players = players,
            TrainCardDeck = trainDeck,
            Market = market,
            DestinationTicketDeck = ticketDeck,
        };
        StateChanged?.Invoke();
    }

    /// <summary>
    /// Player with <paramref name="playerId"/> commits to keeping the
    /// given starting tickets; the rest return to the bottom of the
    /// ticket deck. Advances the current-player pointer and transitions
    /// to <see cref="GamePhase.PlayerTurns"/> once everyone has chosen.
    /// </summary>
    public void KeepStartingTickets(int playerId, IReadOnlyCollection<int> keptTicketIds)
    {
        if (State.Phase != GamePhase.ChoosingStartingTickets)
        {
            throw new InvalidOperationException(
                $"Cannot choose starting tickets in phase {State.Phase}.");
        }
        if (State.CurrentPlayerIndex != playerId)
        {
            throw new InvalidOperationException(
                $"It is player {State.CurrentPlayerIndex}'s turn to choose, not player {playerId}.");
        }
        if (keptTicketIds.Count < Config.StartingTicketsKeepMin)
        {
            throw new InvalidOperationException(
                $"Must keep at least {Config.StartingTicketsKeepMin} starting tickets.");
        }

        var player = State.Players[playerId];
        var pendingIds = player.PendingStartingTickets.Select(t => t.Id).ToHashSet();
        foreach (var id in keptTicketIds)
        {
            if (!pendingIds.Contains(id))
            {
                throw new InvalidOperationException(
                    $"Ticket id {id} is not among player {playerId}'s pending starting tickets.");
            }
        }

        foreach (var ticket in player.PendingStartingTickets.ToList())
        {
            if (keptTicketIds.Contains(ticket.Id))
            {
                player.Tickets.Add(ticket);
            }
            else
            {
                State.DestinationTicketDeck!.DiscardToBottom(ticket);
            }
        }
        player.PendingStartingTickets.Clear();

        int next = State.CurrentPlayerIndex + 1;
        if (next < State.Players.Count)
        {
            State.CurrentPlayerIndex = next;
        }
        else
        {
            State.Phase = GamePhase.PlayerTurns;
            State.CurrentPlayerIndex = 0;
            State.CurrentTurnPicksRemaining = PicksPerTurn;
        }
        StateChanged?.Invoke();
    }

    /// <summary>Current player draws the top card of the train deck. Spends 1 pick.</summary>
    public void DrawFromDeck()
    {
        RequireDrawingPhase();
        if (State.TrainCardDeck!.TotalRemaining == 0)
        {
            throw new InvalidOperationException("Train card deck is empty.");
        }

        var card = State.TrainCardDeck.Draw();
        State.Players[State.CurrentPlayerIndex].AddCard(card);
        SpendPicks(1);
        StateChanged?.Invoke();
    }

    /// <summary>
    /// Current player takes the face-up card in <paramref name="slot"/>.
    /// A non-locomotive spends 1 pick; a face-up locomotive spends 2 and
    /// can only be taken as the player's first pick of the turn.
    /// </summary>
    public void TakeFromMarket(int slot)
    {
        RequireDrawingPhase();
        var current = State.Market!.Slots[slot]
            ?? throw new InvalidOperationException($"Market slot {slot} is empty.");
        bool isLocomotive = current == TrainColor.Locomotive;

        if (isLocomotive && State.CurrentTurnPicksRemaining < PicksPerTurn)
        {
            throw new InvalidOperationException(
                "A face-up locomotive can only be taken as your first pick of the turn.");
        }

        var taken = State.Market.TakeFaceUp(slot);
        State.Players[State.CurrentPlayerIndex].AddCard(taken);
        SpendPicks(isLocomotive ? PicksPerTurn : 1);
        StateChanged?.Invoke();
    }

    private void RequireDrawingPhase()
    {
        if (State.Phase != GamePhase.PlayerTurns)
        {
            throw new InvalidOperationException(
                $"Cannot draw cards in phase {State.Phase}.");
        }
        if (State.CurrentTurnPicksRemaining <= 0)
        {
            throw new InvalidOperationException("No picks remaining this turn.");
        }
    }

    private void SpendPicks(int count)
    {
        State.CurrentTurnPicksRemaining -= count;
        if (State.CurrentTurnPicksRemaining <= 0)
        {
            AdvanceTurn();
        }
    }

    private void AdvanceTurn()
    {
        State.CurrentPlayerIndex = (State.CurrentPlayerIndex + 1) % State.Players.Count;
        State.CurrentTurnPicksRemaining = PicksPerTurn;
    }
}
