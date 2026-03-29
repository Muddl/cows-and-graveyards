namespace CowsGraveyards.UI;

using Godot;

public partial class TutorialOverlay : Control
{
    [Signal]
    public delegate void DismissedEventHandler();

    public const int StepCount = 4;

    private static readonly string[] StepTexts =
    {
        "Tap the left side to count cows on your side!",
        "Tap the right side to count cows on your side!",
        "These are your scores \u2014 left player and right player.",
        "The graveyard button zeros your opponent\u2019s cows \u2014 use it wisely!",
    };

    private Label? _stepLabel;
    private Button? _nextButton;

    public int CurrentStep { get; private set; }

    public bool ShouldShow { get; private set; }

    public bool IsComplete { get; private set; }

    public override void _Ready()
    {
        _stepLabel = GetNodeOrNull<Label>("Panel/StepLabel");
        _nextButton = GetNodeOrNull<Button>("Panel/NextButton");

        if (_nextButton is not null)
            _nextButton.Pressed += AdvanceStep;

        UpdateStepDisplay();
    }

    public void Initialize(bool tutorialSeen)
    {
        ShouldShow = !tutorialSeen;
        IsComplete = tutorialSeen;
        CurrentStep = 0;

        if (IsInsideTree())
            Visible = ShouldShow;
    }

    public void AdvanceStep()
    {
        if (!ShouldShow || IsComplete) return;

        CurrentStep++;

        if (CurrentStep >= StepCount)
        {
            IsComplete = true;
            ShouldShow = false;

            if (IsInsideTree())
                Visible = false;

            EmitSignal(SignalName.Dismissed);
            return;
        }

        UpdateStepDisplay();
    }

    public string GetStepText(int step) =>
        step >= 0 && step < StepCount ? StepTexts[step] : string.Empty;

    private void UpdateStepDisplay()
    {
        if (_stepLabel is not null && CurrentStep < StepCount)
            _stepLabel.Text = StepTexts[CurrentStep];
    }
}
