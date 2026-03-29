namespace CowsGraveyards.Tests.UI;

using CowsGraveyards.Game;
using CowsGraveyards.UI;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class TutorialInputBlockTest
{
    private GameScene _game = null!;

    [BeforeTest]
    public void Setup()
    {
        _game = AutoFree(new GameScene())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GameplayTapIgnoredWhileTutorialActive()
    {
        _game.BeginTutorial();

        _game.SimulateCowTap(100f);

        AssertThat(_game.Scores.LeftScore).IsEqual(0);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GraveyardPressIgnoredWhileTutorialActive()
    {
        _game.SimulateCowTap(800f); // right = 1 (before tutorial starts)
        _game.BeginTutorial();

        _game.SimulateGraveyardPress(TapSide.Left);

        AssertThat(_game.Scores.RightScore).IsEqual(1);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void InputRestoredAfterTutorialDismissed()
    {
        _game.BeginTutorial();
        _game.EndTutorial();

        _game.SimulateCowTap(100f);

        AssertThat(_game.Scores.LeftScore).IsEqual(1);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void IsTutorialActiveReturnsTrueWhileTutorialShowing()
    {
        _game.BeginTutorial();

        AssertThat(_game.IsTutorialActive).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void IsTutorialActiveReturnsFalseAfterDismissal()
    {
        _game.BeginTutorial();
        _game.EndTutorial();

        AssertThat(_game.IsTutorialActive).IsFalse();
    }
}
