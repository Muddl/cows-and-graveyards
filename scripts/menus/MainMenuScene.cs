namespace CowsGraveyards.Menus;

using System.Collections.Generic;
using System.Linq;
using CowsGraveyards.Audio;
using Godot;

public partial class MainMenuScene : Control
{
    [Signal]
    public delegate void NewTripRequestedEventHandler(int slotIndex);

    [Signal]
    public delegate void LoadTripRequestedEventHandler(TripSave save);

    [Signal]
    public delegate void ReplayTutorialRequestedEventHandler();

    private const int SlotCount = 3;

    private readonly SaveManager _saveManager = new();

    // Settable in tests to override OS.GetName() without mocking.
    public string PlatformName { get; set; } = OS.GetName();

    // Main panel refs
    private Control? _mainPanel;
    private Button? _startTripButton;
    private Button? _loadTripButton;
    private Button? _replayTutorialButton;
    private Button? _quitButton;

    // Overlay error label (sits outside VBox so it doesn't shift layout)
    private Label? _errorLabel;

    // Slot select panel refs
    private Control? _slotSelectPanel;
    private readonly Button?[] _slotButtons = new Button?[SlotCount];
    private Button? _backButton;

    // Whether the slot panel was opened for loading (true) or not used for new-trip selection
    private bool _slotPanelIsForLoad;

    public override void _Ready()
    {
        _mainPanel       = GetNodeOrNull<Control>("MainPanel");
        _startTripButton = GetNodeOrNull<Button>("MainPanel/StartTripButton");
        _loadTripButton  = GetNodeOrNull<Button>("MainPanel/LoadTripButton");
        _replayTutorialButton = GetNodeOrNull<Button>("MainPanel/ReplayTutorialButton");
        _quitButton      = GetNodeOrNull<Button>("MainPanel/QuitButton");
        _errorLabel      = GetNodeOrNull<Label>("ErrorLabel");
        _slotSelectPanel = GetNodeOrNull<Control>("SlotSelectPanel");

        for (int i = 0; i < SlotCount; i++)
        {
            int slot = i; // capture for closure
            _slotButtons[i] = GetNodeOrNull<Button>($"SlotSelectPanel/Slot{i}Button");
            if (_slotButtons[i] is { } btn)
                btn.Pressed += () => OnSlotButtonPressed(slot);
        }

        _backButton = GetNodeOrNull<Button>("SlotSelectPanel/BackButton");

        if (_startTripButton is not null)
            _startTripButton.Pressed += OnStartTripPressed;

        if (_loadTripButton is not null)
            _loadTripButton.Pressed += () => OpenSlotPanel(forLoad: true);

        if (_backButton is not null)
            _backButton.Pressed += HideSlotSelectPanel;

        if (_replayTutorialButton is not null)
            _replayTutorialButton.Pressed += OnReplayTutorialPressed;

        if (_quitButton is not null)
            _quitButton.Pressed += OnQuitPressed;

        NewTripRequested  += (int slot) => NavigateToGame(slot, null);
        LoadTripRequested += (TripSave save) => NavigateToGame(save.SlotIndex, save);

        var slots = _saveManager.LoadAllSlots();
        if (_loadTripButton is not null)
            _loadTripButton.Visible = HasAnySave(slots);

        if (_errorLabel is not null)
            _errorLabel.Visible = false;

        if (!HasAvailableSlot(slots))
            ShowErrorMessage("All trip slots are full — complete a trip to start a new one.");

        HideSlotSelectPanel();
        if (_mainPanel is not null)
            _mainPanel.Visible = true;
        ApplyQuitButtonVisibility();

        var audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.PlayMusic("menu_theme");
    }

    // ── Public API (testable without scene tree) ──────────────────────────────

    /// <summary>Returns the display label for a slot given its saved data.</summary>
    public string GetSlotLabel(TripSave? save)
    {
        if (save is null)
            return "Empty";
        return $"Left: {save.LeftScore}  Right: {save.RightScore}";
    }

    /// <summary>Returns true if the Quit button should be shown on this platform.</summary>
    public bool ShouldShowQuitButton() =>
        PlatformName != "Android" && PlatformName != "iOS";

