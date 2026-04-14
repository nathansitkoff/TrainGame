namespace TrainGame.Model;

/// <summary>
/// The color required to claim a board route.
/// Distinct from <see cref="TrainColor"/> because routes can be Gray
/// (any single color) but there is no "Gray" train card, and there are
/// no "Locomotive" routes.
/// </summary>
public enum RouteColor
{
    Gray,
    Pink,
    White,
    Blue,
    Yellow,
    Orange,
    Black,
    Red,
    Green
}
