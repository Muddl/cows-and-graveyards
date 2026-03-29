namespace CowsGraveyards.Tests.UI;

using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class ReplayTutorialTest
{
    private MainMenuScene _menu = null!;

    [BeforeTest]
    public void Setup()
    {
        _menu = AutoFree(new MainMenuScene())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ReplayTutorialCallsResetOnSave()
    {
        var save = new TripSave(0, 5, 3, tutorialSeen: true, graveyardExplained: true);

        var reset = save.WithTutorialReset();

        AssertThat(reset.TutorialSeen).IsFalse();
        AssertThat(reset.GraveyardExplained).IsFalse();
        AssertThat(reset.LeftScore).IsEqual(5);
        AssertThat(reset.RightScore).IsEqual(3);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ReplayTutorialEmitsSignal()
    {
        bool signalFired = false;
        _menu.ReplayTutorialRequested += () => signalFired = true;

        _menu.OnReplayTutorialPressed();

        AssertThat(signalFired).IsTrue();
    }
}
