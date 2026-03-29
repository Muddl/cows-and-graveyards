namespace CowsGraveyards.Tests.UI;

using CowsGraveyards.UI;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class TutorialOverlayTest
{
    private TutorialOverlay _overlay = null!;

    [BeforeTest]
    public void Setup()
    {
        _overlay = AutoFree(new TutorialOverlay())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void OverlayVisibleWhenTutorialNotSeen()
    {
        _overlay.Initialize(tutorialSeen: false);

        AssertThat(_overlay.ShouldShow).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void OverlayHiddenWhenTutorialSeen()
    {
        _overlay.Initialize(tutorialSeen: true);

        AssertThat(_overlay.ShouldShow).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void AdvanceStepIncrementsCurrentStep()
    {
        _overlay.Initialize(tutorialSeen: false);

        AssertThat(_overlay.CurrentStep).IsEqual(0);

        _overlay.AdvanceStep();

        AssertThat(_overlay.CurrentStep).IsEqual(1);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void AdvancingPastAllStepsMarksTutorialComplete()
    {
        _overlay.Initialize(tutorialSeen: false);

        for (int i = 0; i < TutorialOverlay.StepCount; i++)
            _overlay.AdvanceStep();

        AssertThat(_overlay.IsComplete).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void CompletionEmitsDismissedSignal()
    {
        _overlay.Initialize(tutorialSeen: false);
        bool dismissed = false;
        _overlay.Dismissed += () => dismissed = true;

        for (int i = 0; i < TutorialOverlay.StepCount; i++)
            _overlay.AdvanceStep();

        AssertThat(dismissed).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void HasFourSteps()
    {
        AssertThat(TutorialOverlay.StepCount).IsEqual(4);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void AdvanceStepDoesNothingWhenAlreadyComplete()
    {
        _overlay.Initialize(tutorialSeen: false);

        for (int i = 0; i < TutorialOverlay.StepCount; i++)
            _overlay.AdvanceStep();

        int stepAfterComplete = _overlay.CurrentStep;
        _overlay.AdvanceStep();

        AssertThat(_overlay.CurrentStep).IsEqual(stepAfterComplete);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void AdvanceStepDoesNothingWhenTutorialAlreadySeen()
    {
        _overlay.Initialize(tutorialSeen: true);

        _overlay.AdvanceStep();

        AssertThat(_overlay.CurrentStep).IsEqual(0);
    }
}
