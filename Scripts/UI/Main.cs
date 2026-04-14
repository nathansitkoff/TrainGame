using System.Linq;
using Godot;
using TrainGame.Logic;
using TrainGame.Model;

namespace TrainGame.UI;

public partial class Main : Control
{
    private GameEngine _engine = null!;

    public override void _Ready()
    {
        var configJson = LoadConfigJson();
        var config = GameConfigLoader.FromJson(configJson);
        _engine = new GameEngine(config, new SeededRandom(1));
        _engine.StateChanged += OnStateChanged;
        _engine.StartNewGame();
    }

    private void OnStateChanged()
    {
        var label = GetNode<Label>("DebugLabel");
        label.Text = BuildDebugText(_engine.Config, _engine.State);
    }

    private static string LoadConfigJson()
    {
        using var file = Godot.FileAccess.Open("res://Data/game_config.json", Godot.FileAccess.ModeFlags.Read);
        if (file is null)
        {
            GD.PrintErr("Failed to open res://Data/game_config.json");
            return "{}";
        }
        return file.GetAsText();
    }

    private static string BuildDebugText(GameConfig cfg, GameState state)
    {
        var scoring = string.Join(", ",
            cfg.RouteScoring.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}"));

        return "Ticket to Ride — Phase 1 (skeleton)\n\n" +
               $"Game started: {state.Started}\n\n" +
               "Config loaded from res://Data/game_config.json\n" +
               $"  Trains per player:         {cfg.TrainsPerPlayer}\n" +
               $"  Starting hand size:        {cfg.StartingHandSize}\n" +
               $"  Train cards per color:     {cfg.TrainCardsPerColor}\n" +
               $"  Locomotive cards:          {cfg.LocomotiveCardCount}\n" +
               $"  Market size:               {cfg.MarketSize}\n" +
               $"  Loco reshuffle threshold:  {cfg.MarketLocomotiveReshuffleThreshold}\n" +
               $"  Starting tickets dealt:    {cfg.StartingTicketsDealt} (keep >= {cfg.StartingTicketsKeepMin})\n" +
               $"  Longest route bonus:       {cfg.LongestRouteBonus}\n" +
               $"  Route scoring (len:pts):   {scoring}";
    }
}
