namespace CowsGraveyards.Game;

using Godot;

public partial class ScoreHud : Control
{
    private static readonly Color LeftColor = new(0.2f, 0.6f, 1.0f);
    private static readonly Color RightColor = new(1.0f, 0.4f, 0.3f);

    private Label? _leftLabel;
    private Label? _rightLabel;

    public string LeftScoreText { get; private set; } = "Left: 0";

    public string RightScoreText { get; private set; } = "Right: 0";

    public Color LeftLabelColor { get; private set; } = LeftColor;

    public Color RightLabelColor { get; private set; } = RightColor;

    public override void _Ready()
    {
        _leftLabel = GetNode<Label>("LeftLabel");
        _rightLabel = GetNode<Label>("RightLabel");

        _leftLabel.Modulate = LeftLabelColor;
        _rightLabel.Modulate = RightLabelColor;
    }

    public void UpdateScores(int left, int right)
    {
        LeftScoreText = $"Left: {left}";
        RightScoreText = $"Right: {right}";

        if (_leftLabel is not null) _leftLabel.Text = LeftScoreText;
        if (_rightLabel is not null) _rightLabel.Text = RightScoreText;
    }
}
