namespace CowsGraveyards.Game;

using Godot;

public partial class CowEntity : Node3D
{
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _duration;
    private float _elapsed;
    private bool _animating;

    public void StartDrivePast(Vector3 startPosition, Vector3 endPosition, float duration = 3f)
    {
        _startPosition = startPosition;
        _endPosition = endPosition;
        _duration = duration;
        _elapsed = 0f;
        _animating = true;

        GlobalPosition = _startPosition;
    }

    public override void _Process(double delta)
    {
        if (!_animating)
        {
            return;
        }

        _elapsed += (float)delta;
        float t = Mathf.Clamp(_elapsed / _duration, 0f, 1f);

        GlobalPosition = _startPosition.Lerp(_endPosition, t);

        if (t >= 1f)
        {
            _animating = false;
            QueueFree();
        }
    }
}