    /// <summary>Returns true if at least one slot is available (null).</summary>
    public bool HasAvailableSlot(IList<TripSave?> slots) =>
        slots.Any(s => s is null);

    /// <summary>Returns true if at least one slot has a save.</summary>
    public bool HasAnySave(IList<TripSave?> slots) =>
        slots.Any(s => s is not null);

    /// <summary>Called by the Start Trip button. Loads saves internally.</summary>
    public void OnStartTripPressed() =>
        OnStartTripPressed(_saveManager.LoadAllSlots());

    /// <summary>Finds the first free slot and starts a trip, or shows an error if all full.</summary>
    public void OnStartTripPressed(IList<TripSave?> slots)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] is null)
            {
                OnNewTripSlotPressed(i);
                return;
            }
        }

        ShowErrorMessage("All trip slots are full — complete a trip to start a new one.");
    }

    /// <summary>Called by a New Trip slot button (or directly in tests).</summary>
    public void OnNewTripSlotPressed(int slotIndex)
    {
        EmitSignal(SignalName.NewTripRequested, slotIndex);
    }

    /// <summary>Called by the Load Trip slot button (or directly in tests).</summary>
    public void OnLoadTripSlotPressed(TripSave save)
    {
        EmitSignal(SignalName.LoadTripRequested, save);
    }

    /// <summary>Called by the Replay Tutorial button. Resets tutorial flags
    /// for the first occupied save slot and starts that trip with tutorial active.</summary>
    public void OnReplayTutorialPressed()
    {
        var slots = _saveManager.LoadAllSlots();

        // Find first slot with a save and reset its tutorial flags
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] is { } save)
            {
                var reset = save.WithTutorialReset();
                _saveManager.SaveSlot(reset);
                EmitSignal(SignalName.ReplayTutorialRequested);
                EmitSignal(SignalName.LoadTripRequested, reset);
                return;
            }
        }

        // No saves exist — start a fresh trip on slot 0 (tutorial shows by default)
        EmitSignal(SignalName.ReplayTutorialRequested);
        EmitSignal(SignalName.NewTripRequested, 0);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void OnSlotButtonPressed(int slotIndex)
    {
        HideSlotSelectPanel();

        if (_slotPanelIsForLoad)
        {
            var save = _saveManager.LoadSlot(slotIndex);
            if (save is not null)
                OnLoadTripSlotPressed(save);
        }
        else
        {
            OnNewTripSlotPressed(slotIndex);
        }
    }

    private void OpenSlotPanel(bool forLoad)
    {
        _slotPanelIsForLoad = forLoad;
        var slots = _saveManager.LoadAllSlots();

        for (int i = 0; i < SlotCount; i++)
        {
            if (_slotButtons[i] is not { } btn) continue;
            var save = slots[i];
            btn.Text = GetSlotLabel(save);
            // For load panel, hide empty slots; for new-trip panel all are shown
            btn.Visible = !forLoad || save is not null;
        }

        if (_mainPanel is not null)
            _mainPanel.Visible = false;

        if (_slotSelectPanel is not null)
            _slotSelectPanel.Visible = true;
    }

    private void HideSlotSelectPanel()
    {
        if (_slotSelectPanel is not null)
            _slotSelectPanel.Visible = false;

        if (_mainPanel is not null)
            _mainPanel.Visible = true;
    }

    private void ShowErrorMessage(string msg)
    {
        if (_errorLabel is null) return;

        _errorLabel.Text = msg;
        _errorLabel.Visible = true;

        if (_startTripButton is not null)
            _startTripButton.Visible = false;
    }

    private void NavigateToGame(int slotIndex, TripSave? save)
    {
        PendingTrip.Set(slotIndex, save);
        GetTree().ChangeSceneToFile("res://scenes/game/GameScene.tscn");
    }

    private void ApplyQuitButtonVisibility()
    {
        if (_quitButton is not null)
            _quitButton.Visible = ShouldShowQuitButton();
    }

    private static void OnQuitPressed() => (Engine.GetMainLoop() as SceneTree)?.Quit();
}
