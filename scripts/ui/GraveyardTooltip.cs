namespace CowsGraveyards.UI;

using Godot;

public partial class GraveyardTooltip : Control
{
    [Signal]
    public delegate void ConfirmedEventHandler();

    private const string ExplanationText =
        "Pressing Graveyard resets your opponent's cow count to zero! Use it when you pass a graveyard.";

    private Label? _messageLabel;
    private Button? _gotItButton;

    public bool IsExplained { get; private set; }

    public override void _Ready()
    {
        _messageLabel = GetNodeOrNull<Label>("Panel/MessageLabel");
        _gotItButton = GetNodeOrNull<Button>("Panel/GotItButton");

        if (_messageLabel is not null)
            _messageLabel.Text = ExplanationText;

        if (_gotItButton is not null)
            _gotItButton.Pressed += Confirm;
    }

    public void Initialize(bool graveyardExplained)
    {
        IsExplained = graveyardExplained;

        if (IsInsideTree())
            Visible = false;
    }

    public bool ShouldBlockAction()
    {
        if (IsExplained) return false;

        if (IsInsideTree())
            Visible = true;

        return true;
    }

    public void Confirm()
    {
        IsExplained = true;

        if (IsInsideTree())
            Visible = false;

        EmitSignal(SignalName.Confirmed);
    }
}
