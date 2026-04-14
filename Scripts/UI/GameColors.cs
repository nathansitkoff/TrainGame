using Godot;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Shared color palette for rendering train cards and route segments.
/// Centralized so BoardView and HudPanel render consistently.
/// </summary>
public static class GameColors
{
    public static Color ForRoute(RouteColor c) => c switch
    {
        RouteColor.Gray   => new Color(0.5f, 0.5f, 0.5f),
        RouteColor.Pink   => new Color(1.00f, 0.41f, 0.71f),
        RouteColor.White  => new Color(0.95f, 0.95f, 0.95f),
        RouteColor.Blue   => new Color(0.20f, 0.45f, 0.90f),
        RouteColor.Yellow => new Color(0.98f, 0.85f, 0.10f),
        RouteColor.Orange => new Color(1.00f, 0.55f, 0.10f),
        RouteColor.Black  => new Color(0.05f, 0.05f, 0.05f),
        RouteColor.Red    => new Color(0.90f, 0.15f, 0.15f),
        RouteColor.Green  => new Color(0.15f, 0.65f, 0.25f),
        _                 => Colors.Magenta,
    };

    public static Color ForTrain(TrainColor c) => c switch
    {
        TrainColor.Pink       => new Color(1.00f, 0.41f, 0.71f),
        TrainColor.White      => new Color(0.95f, 0.95f, 0.95f),
        TrainColor.Blue       => new Color(0.20f, 0.45f, 0.90f),
        TrainColor.Yellow     => new Color(0.98f, 0.85f, 0.10f),
        TrainColor.Orange     => new Color(1.00f, 0.55f, 0.10f),
        TrainColor.Black      => new Color(0.05f, 0.05f, 0.05f),
        TrainColor.Red        => new Color(0.90f, 0.15f, 0.15f),
        TrainColor.Green      => new Color(0.15f, 0.65f, 0.25f),
        TrainColor.Locomotive => new Color(0.98f, 0.80f, 0.20f),
        _                     => Colors.Magenta,
    };

    public static string TrainLabel(TrainColor c) => c switch
    {
        TrainColor.Locomotive => "LOCO",
        _                     => c.ToString().ToUpperInvariant(),
    };
}
