namespace CowsGraveyards.Game;

using Godot;

public partial class GameScene : Node3D
{
    private static readonly PackedScene CowScene =
        GD.Load<PackedScene>("res://scenes/game/CowEntity.tscn");

    private GameState _gameState = null!;
    private SideDetector _sideDetector = null!;
    private CowSpawner _cowSpawner = null!;

    public override void _Ready()
    {
        _gameState = new GameState();
        _sideDetector = new SideDetector();
        _cowSpawner = new CowSpawner();
    }

    public override void _UnhandledInput(InputEvent @event)
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

    private void SpawnCow(TapSide side)
    {
        var cow = CowScene.Instantiate<CowEntity>();
        AddChild(cow);

        var start = _cowSpawner.GetSpawnPosition(side);
        var end = _cowSpawner.GetEndPosition(side);
        cow.StartDrivePast(start, end);

        _cowSpawner.RecordSpawn();
    }

    private void IncrementScore(TapSide side)
    {
        if (side == TapSide.Left)
        {
            _gameState.Scores.IncrementLeft();
        }
        else
        {
            _gameState.Scores.IncrementRight();
        }
    }
}
