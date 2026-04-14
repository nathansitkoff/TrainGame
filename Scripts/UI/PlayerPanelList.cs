using System.Collections.Generic;
using Godot;
using TrainGame.Logic;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Right-side sidebar listing every player's color, score, trains,
/// hand size, and ticket count. The current player is highlighted.
/// </summary>
public partial class PlayerPanelList : PanelContainer
{
    private GameEngine? _engine;
    private VBoxContainer _root = null!;
    private readonly List<PlayerRow> _rows = new();

    private sealed class PlayerRow
    {
        public PanelContainer Panel = null!;
        public Label Name = null!;
        public Label Stats = null!;
        public ColorRect Swatch = null!;
    }

    public override void _Ready()
    {
        _root = new VBoxContainer();
        _root.AddThemeConstantOverride("separation", 10);
        AddChild(_root);

        var header = new Label { Text = "Players" };
        header.AddThemeFontSizeOverride("font_size", 20);
        _root.AddChild(header);
    }

    public void Attach(GameEngine engine)
    {
        _engine = engine;
        engine.StateChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        if (_engine is null) return;
        var players = _engine.State.Players;

        // Rebuild if the player count changed
        while (_rows.Count > players.Count)
        {
            var last = _rows[^1];
            last.Panel.QueueFree();
            _rows.RemoveAt(_rows.Count - 1);
        }
        while (_rows.Count < players.Count)
        {
            _rows.Add(CreateRow());
        }

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var row = _rows[i];
            row.Name.Text = $"Player {player.Id + 1}  ({player.Color})";
            row.Stats.Text =
                $"Score: {player.Score}   Trains: {player.TrainsRemaining}\n" +
                $"Cards: {player.TotalHandCards}   Tickets: {player.Tickets.Count}\n" +
                FormatHand(player);
            row.Swatch.Color = PlayerColorToGodot(player.Color);

            bool isCurrent =
                _engine.State.CurrentPlayerIndex == i &&
                (_engine.State.Phase == GamePhase.PlayerTurns ||
                 _engine.State.Phase == GamePhase.ChoosingStartingTickets);
            var style = new StyleBoxFlat
            {
                BgColor = new Color(0.15f, 0.15f, 0.18f),
                BorderColor = isCurrent ? new Color(1f, 0.9f, 0.2f) : new Color(0.3f, 0.3f, 0.3f),
                BorderWidthLeft = 3,
                BorderWidthRight = 3,
                BorderWidthTop = 3,
                BorderWidthBottom = 3,
                ContentMarginLeft = 8,
                ContentMarginRight = 8,
                ContentMarginTop = 6,
                ContentMarginBottom = 6,
            };
            row.Panel.AddThemeStyleboxOverride("panel", style);
        }
    }

    private PlayerRow CreateRow()
    {
        var panel = new PanelContainer();
        _root.AddChild(panel);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 10);
        panel.AddChild(hbox);

        var swatch = new ColorRect
        {
            CustomMinimumSize = new Vector2(20, 48),
        };
        hbox.AddChild(swatch);

        var textBox = new VBoxContainer();
        textBox.AddThemeConstantOverride("separation", 2);
        hbox.AddChild(textBox);

        var name = new Label();
        name.AddThemeFontSizeOverride("font_size", 16);
        textBox.AddChild(name);

        var stats = new Label();
        stats.AddThemeFontSizeOverride("font_size", 12);
        textBox.AddChild(stats);

        return new PlayerRow
        {
            Panel = panel,
            Name = name,
            Stats = stats,
            Swatch = swatch,
        };
    }

    private static string FormatHand(PlayerState p)
    {
        if (p.TotalHandCards == 0) return "Hand: (empty)";
        var parts = new System.Collections.Generic.List<string>();
        foreach (var color in new[]
        {
            TrainColor.Pink, TrainColor.White, TrainColor.Blue, TrainColor.Yellow,
            TrainColor.Orange, TrainColor.Black, TrainColor.Red, TrainColor.Green,
            TrainColor.Locomotive,
        })
        {
            if (p.Hand.TryGetValue(color, out int n) && n > 0)
            {
                var label = color == TrainColor.Locomotive ? "L" : color.ToString()[..1];
                parts.Add($"{label}:{n}");
            }
        }
        return "Hand: " + string.Join(" ", parts);
    }

    private static Color PlayerColorToGodot(PlayerColor c) => c switch
    {
        PlayerColor.Red    => new Color(0.90f, 0.15f, 0.15f),
        PlayerColor.Blue   => new Color(0.20f, 0.45f, 0.90f),
        PlayerColor.Green  => new Color(0.15f, 0.65f, 0.25f),
        PlayerColor.Yellow => new Color(0.98f, 0.85f, 0.10f),
        PlayerColor.Black  => new Color(0.1f, 0.1f, 0.1f),
        _                  => Colors.Magenta,
    };
}
