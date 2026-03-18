namespace CowsGraveyards.Game;

using Godot;

public partial class GameScene : Node3D
{
    private static readonly PackedScene CowScene =
        GD.Load<PackedScene>("res://scenes/game/CowEntity.tscn");

    private static readonly PackedScene GraveyardScene =
        GD.Load<PackedScene>("res://scenes/game/GraveyardEntity.tscn");

    private GameState _gameState = null!;
    private SideDetector _sideDetector = null!;
    private CowSpawner _cowSpawner = null!;
    private ScoreHud _scoreHud = null!;

    public override void _Ready()
    {
        _gameState = new GameState();
        _sideDetector = new SideDetector();
        _cowSpawner = new CowSpawner();
        _scoreHud = GetNode<ScoreHud>("CanvasLayer/ScoreHud");

        var buttonLeft = GetNode<GraveyardButton>("CanvasLayer/GraveyardButtonLeft");
        var buttonRight = GetNode<GraveyardButton>("CanvasLayer/GraveyardButtonRight");
        buttonLeft.Activated += OnGraveyardActivated;
        buttonRight.Activated += OnGraveyardActivated;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch { Pressed: true } touch)
        {
            HandleTap(touch.Position);
        }
    }

    private void HandleTap(Vector2 tapPosition)
    {
        float screenWidth = GetViewport().GetVisibleRect().Size.X;
        var side = _sideDetector.Detect(tapPosition.X, screenWidth);

        SpawnCow(side);
        IncrementScore(side);
    }

    private void OnGraveyardActivated(int side)
    {
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

        _scoreHud.UpdateScores(_gameState.Scores.LeftScore, _gameState.Scores.RightScore);
    }

    private void ZeroOpposingScore(TapSide buttonSide)
    {
        if (buttonSide == TapSide.Left)
            _gameState.Scores.ZeroRight();
        else
            _gameState.Scores.ZeroLeft();

        _scoreHud.UpdateScores(_gameState.Scores.LeftScore, _gameState.Scores.RightScore);
    }
}
