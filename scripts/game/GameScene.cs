namespace CowsGraveyards.Game;

using System;
using CowsGraveyards.Audio;
using CowsGraveyards.Menus;
using CowsGraveyards.UI;
using Godot;

public partial class GameScene : Node3D
{
    private static readonly PackedScene CowScene =
        GD.Load<PackedScene>("res://scenes/game/CowEntity.tscn");

    private static readonly PackedScene GraveyardScene =
        GD.Load<PackedScene>("res://scenes/game/GraveyardEntity.tscn");

    // Initialized at field level so test-seam methods work without _Ready().
    private GameState _gameState = new();
    private SideDetector _sideDetector = new();
    private CowSpawner _cowSpawner = new();
    private ScoreHud? _scoreHud;
    private GraveyardButton _graveyardButtonLeft = null!;
    private GraveyardButton _graveyardButtonRight = null!;
    private PauseMenu? _pauseMenu;
    private Button? _pauseButton;
    private AudioManager? _audioManager;

    private TutorialOverlay? _tutorialOverlay;
    private GraveyardTooltip? _graveyardTooltip;
    private bool _ownsGraveyardTooltip;

    // Exposed for input-isolation tests and trip-state integration.
    public ScoreTracker Scores => _gameState.Scores;

    public bool IsInputEnabled { get; private set; } = true;

    public bool IsTutorialActive { get; private set; }

    public bool IsGraveyardTooltipShowing { get; private set; }

