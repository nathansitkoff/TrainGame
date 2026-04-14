using Godot;
using TrainGame.Logic;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Phase 3 view: sidebar showing draw pile count, discard pile count,
/// the 5 face-up market slots, and destination ticket deck count.
/// Subscribes to <see cref="GameEngine.StateChanged"/> to refresh.
/// </summary>
public partial class HudPanel : PanelContainer
{
    private GameEngine? _engine;
    private Label _drawPileLabel = null!;
    private Label _discardPileLabel = null!;
    private Label _ticketDeckLabel = null!;
    private readonly ColorRect[] _marketRects = new ColorRect[5];
    private readonly Label[] _marketLabels = new Label[5];

    public override void _Ready()
    {
        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 12);
        AddChild(root);

        var title = new Label { Text = "Market / Decks" };
        title.AddThemeFontSizeOverride("font_size", 20);
        root.AddChild(title);

        _drawPileLabel = new Label();
        _discardPileLabel = new Label();
        _ticketDeckLabel = new Label();
        root.AddChild(_drawPileLabel);
        root.AddChild(_discardPileLabel);
        root.AddChild(_ticketDeckLabel);

        var marketHeader = new Label { Text = "Face-up cards" };
        marketHeader.AddThemeFontSizeOverride("font_size", 16);
        root.AddChild(marketHeader);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 8);
        root.AddChild(hbox);

        for (int i = 0; i < _marketRects.Length; i++)
        {
            var cell = new VBoxContainer();
            cell.AddThemeConstantOverride("separation", 2);
            hbox.AddChild(cell);

            var rect = new ColorRect
            {
                CustomMinimumSize = new Vector2(48, 64),
                Color = new Color(0.15f, 0.15f, 0.15f),
            };
            cell.AddChild(rect);
            _marketRects[i] = rect;

            var label = new Label { Text = "-" };
            label.AddThemeFontSizeOverride("font_size", 10);
            cell.AddChild(label);
            _marketLabels[i] = label;
        }
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
        var state = _engine.State;
        if (!state.Started || state.TrainCardDeck is null || state.Market is null || state.DestinationTicketDeck is null)
        {
            _drawPileLabel.Text = "Draw pile: -";
            _discardPileLabel.Text = "Discard pile: -";
            _ticketDeckLabel.Text = "Ticket deck: -";
            return;
        }

        _drawPileLabel.Text = $"Draw pile:    {state.TrainCardDeck.DrawPileCount}";
        _discardPileLabel.Text = $"Discard pile: {state.TrainCardDeck.DiscardPileCount}";
        _ticketDeckLabel.Text = $"Ticket deck:  {state.DestinationTicketDeck.RemainingCount}";

        for (int i = 0; i < _marketRects.Length; i++)
        {
            var slot = state.Market.Slots[i];
            if (slot is TrainColor c)
            {
                _marketRects[i].Color = GameColors.ForTrain(c);
                _marketLabels[i].Text = GameColors.TrainLabel(c);
            }
            else
            {
                _marketRects[i].Color = new Color(0.15f, 0.15f, 0.15f);
                _marketLabels[i].Text = "-";
            }
        }
    }
}
