using Godot;
using TrainGame.Logic;
using TrainGame.Model;

namespace TrainGame.UI;

/// <summary>
/// Initial modal that picks the player count. Dismisses itself after
/// <see cref="GameEngine.StartNewGame"/> has been called.
/// </summary>
public partial class SetupOverlay : Control
{
    private GameEngine? _engine;

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Stop;

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
        center.AddChild(panel);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 16);
        panel.AddChild(vbox);

        var title = new Label { Text = "Ticket to Ride" };
        title.AddThemeFontSizeOverride("font_size", 28);
        title.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(title);

        var sub = new Label { Text = "Choose player count" };
        sub.AddThemeFontSizeOverride("font_size", 18);
        sub.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(sub);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 12);
        vbox.AddChild(hbox);

        for (int n = 2; n <= 5; n++)
        {
            int count = n;
            var btn = new Button
            {
                Text = $"{count} players",
                CustomMinimumSize = new Vector2(110, 40),
            };
            btn.Pressed += () => OnPlayerCountPressed(count);
            hbox.AddChild(btn);
        }
    }

    public void Attach(GameEngine engine)
    {
        _engine = engine;
        engine.StateChanged += Refresh;
        Refresh();
    }

    private void OnPlayerCountPressed(int count)
    {
        if (_engine is null) return;
        _engine.StartNewGame(count);
    }

    private void Refresh()
    {
        Visible = _engine is not null && !_engine.State.Started;
    }
}
