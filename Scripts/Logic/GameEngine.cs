using System;
using TrainGame.Model;

namespace TrainGame.Logic;

/// <summary>
/// The Controller in MVC. Owns the GameState, validates and applies actions
/// from the View, and raises StateChanged so views can redraw.
/// Pure C#: no Godot references — fully unit-testable.
/// </summary>
public sealed class GameEngine
{
    private readonly IRandom _random;

    public GameConfig Config { get; }
    public GameState State { get; private set; } = new();

    public event Action? StateChanged;

    public GameEngine(GameConfig config, IRandom random)
    {
        Config = config;
        _random = random;
    }

    public void StartNewGame()
    {
        State = new GameState { Started = true };
        StateChanged?.Invoke();
    }
}
