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
        if (playerCount < 2 || playerCount > PlayerColorOrder.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(playerCount),
                $"Player count must be between 2 and {PlayerColorOrder.Length}.");
        }

        var trainDeck = TrainCardDeck.BuildShuffled(Config, _random);
        var market = new Market(trainDeck, Config);
        var ticketDeck = new DestinationTicketDeck(_random);

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
        }
        StateChanged?.Invoke();
    }
}
