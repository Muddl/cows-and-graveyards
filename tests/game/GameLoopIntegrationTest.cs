namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Integration tests for the full tap-to-score pipeline:
/// tap position → SideDetector → ScoreTracker → ScoreHud
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class GameLoopIntegrationTest
{
    private SideDetector _detector = null!;
    private GameState _state = null!;
    private ScoreHud _hud = null!;

    [BeforeTest]
    public void Setup()
    {
        _detector = new SideDetector();
        _state = new GameState();
        _hud = AutoFree(new ScoreHud())!;
    }

    private void SimulateTap(float tapX, float screenWidth = 1080f)
    {
        var side = _detector.Detect(tapX, screenWidth);
        if (side == TapSide.Left)
            _state.Scores.IncrementLeft();
        else
            _state.Scores.IncrementRight();
        _hud.UpdateScores(_state.Scores.LeftScore, _state.Scores.RightScore);
    }

    // Task 4.3 — full tap-to-score loop

    [TestCase]
    public void LeftTapIncrementsLeftScoreAndUpdatesHud()
    {
        SimulateTap(100f);

        AssertThat(_state.Scores.LeftScore).IsEqual(1);
        AssertThat(_state.Scores.RightScore).IsEqual(0);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 1");
        AssertThat(_hud.RightScoreText).IsEqual("R: 0");
    }

    [TestCase]
    public void RightTapIncrementsRightScoreAndUpdatesHud()
    {
        SimulateTap(800f);

        AssertThat(_state.Scores.LeftScore).IsEqual(0);
        AssertThat(_state.Scores.RightScore).IsEqual(1);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 0");
        AssertThat(_hud.RightScoreText).IsEqual("R: 1");
    }

    [TestCase]
    public void HudReflectsEachTapImmediately()
    {
        SimulateTap(100f);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 1");

        SimulateTap(800f);
        AssertThat(_hud.RightScoreText).IsEqual("R: 1");

        SimulateTap(100f);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 2");
    }

    [TestCase]
    public void TapsOnDifferentScreenWidthsResolveCorrectly()
    {
        SimulateTap(100f, 720f);    // left on 720-wide screen
        SimulateTap(500f, 720f);    // right on 720-wide screen

        AssertThat(_state.Scores.LeftScore).IsEqual(1);
        AssertThat(_state.Scores.RightScore).IsEqual(1);
    }

    // Task 4.4 — scores persist across multiple taps on both sides

    [TestCase]
    public void ScoresPersistAcrossMultipleTapsOnBothSides()
    {
        SimulateTap(100f);
        SimulateTap(100f);
        SimulateTap(100f);
        SimulateTap(800f);
        SimulateTap(800f);

        AssertThat(_state.Scores.LeftScore).IsEqual(3);
        AssertThat(_state.Scores.RightScore).IsEqual(2);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 3");
        AssertThat(_hud.RightScoreText).IsEqual("R: 2");
    }

    [TestCase]
    public void ScoresAreIndependentAndNeverInterfere()
    {
        for (int i = 0; i < 5; i++) SimulateTap(100f);
        for (int i = 0; i < 3; i++) SimulateTap(800f);

        AssertThat(_state.Scores.LeftScore).IsEqual(5);
        AssertThat(_state.Scores.RightScore).IsEqual(3);
    }

    [TestCase]
    public void HudMatchesScoreTrackerAtAllTimes()
    {
        var taps = new[] { 100f, 800f, 100f, 100f, 800f };
        foreach (var tap in taps)
        {
            SimulateTap(tap);
            AssertThat(_hud.LeftScoreText).IsEqual($"L: {_state.Scores.LeftScore}");
            AssertThat(_hud.RightScoreText).IsEqual($"R: {_state.Scores.RightScore}");
        }
    }
}
