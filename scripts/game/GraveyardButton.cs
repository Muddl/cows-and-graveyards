namespace CowsGraveyards.Game;

using Godot;

public partial class GraveyardButton : Control
{
    [Signal]
    public delegate void ActivatedEventHandler(int side);

    [Export]
    public TapSide Side { get; set; }

    public override void _Ready()
    {
        var button = GetNode<Button>("Button");
        button.Pressed += OnButtonPressed;
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public void OnButtonPressed()
    {
        EmitSignal(SignalName.Activated, (int)Side);
    }
}
