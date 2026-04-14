using Godot;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Phase 2 view: draws the board image centered at native scale, then
/// overlays city dots and route segments computed from <see cref="UsMap"/>.
/// All board-relative positions go through <see cref="ToScreen"/>, which
/// maps normalized [0,1] coordinates onto the current rendered board
/// rectangle. To switch to a fitted/scaled board later, change only the
/// values of <c>_boardOrigin</c> and <c>_boardSize</c>.
/// </summary>
public partial class BoardView : Control
{
    private const string BoardTexturePath = "res://Resources/Board/us_map.jpg";

    private Texture2D? _board;
    private Vector2 _boardOrigin;
    private Vector2 _boardSize;

    public override void _Ready()
    {
        _board = GD.Load<Texture2D>(BoardTexturePath);
        if (_board is null)
        {
            GD.PrintErr($"Failed to load board texture at {BoardTexturePath}");
        }
        Resized += QueueRedraw;
        RecomputeBoardRect();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            RecomputeBoardRect();
        }
    }

    private void RecomputeBoardRect()
    {
        if (_board is null) return;
        // Phase 2: native scale, centered. Future scale modes only need
        // to assign different values to these two fields.
        _boardSize = _board.GetSize();
        _boardOrigin = ((Size - _boardSize) / 2f).Floor();
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_board is null) return;

        DrawTextureRect(_board, new Rect2(_boardOrigin, _boardSize), tile: false);

        // Routes — thin colored lines on top of the board art so we can
        // visually verify city/route data is wired up correctly.
        foreach (var route in UsMap.Routes)
        {
            var a = ToScreen(UsMap.CityPositions[route.CityA]);
            var b = ToScreen(UsMap.CityPositions[route.CityB]);
            // Offset double routes slightly perpendicular so both halves
            // are visible instead of overlapping into one line.
            if (route.DoublePartnerId is int partnerId)
            {
                var dir = (b - a).Normalized();
                var perp = new Vector2(-dir.Y, dir.X) * 3f;
                if (route.Id < partnerId) { a += perp; b += perp; }
                else { a -= perp; b -= perp; }
            }
            DrawLine(a, b, GameColors.ForRoute(route.Color), width: 2f, antialiased: true);
        }

        // City dots — white halo + colored center for visibility on any background.
        foreach (var (_, pos) in UsMap.CityPositions)
        {
            var p = ToScreen(pos);
            DrawCircle(p, 5f, Colors.White);
            DrawCircle(p, 3f, Colors.Black);
        }
    }

    private Vector2 ToScreen(NormPos p) =>
        _boardOrigin + new Vector2(p.X * _boardSize.X, p.Y * _boardSize.Y);
}
