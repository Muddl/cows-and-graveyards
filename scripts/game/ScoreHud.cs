namespace CowsGraveyards.Game;

using Godot;

public partial class ScoreHud : Control
{
    private Label? _leftLabel;
    private Label? _rightLabel;

    public string LeftScoreText { get; private set; } = "L: 0";

    public string RightScoreText { get; private set; } = "R: 0";

    public override void _Ready()
    {
        _leftLabel = GetNode<Label>("LeftLabel");
        _rightLabel = GetNode<Label>("RightLabel");
    }

    public void UpdateScores(int left, int right)
    {
        LeftScoreText = $"L: {left}";
        RightScoreText = $"R: {right}";

        if (_leftLabel is not null) _leftLabel.Text = LeftScoreText;
        if (_rightLabel is not null) _rightLabel.Text = RightScoreText;
    }
}
