namespace CowsGraveyards.Game;

using System;
using CowsGraveyards.Audio;
using CowsGraveyards.Menus;
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

    // Exposed for input-isolation tests and trip-state integration.
    public ScoreTracker Scores => _gameState.Scores;

    public bool IsInputEnabled { get; private set; } = true;

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

        InitTripSlot(PendingTrip.SlotIndex, PendingTrip.Save);
        PendingTrip.Clear();
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
        _saveManager?.SaveSlot(new TripSave(ActiveSlot, Scores.LeftScore, Scores.RightScore));
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
        }
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

        ZeroOpposingScore(side);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

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
    }
}