    public override void _Ready()
    {
        _audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        _scoreHud = GetNode<ScoreHud>("CanvasLayer/ScoreHud");

        _graveyardButtonLeft = GetNode<GraveyardButton>("CanvasLayer/GraveyardButtonLeft");
        _graveyardButtonRight = GetNode<GraveyardButton>("CanvasLayer/GraveyardButtonRight");
        _graveyardButtonLeft.Activated += OnGraveyardActivated;
        _graveyardButtonRight.Activated += OnGraveyardActivated;

        _pauseMenu = GetNodeOrNull<PauseMenu>("CanvasLayer/PauseMenu");
        if (_pauseMenu is not null)
        {
            _pauseMenu.ResumeRequested += Resume;
            _pauseMenu.MainMenuRequested += SaveAndExit;
            _pauseMenu.CompleteTripRequested += CompleteAndExit;
        }

        _pauseButton = GetNodeOrNull<Button>("CanvasLayer/PauseButton");
        if (_pauseButton is not null)
            _pauseButton.Pressed += Pause;

        _tutorialOverlay = GetNodeOrNull<TutorialOverlay>("CanvasLayer/TutorialOverlay");
        if (_tutorialOverlay is not null)
            _tutorialOverlay.Dismissed += EndTutorial;

        _graveyardTooltip = GetNodeOrNull<GraveyardTooltip>("CanvasLayer/GraveyardTooltip");
        if (_graveyardTooltip is not null)
            _graveyardTooltip.Confirmed += OnGraveyardTooltipConfirmed;

        InitTripSlot(PendingTrip.SlotIndex, PendingTrip.Save);
        PendingTrip.Clear();

        // Show tutorial/tooltip based on persisted flags
        if (_tutorialOverlay is not null && !_tutorialSeen)
            BeginTutorial();

        if (_graveyardTooltip is not null && !_graveyardExplained)
            _graveyardTooltip.Initialize(graveyardExplained: false);
        else if (_graveyardTooltip is not null)
            _graveyardTooltip.Initialize(graveyardExplained: true);

        _audioManager?.StartAmbient();
        _audioManager?.PlayMusic("gameplay_theme");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete && _ownsGraveyardTooltip)
            _graveyardTooltip?.Free();
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsInputEnabled) return;

        if (@event is InputEventScreenTouch { Pressed: true } touch)
        {
            HandleTap(touch.Position);
        }
    }

    // ── Pause / Resume ────────────────────────────────────────────────────────

    public void Pause()
    {
        IsInputEnabled = false;
        SetProcessInput(false);

        if (_pauseMenu is not null)
            _pauseMenu.Visible = true;
    }

    public void Resume()
    {
        IsInputEnabled = true;
        SetProcessInput(true);

        if (_pauseMenu is not null)
            _pauseMenu.Visible = false;
    }

    // ── Scene transition ──────────────────────────────────────────────────────

    public void SaveAndExit()
    {
        _saveManager?.SaveSlot(new TripSave(ActiveSlot, Scores.LeftScore, Scores.RightScore,
            _tutorialSeen, _graveyardExplained));
        GetTree()?.ChangeSceneToFile("res://scenes/menus/MainMenuScene.tscn");
    }

    public void CompleteAndExit()
    {
        var record = new CompletedTripRecord(
            Scores.LeftScore,
            Scores.RightScore,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        );
        _tripHistoryManager.Append(record);
        _saveManager?.DeleteSlot(ActiveSlot);
        GetTree()?.ChangeSceneToFile("res://scenes/menus/MainMenuScene.tscn");
    }

    // ── Trip slot initialisation (Task 4.3) ───────────────────────────────────

    public int ActiveSlot { get; private set; }
    private SaveManager? _saveManager;
    private TripHistoryManager _tripHistoryManager = new();
    private bool _tutorialSeen;
    private bool _graveyardExplained;

    public void InitTripSlot(int slotIndex, TripSave? existingSave, SaveManager? saveManager = null, TripHistoryManager? historyManager = null)
    {
        ActiveSlot = slotIndex;
        _saveManager = saveManager ?? new SaveManager();
        _tripHistoryManager = historyManager ?? new TripHistoryManager();

        if (existingSave is not null)
        {
            _gameState.Scores.SetLeft(existingSave.LeftScore);
            _gameState.Scores.SetRight(existingSave.RightScore);
            _scoreHud?.UpdateScores(Scores.LeftScore, Scores.RightScore);
            _tutorialSeen = existingSave.TutorialSeen;
            _graveyardExplained = existingSave.GraveyardExplained;
        }
    }

    // ── Tutorial ──────────────────────────────────────────────────────────────

    public void BeginTutorial()
    {
        IsTutorialActive = true;
        IsInputEnabled = false;

        if (_tutorialOverlay is not null)
        {
            _tutorialOverlay.Initialize(tutorialSeen: false);
            _tutorialOverlay.Visible = true;
        }
    }

    public void EndTutorial()
    {
        IsTutorialActive = false;
        IsInputEnabled = true;
        _tutorialSeen = true;

        if (_tutorialOverlay is not null)
            _tutorialOverlay.Visible = false;

        PersistOnboardingFlags();
    }

    // ── Graveyard tooltip ──────────────────────────────────────────────────────

    public void EnableGraveyardTooltip()
    {
        if (_graveyardTooltip is null)
        {
            _graveyardTooltip = new GraveyardTooltip();
            _graveyardTooltip.Confirmed += OnGraveyardTooltipConfirmed;
            _ownsGraveyardTooltip = true;
        }

        _graveyardTooltip.Initialize(graveyardExplained: false);
    }

    public void ConfirmGraveyardTooltip()
    {
        IsGraveyardTooltipShowing = false;
        _graveyardTooltip?.Confirm();
    }

    private void OnGraveyardTooltipConfirmed()
    {
        IsGraveyardTooltipShowing = false;
        _graveyardExplained = true;
        PersistOnboardingFlags();
    }

    private bool TryBlockGraveyardWithTooltip()
    {
        if (_graveyardTooltip is null) return false;
        if (!_graveyardTooltip.ShouldBlockAction()) return false;

        IsGraveyardTooltipShowing = true;
        return true;
    }

    // ── Test seams (score logic without scene-tree spawning) ──────────────────

    /// <summary>
    /// Simulates a cow tap at the given screen position, running only score
    /// logic (no spawn). Respects <see cref="IsInputEnabled"/>.
    /// </summary>
    public void SimulateCowTap(float tapX, float screenWidth = 1080f)
    {
        if (!IsInputEnabled) return;

        var side = _sideDetector.Detect(tapX, screenWidth);
        IncrementScore(side);
    }

    /// <summary>
    /// Simulates a graveyard button press for the given side, running only
    /// score logic (no spawn). Respects <see cref="IsInputEnabled"/>.
    /// </summary>
    public void SimulateGraveyardPress(TapSide side)
    {
        if (!IsInputEnabled) return;
        if (TryBlockGraveyardWithTooltip()) return;

        ZeroOpposingScore(side);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void PersistOnboardingFlags()
    {
        _saveManager?.SaveSlot(new TripSave(ActiveSlot, Scores.LeftScore, Scores.RightScore,
            _tutorialSeen, _graveyardExplained));
    }

    private void HandleTap(Vector2 tapPosition)
    {
        if (_graveyardButtonLeft.GetButtonRect().HasPoint(tapPosition) ||
            _graveyardButtonRight.GetButtonRect().HasPoint(tapPosition))
            return;

        if (_pauseButton is not null && _pauseButton.GetGlobalRect().HasPoint(tapPosition))
            return;

        float screenWidth = GetViewport().GetVisibleRect().Size.X;
        var side = _sideDetector.Detect(tapPosition.X, screenWidth);

        SpawnCow(side);
        IncrementScore(side);
    }

    private void OnGraveyardActivated(int side)
    {
        if (!IsInputEnabled) return;
        if (TryBlockGraveyardWithTooltip()) return;

        var buttonSide = (TapSide)side;
        SpawnGraveyard(buttonSide);
        ZeroOpposingScore(buttonSide);
    }

    private void SpawnCow(TapSide side)
    {
        var cow = CowScene.Instantiate<CowEntity>();
        AddChild(cow);

        var start = _cowSpawner.GetSpawnPosition(side);
        var end = _cowSpawner.GetEndPosition(side);
        cow.StartDrivePast(start, end);

        _cowSpawner.RecordSpawn();
        _audioManager?.PlaySfx("tap");
        _audioManager?.PlaySfx("cow_moo");
    }

    private void SpawnGraveyard(TapSide side)
    {
        var graveyard = GraveyardScene.Instantiate<GraveyardEntity>();
        AddChild(graveyard);

        var start = _cowSpawner.GetSpawnPosition(side);
        var end = _cowSpawner.GetEndPosition(side);
        graveyard.StartDrivePast(start, end);

        _audioManager?.PlaySfx("gravestone_thud");
    }

    private void IncrementScore(TapSide side)
    {
        if (side == TapSide.Left)
            _gameState.Scores.IncrementLeft();
        else
            _gameState.Scores.IncrementRight();

        _scoreHud?.UpdateScores(_gameState.Scores.LeftScore, _gameState.Scores.RightScore);
    }

    private void ZeroOpposingScore(TapSide buttonSide)
    {
        if (buttonSide == TapSide.Left)
            _gameState.Scores.ZeroRight();
        else
            _gameState.Scores.ZeroLeft();

        _scoreHud?.UpdateScores(_gameState.Scores.LeftScore, _gameState.Scores.RightScore);
        _audioManager?.PlaySfx("graveyard_penalty");
    }
}
