namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Integration tests for the full graveyard pipeline:
/// GraveyardButton activation → ZeroLeft/ZeroRight → ScoreHud update.
/// Covers coexistence with cow taps to guard against regressions.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class GraveyardIntegrationTest
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

    private void SimulateCowTap(float tapX, float screenWidth = 1080f)
    {
        var side = _detector.Detect(tapX, screenWidth);
        if (side == TapSide.Left)
            _state.Scores.IncrementLeft();
        else
            _state.Scores.IncrementRight();
        _hud.UpdateScores(_state.Scores.LeftScore, _state.Scores.RightScore);
    }

    /// <summary>
    /// Simulates a GraveyardButton press: the pressed side spawns an entity
    /// (visual, not tested here) and zeroes the OPPOSING side's score.
    /// </summary>
    private void SimulateGraveyardPress(TapSide buttonSide)
    {
        if (buttonSide == TapSide.Left)
            _state.Scores.ZeroRight();
        else
            _state.Scores.ZeroLeft();
        _hud.UpdateScores(_state.Scores.LeftScore, _state.Scores.RightScore);
    }

    // Task 4.1 — left graveyard zeroes right score, HUD reflects immediately

    [TestCase]
    public void LeftGraveyardZeroesRightScore()
    {
        SimulateCowTap(800f);
        SimulateCowTap(800f);
        SimulateCowTap(800f);

        SimulateGraveyardPress(TapSide.Left);

        AssertThat(_state.Scores.RightScore).IsEqual(0);
    }

    [TestCase]
    public void LeftGraveyardUpdatesHudRightScoreToZero()
    {
        SimulateCowTap(800f);
        SimulateCowTap(800f);

        SimulateGraveyardPress(TapSide.Left);

        AssertThat(_hud.RightScoreText).IsEqual("R: 0");
    }

    // Task 4.2 — right graveyard zeroes left score, HUD reflects immediately

    [TestCase]
    public void RightGraveyardZeroesLeftScore()
    {
        SimulateCowTap(100f);
        SimulateCowTap(100f);
        SimulateCowTap(100f);

        SimulateGraveyardPress(TapSide.Right);

        AssertThat(_state.Scores.LeftScore).IsEqual(0);
    }

    [TestCase]
    public void RightGraveyardUpdatesHudLeftScoreToZero()
    {
        SimulateCowTap(100f);
        SimulateCowTap(100f);

        SimulateGraveyardPress(TapSide.Right);

        AssertThat(_hud.LeftScoreText).IsEqual("L: 0");
    }

    // Task 4.3 — graveyard activation does not affect the same side's score

    [TestCase]
    public void LeftGraveyardDoesNotAffectLeftScore()
    {
        SimulateCowTap(100f);
        SimulateCowTap(100f);

        SimulateGraveyardPress(TapSide.Left);

        AssertThat(_state.Scores.LeftScore).IsEqual(2);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 2");
    }

    [TestCase]
    public void RightGraveyardDoesNotAffectRightScore()
    {
        SimulateCowTap(800f);
        SimulateCowTap(800f);

        SimulateGraveyardPress(TapSide.Right);

        AssertThat(_state.Scores.RightScore).IsEqual(2);
        AssertThat(_hud.RightScoreText).IsEqual("R: 2");
    }

    // Task 4.4 — interleaved cow taps and graveyard activations

    [TestCase]
    public void GraveyardAfterTapsResetsOpposingScoreOnly()
    {
        SimulateCowTap(100f);
        SimulateCowTap(100f);
        SimulateCowTap(800f);
        SimulateCowTap(800f);

        SimulateGraveyardPress(TapSide.Left);  // zeroes right

        AssertThat(_state.Scores.LeftScore).IsEqual(2);
        AssertThat(_state.Scores.RightScore).IsEqual(0);
        AssertThat(_hud.LeftScoreText).IsEqual("L: 2");
        AssertThat(_hud.RightScoreText).IsEqual("R: 0");
    }

    [TestCase]
    public void TapsAfterGraveyardAccumulateFromZero()
    {
        SimulateCowTap(800f);
        SimulateCowTap(800f);
        SimulateGraveyardPress(TapSide.Left);  // right now 0
        SimulateCowTap(800f);
        SimulateCowTap(800f);

        AssertThat(_state.Scores.RightScore).IsEqual(2);
        AssertThat(_hud.RightScoreText).IsEqual("R: 2");
    }

    [TestCase]
    public void MultipleGraveyardPressesAccumulateCorrectly()
    {
        SimulateCowTap(100f);
        SimulateCowTap(100f);
        SimulateCowTap(800f);
        SimulateCowTap(800f);

        SimulateGraveyardPress(TapSide.Left);   // right → 0
        SimulateCowTap(800f);                    // right → 1
        SimulateGraveyardPress(TapSide.Right);   // left → 0
        SimulateCowTap(100f);                    // left → 1

        AssertThat(_state.Scores.LeftScore).IsEqual(1);
        AssertThat(_state.Scores.RightScore).IsEqual(1);
    }

    [TestCase]
    public void HudAlwaysMatchesScoreTrackerThroughInterleavedEvents()
    {
        var events = new (bool isGraveyard, float tapX, TapSide side)[]
        {
            (false, 100f, TapSide.Left),
            (false, 800f, TapSide.Left),
            (true,  0f,   TapSide.Left),
            (false, 100f, TapSide.Left),
            (true,  0f,   TapSide.Right),
            (false, 800f, TapSide.Left),
        };

        foreach (var (isGraveyard, tapX, side) in events)
        {
            if (isGraveyard)
                SimulateGraveyardPress(side);
            else
                SimulateCowTap(tapX);

            AssertThat(_hud.LeftScoreText).IsEqual($"L: {_state.Scores.LeftScore}");
            AssertThat(_hud.RightScoreText).IsEqual($"R: {_state.Scores.RightScore}");
        }
    }
}
