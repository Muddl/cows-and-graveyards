namespace CowsGraveyards.Menus;

using Godot;

public partial class PauseMenu : Control
{
    [Signal]
    public delegate void ResumeRequestedEventHandler();

    [Signal]
    public delegate void MainMenuRequestedEventHandler();

    [Signal]
    public delegate void CompleteTripRequestedEventHandler();

    // Settable in tests to override OS.GetName() without mocking.
    public string PlatformName { get; set; } = OS.GetName();

    private Button? _quitButton;

    public override void _Ready()
    {
        var resumeButton = GetNodeOrNull<Button>("VBox/ResumeButton");
        var mainMenuButton = GetNodeOrNull<Button>("VBox/MainMenuButton");
        var completeTripButton = GetNodeOrNull<Button>("VBox/CompleteTripButton");
        _quitButton = GetNodeOrNull<Button>("VBox/QuitButton");

        if (resumeButton is not null)
            resumeButton.Pressed += OnResumePressed;

        if (mainMenuButton is not null)
            mainMenuButton.Pressed += OnMainMenuPressed;

        if (completeTripButton is not null)
            completeTripButton.Pressed += OnCompleteTripPressed;

        if (_quitButton is not null)
            _quitButton.Pressed += OnQuitPressed;

        ApplyQuitButtonVisibility();
    }

    // ── Public API (testable without scene tree) ──────────────────────────────

    public bool ShouldShowQuitButton() =>
        PlatformName != "Android" && PlatformName != "iOS";

    public void OnResumePressed()
    {
        EmitSignal(SignalName.ResumeRequested);
    }

    public void OnMainMenuPressed()
    {
        EmitSignal(SignalName.MainMenuRequested);
    }

    public void OnCompleteTripPressed()
    {
        EmitSignal(SignalName.CompleteTripRequested);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void ApplyQuitButtonVisibility()
    {
        if (_quitButton is not null)
            _quitButton.Visible = ShouldShowQuitButton();
    }

    private static void OnQuitPressed() =>
        (Engine.GetMainLoop() as SceneTree)?.Quit();
}
