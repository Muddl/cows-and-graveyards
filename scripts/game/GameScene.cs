namespace CowsGraveyards.Game;

using Godot;

public partial class GameScene : Node3D
{
    private GameState _gameState = null!;

    public override void _Ready()
    {
        _gameState = new GameState();
    }
}
