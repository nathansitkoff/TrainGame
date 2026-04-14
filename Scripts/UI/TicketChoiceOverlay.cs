using System.Collections.Generic;
using Godot;
using TrainGame.Logic;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Modal shown during <see cref="GamePhase.ChoosingStartingTickets"/>.
/// Lets the current player pick which starting tickets to keep
/// (must keep at least <c>StartingTicketsKeepMin</c>).
/// </summary>
public partial class TicketChoiceOverlay : Control
{
    private GameEngine? _engine;
    private VBoxContainer _ticketList = null!;
    private Label _titleLabel = null!;
    private Label _instructionLabel = null!;
    private Button _confirmButton = null!;
    private readonly List<CheckBox> _checkBoxes = new();
    private readonly List<int> _ticketIds = new();

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Stop;
        Visible = false;

        var dim = new ColorRect
        {
            Color = new Color(0f, 0f, 0f, 0.55f),
        };
        dim.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(dim);

        var center = new CenterContainer();
        center.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(center);

        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(420, 0);
        center.AddChild(panel);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 12);
        panel.AddChild(vbox);

        _titleLabel = new Label();
        _titleLabel.AddThemeFontSizeOverride("font_size", 22);
        _titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(_titleLabel);

        _instructionLabel = new Label();
        _instructionLabel.AddThemeFontSizeOverride("font_size", 14);
        _instructionLabel.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(_instructionLabel);

        _ticketList = new VBoxContainer();
        _ticketList.AddThemeConstantOverride("separation", 6);
        vbox.AddChild(_ticketList);

        _confirmButton = new Button { Text = "Keep selected tickets" };
        _confirmButton.Pressed += OnConfirm;
        vbox.AddChild(_confirmButton);
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
        if (state.Phase != GamePhase.ChoosingStartingTickets)
        {
            Visible = false;
            return;
        }

        Visible = true;
        var player = state.Players[state.CurrentPlayerIndex];
        _titleLabel.Text = $"Player {player.Id + 1} ({player.Color}): Starting tickets";
        _instructionLabel.Text =
            $"Keep at least {_engine.Config.StartingTicketsKeepMin} of {_engine.Config.StartingTicketsDealt} tickets";

        // Rebuild the ticket checkboxes for the current player
        foreach (var child in _ticketList.GetChildren())
        {
            child.QueueFree();
        }
        _checkBoxes.Clear();
        _ticketIds.Clear();

        foreach (var ticket in player.PendingStartingTickets)
        {
            var cb = new CheckBox
            {
                Text = $"  {CityNames.Display(ticket.CityA)}  \u2192  {CityNames.Display(ticket.CityB)}   ({ticket.Points} pts)",
                ButtonPressed = true,
            };
            cb.Toggled += _ => UpdateConfirmEnabled();
            _ticketList.AddChild(cb);
            _checkBoxes.Add(cb);
            _ticketIds.Add(ticket.Id);
        }

        UpdateConfirmEnabled();
    }

    private void UpdateConfirmEnabled()
    {
        if (_engine is null) return;
        int selected = 0;
        foreach (var cb in _checkBoxes)
        {
            if (cb.ButtonPressed) selected++;
        }
        _confirmButton.Disabled = selected < _engine.Config.StartingTicketsKeepMin;
    }

    private void OnConfirm()
    {
        if (_engine is null) return;
        var kept = new List<int>();
        for (int i = 0; i < _checkBoxes.Count; i++)
        {
            if (_checkBoxes[i].ButtonPressed)
            {
                kept.Add(_ticketIds[i]);
            }
        }
        _engine.KeepStartingTickets(_engine.State.CurrentPlayerIndex, kept);
    }
}
