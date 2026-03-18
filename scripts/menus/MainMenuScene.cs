namespace CowsGraveyards.Menus;

using Godot;

public partial class MainMenuScene : Control
{
    [Signal]
    public delegate void NewTripRequestedEventHandler(int slotIndex);

    [Signal]
    public delegate void LoadTripRequestedEventHandler(TripSave save);

    private const int SlotCount = 3;

    private readonly SaveManager _saveManager = new();

    // Settable in tests to override OS.GetName() without mocking.
    public string PlatformName { get; set; } = OS.GetName();

    // Child node references — set in _Ready() when scene tree is available.
    private Button?[] _newTripButtons = new Button?[SlotCount];
    private Button?[] _loadTripButtons = new Button?[SlotCount];
    private Button? _quitButton;

    public override void _Ready()
    {
        _quitButton = GetNodeOrNull<Button>("VBox/QuitButton");

        for (int i = 0; i < SlotCount; i++)
        {
            int slot = i; // capture for closure
            _newTripButtons[i] = GetNodeOrNull<Button>($"VBox/Slot{i}/NewTripButton");
            _loadTripButtons[i] = GetNodeOrNull<Button>($"VBox/Slot{i}/LoadTripButton");

            if (_newTripButtons[i] is { } newBtn)
                newBtn.Pressed += () => OnNewTripSlotPressed(slot);

            if (_loadTripButtons[i] is { } loadBtn)
                loadBtn.Pressed += () => OnLoadTripSlotPressedByIndex(slot);
        }

        if (_quitButton is not null)
            _quitButton.Pressed += OnQuitPressed;

        RefreshSlotLabels();
        ApplyQuitButtonVisibility();
    }

    // ── Public API (testable without scene tree) ──────────────────────────────

    /// <summary>Returns the display label for a slot given its saved data.</summary>
    public string GetSlotLabel(TripSave? save)
    {
        if (save is null)
            return "Empty";
        return $"L: {save.LeftScore}  R: {save.RightScore}";
    }

    /// <summary>Returns true if the Quit button should be shown on this platform.</summary>
    public bool ShouldShowQuitButton() =>
        PlatformName != "Android" && PlatformName != "iOS";

    /// <summary>Called by the New Trip button for a given slot (or directly in tests).</summary>
    public void OnNewTripSlotPressed(int slotIndex)
    {
        EmitSignal(SignalName.NewTripRequested, slotIndex);
    }

    /// <summary>Called by the Load Trip button (or directly in tests).</summary>
    public void OnLoadTripSlotPressed(TripSave save)
    {
        EmitSignal(SignalName.LoadTripRequested, save);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void OnLoadTripSlotPressedByIndex(int slotIndex)
    {
        var save = _saveManager.LoadSlot(slotIndex);
        if (save is not null)
            OnLoadTripSlotPressed(save);
    }

    private void RefreshSlotLabels()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            var save = _saveManager.LoadSlot(i);
            if (_loadTripButtons[i] is { } btn)
                btn.Text = GetSlotLabel(save);
        }
    }

    private void ApplyQuitButtonVisibility()
    {
        if (_quitButton is not null)
            _quitButton.Visible = ShouldShowQuitButton();
    }

    private static void OnQuitPressed() => (Engine.GetMainLoop() as SceneTree)?.Quit();
}
