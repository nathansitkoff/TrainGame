using Godot;
using TrainGame.Logic;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Left sidebar: draw pile + 5 face-up market slots + ticket deck count.
/// The deck and market slots are clickable buttons that submit draw
/// actions to <see cref="GameEngine"/>; buttons enable/disable based on
/// whether the action is legal for the current player and pick count.
/// </summary>
public partial class HudPanel : PanelContainer
{
    private const int MarketSize = 5;

    private GameEngine? _engine;
    private Label _turnLabel = null!;
    private Label _picksLabel = null!;
    private Button _drawDeckButton = null!;
    private Label _discardLabel = null!;
    private Label _ticketDeckLabel = null!;
    private readonly Button[] _marketButtons = new Button[MarketSize];

    public override void _Ready()
    {
        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 10);
        AddChild(root);

        var title = new Label { Text = "Market / Decks" };
        title.AddThemeFontSizeOverride("font_size", 20);
        root.AddChild(title);

        _turnLabel = new Label();
        _turnLabel.AddThemeFontSizeOverride("font_size", 14);
        root.AddChild(_turnLabel);

        _picksLabel = new Label();
        _picksLabel.AddThemeFontSizeOverride("font_size", 14);
        root.AddChild(_picksLabel);

        _drawDeckButton = new Button
        {
            Text = "Draw from deck",
            CustomMinimumSize = new Vector2(0, 36),
        };
        _drawDeckButton.Pressed += OnDrawDeckPressed;
        root.AddChild(_drawDeckButton);

        _discardLabel = new Label();
        _discardLabel.AddThemeFontSizeOverride("font_size", 12);
        root.AddChild(_discardLabel);

        var marketHeader = new Label { Text = "Face-up cards (click to take)" };
        marketHeader.AddThemeFontSizeOverride("font_size", 14);
        root.AddChild(marketHeader);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 6);
        root.AddChild(hbox);

        for (int i = 0; i < MarketSize; i++)
        {
            int slot = i;
            var btn = new Button
            {
                CustomMinimumSize = new Vector2(54, 72),
                Text = "-",
            };
            btn.Pressed += () => OnMarketSlotPressed(slot);
            hbox.AddChild(btn);
            _marketButtons[i] = btn;
        }

        _ticketDeckLabel = new Label();
        _ticketDeckLabel.AddThemeFontSizeOverride("font_size", 12);
        root.AddChild(_ticketDeckLabel);
    }

    public void Attach(GameEngine engine)
    {
        _engine = engine;
        engine.StateChanged += Refresh;
        Refresh();
    }

    private void OnDrawDeckPressed()
    {
        if (_engine is null) return;
        _engine.DrawFromDeck();
    }

    private void OnMarketSlotPressed(int slot)
    {
        if (_engine is null) return;
        _engine.TakeFromMarket(slot);
    }

    private void Refresh()
    {
        if (_engine is null) return;
        var state = _engine.State;

        if (!state.Started || state.TrainCardDeck is null || state.Market is null || state.DestinationTicketDeck is null)
        {
            _turnLabel.Text = "Turn: -";
            _picksLabel.Text = "Picks remaining: -";
            _drawDeckButton.Text = "Draw from deck";
            _drawDeckButton.Disabled = true;
            _discardLabel.Text = "Discard pile: -";
            _ticketDeckLabel.Text = "Ticket deck: -";
            foreach (var btn in _marketButtons)
            {
                btn.Text = "-";
                btn.Disabled = true;
                ApplySlotStyle(btn, bg: new Color(0.15f, 0.15f, 0.15f));
            }
            return;
        }

        var current = state.Players[state.CurrentPlayerIndex];
        _turnLabel.Text = state.Phase switch
        {
            GamePhase.PlayerTurns => $"Turn: Player {current.Id + 1} ({current.Color})",
            GamePhase.ChoosingStartingTickets => "Choosing starting tickets",
            _ => $"Phase: {state.Phase}",
        };
        _picksLabel.Text = state.Phase == GamePhase.PlayerTurns
            ? $"Picks remaining: {state.CurrentTurnPicksRemaining}"
            : "Picks remaining: -";

        bool drawingAllowed = state.Phase == GamePhase.PlayerTurns
            && state.CurrentTurnPicksRemaining > 0;

        _drawDeckButton.Text = $"Draw from deck ({state.TrainCardDeck.DrawPileCount})";
        _drawDeckButton.Disabled = !drawingAllowed || state.TrainCardDeck.TotalRemaining == 0;

        _discardLabel.Text = $"Discard pile: {state.TrainCardDeck.DiscardPileCount}";
        _ticketDeckLabel.Text = $"Ticket deck:  {state.DestinationTicketDeck.RemainingCount}";

        for (int i = 0; i < MarketSize; i++)
        {
            var btn = _marketButtons[i];
            var slot = state.Market.Slots[i];
            if (slot is TrainColor c)
            {
                btn.Text = GameColors.TrainLabel(c);
                ApplySlotStyle(btn, bg: GameColors.ForTrain(c));
                bool locoBlocked = c == TrainColor.Locomotive && state.CurrentTurnPicksRemaining < 2;
                btn.Disabled = !drawingAllowed || locoBlocked;
            }
            else
            {
                btn.Text = "-";
                ApplySlotStyle(btn, bg: new Color(0.15f, 0.15f, 0.15f));
                btn.Disabled = true;
            }
        }
    }

    private static void ApplySlotStyle(Button btn, Color bg)
    {
        var style = new StyleBoxFlat
        {
            BgColor = bg,
            BorderColor = new Color(0.1f, 0.1f, 0.1f),
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderWidthTop = 2,
            BorderWidthBottom = 2,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4,
        };
        btn.AddThemeStyleboxOverride("normal", style);
        btn.AddThemeStyleboxOverride("hover", style);
        btn.AddThemeStyleboxOverride("pressed", style);
        btn.AddThemeStyleboxOverride("disabled",
            new StyleBoxFlat
            {
                BgColor = new Color(bg.R * 0.4f, bg.G * 0.4f, bg.B * 0.4f),
                BorderColor = new Color(0.1f, 0.1f, 0.1f),
                BorderWidthLeft = 2,
                BorderWidthRight = 2,
                BorderWidthTop = 2,
                BorderWidthBottom = 2,
                CornerRadiusTopLeft = 4,
                CornerRadiusTopRight = 4,
                CornerRadiusBottomLeft = 4,
                CornerRadiusBottomRight = 4,
            });

        // Use contrasting text color for dark slot backgrounds
        float luminance = 0.299f * bg.R + 0.587f * bg.G + 0.114f * bg.B;
        btn.AddThemeColorOverride("font_color",
            luminance < 0.5f ? Colors.White : Colors.Black);
        btn.AddThemeColorOverride("font_disabled_color",
            luminance < 0.5f ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.2f, 0.2f, 0.2f));
    }
}
