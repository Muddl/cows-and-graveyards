namespace CowsGraveyards.Game;

using Godot;

public partial class GraveyardButton : Control
{
    [Signal]
    public delegate void ActivatedEventHandler(int side);

    [Export]
    public TapSide Side { get; set; }

    private Button _button = null!;

    public override void _Ready()
    {
        _button = GetNode<Button>("Button");
        _button.Pressed += OnButtonPressed;
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public Rect2 GetButtonRect() => _button.GetGlobalRect();

    public void OnButtonPressed()
    {
        EmitSignal(SignalName.Activated, (int)Side);
    }
}
