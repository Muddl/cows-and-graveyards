namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Integration tests for GameScene input isolation.
/// Verifies that Pause() blocks score changes from cow taps and graveyard
/// presses, and Resume() restores them — independent of the Godot scene tree.
/// Uses GameScene's test-seam methods (SimulateCowTap, SimulateGraveyardPress).
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class InputIsolationTest
{
    private GameScene _game = null!;

    [BeforeTest]
    public void Setup()
    {
        _game = AutoFree(new GameScene())!;
    }

    // ── IsInputEnabled flag ───────────────────────────────────────────────────

    [TestCase]
    public void InputEnabledByDefault()
    {
        AssertThat(_game.IsInputEnabled).IsTrue();
    }

    [TestCase]
    public void PauseDisablesInput()
    {
        _game.Pause();

        AssertThat(_game.IsInputEnabled).IsFalse();
    }

    [TestCase]
    public void ResumeEnablesInput()
    {
        _game.Pause();
        _game.Resume();

        AssertThat(_game.IsInputEnabled).IsTrue();
    }

    // ── Cow tap isolation ─────────────────────────────────────────────────────

    [TestCase]
    public void CowTapIncrementsScoreWhenNotPaused()
    {
        _game.SimulateCowTap(100f); // left tap

        AssertThat(_game.Scores.LeftScore).IsEqual(1);
    }

    [TestCase]
    public void CowTapDoesNotIncrementScoreWhenPaused()
    {
        _game.Pause();
        _game.SimulateCowTap(100f);

        AssertThat(_game.Scores.LeftScore).IsEqual(0);
    }

    [TestCase]
    public void CowTapIncrementsScoreAfterResume()
    {
        _game.Pause();
        _game.SimulateCowTap(100f); // blocked
        _game.Resume();
        _game.SimulateCowTap(100f); // should count

        AssertThat(_game.Scores.LeftScore).IsEqual(1);
    }

    [TestCase]
    public void MultiplePausedTapsProduceNoScoreChange()
    {
        _game.SimulateCowTap(100f); // scores 1
        _game.Pause();
        _game.SimulateCowTap(100f);
        _game.SimulateCowTap(100f);
        _game.SimulateCowTap(800f);

        AssertThat(_game.Scores.LeftScore).IsEqual(1);
        AssertThat(_game.Scores.RightScore).IsEqual(0);
    }

    // ── Graveyard press isolation ─────────────────────────────────────────────

    [TestCase]
    public void GraveyardPressZeroesScoreWhenNotPaused()
    {
        _game.SimulateCowTap(800f); // right = 1
        _game.SimulateGraveyardPress(TapSide.Left); // zeroes right

        AssertThat(_game.Scores.RightScore).IsEqual(0);
    }

    [TestCase]
    public void GraveyardPressDoesNotZeroScoreWhenPaused()
    {
        _game.SimulateCowTap(800f); // right = 1
        _game.Pause();
        _game.SimulateGraveyardPress(TapSide.Left); // should be blocked

        AssertThat(_game.Scores.RightScore).IsEqual(1);
    }

    [TestCase]
    public void GraveyardPressZeroesScoreAfterResume()
    {
        _game.SimulateCowTap(800f); // right = 1
        _game.Pause();
        _game.SimulateGraveyardPress(TapSide.Left); // blocked
        _game.Resume();
        _game.SimulateGraveyardPress(TapSide.Left); // should zero

        AssertThat(_game.Scores.RightScore).IsEqual(0);
    }

    // ── Interleaved pause/resume cycles ──────────────────────────────────────

    [TestCase]
    public void ScoreOnlyChangesWhenInputEnabled()
    {
        _game.SimulateCowTap(100f); // left = 1

        _game.Pause();
        _game.SimulateCowTap(100f); // blocked
        _game.SimulateCowTap(100f); // blocked

        _game.Resume();
        _game.SimulateCowTap(100f); // left = 2
        _game.SimulateCowTap(100f); // left = 3

        AssertThat(_game.Scores.LeftScore).IsEqual(3);
    }
}
