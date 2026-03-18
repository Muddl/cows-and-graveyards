namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class ScoreHudTest
{
    private ScoreHud _hud = null!;

    [BeforeTest]
    public void Setup()
    {
        _hud = AutoFree(new ScoreHud())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void InitialLeftScoreDisplaysZero()
    {
        AssertThat(_hud.LeftScoreText).IsEqual("L: 0");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void InitialRightScoreDisplaysZero()
    {
        AssertThat(_hud.RightScoreText).IsEqual("R: 0");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void UpdateScoresSetsLeftText()
    {
        _hud.UpdateScores(3, 0);

        AssertThat(_hud.LeftScoreText).IsEqual("L: 3");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void UpdateScoresSetsRightText()
    {
        _hud.UpdateScores(0, 5);

        AssertThat(_hud.RightScoreText).IsEqual("R: 5");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void UpdateScoresSetsBothSides()
    {
        _hud.UpdateScores(7, 4);

        AssertThat(_hud.LeftScoreText).IsEqual("L: 7");
        AssertThat(_hud.RightScoreText).IsEqual("R: 4");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void UpdateScoresReflectsSubsequentChanges()
    {
        _hud.UpdateScores(1, 0);
        _hud.UpdateScores(2, 0);
        _hud.UpdateScores(3, 1);

        AssertThat(_hud.LeftScoreText).IsEqual("L: 3");
        AssertThat(_hud.RightScoreText).IsEqual("R: 1");
    }
}