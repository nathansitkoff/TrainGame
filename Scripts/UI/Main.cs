using Godot;
using TrainGame.Logic;

namespace TrainGame.UI;

public partial class Main : Control
{
    private GameEngine _engine = null!;

    public override void _Ready()
    {
        var configJson = LoadConfigJson();
        var config = GameConfigLoader.FromJson(configJson);
        _engine = new GameEngine(config, new SeededRandom(1));

        GetNode<HudPanel>("HudPanel").Attach(_engine);
        GetNode<PlayerPanelList>("PlayerPanelList").Attach(_engine);
        GetNode<SetupOverlay>("SetupOverlay").Attach(_engine);
        GetNode<TicketChoiceOverlay>("TicketChoiceOverlay").Attach(_engine);

        GD.Print("TrainGame ready. Awaiting player count selection.");
    }

    private static string LoadConfigJson()
    {
        using var file = Godot.FileAccess.Open(
            "res://Data/game_config.json", Godot.FileAccess.ModeFlags.Read);
        if (file is null)
        {
            GD.PrintErr("Failed to open res://Data/game_config.json");
            return "{}";
        }
        return file.GetAsText();
    }
}
