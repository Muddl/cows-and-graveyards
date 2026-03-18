namespace CowsGraveyards.Game;

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

    // Exposed for input-isolation tests and trip-state integration.
    public ScoreTracker Scores => _gameState.Scores;

    public bool IsInputEnabled { get; private set; } = true;

    public override void _Ready()
    {
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
        }

        var pauseButton = GetNodeOrNull<Button>("CanvasLayer/PauseButton");
        if (pauseButton is not null)
            pauseButton.Pressed += Pause;
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
        _saveManager?.SaveSlot(new TripSave(_activeSlot, Scores.LeftScore, Scores.RightScore));
        GetTree().ChangeSceneToFile("res://scenes/menus/MainMenuScene.tscn");
    }

    // ── Trip slot initialisation (Task 4.3) ───────────────────────────────────

    private int _activeSlot;
    private SaveManager? _saveManager;

    public void InitTripSlot(int slotIndex, TripSave? existingSave, SaveManager? saveManager = null)
    {
        _activeSlot = slotIndex;
        _saveManager = saveManager ?? new SaveManager();

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
    }

    private void SpawnGraveyard(TapSide side)
    {
        var graveyard = GraveyardScene.Instantiate<GraveyardEntity>();
        AddChild(graveyard);

        var start = _cowSpawner.GetSpawnPosition(side);
        var end = _cowSpawner.GetEndPosition(side);
        graveyard.StartDrivePast(start, end);
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
