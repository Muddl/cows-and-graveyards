namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class GameStateTest
{
    private GameState _state = null!;

    [BeforeTest]
    public void Setup()
    {
        _state = new GameState();
    }

    [TestCase]
    public void InitializesWithPlayingStatus()
    {
        AssertThat(_state.IsPlaying).IsTrue();
    }

    [TestCase]
    public void HasScoreTracker()
    {
        AssertThat(_state.Scores).IsNotNull();
    }

    [TestCase]
    public void ScoresStartAtZero()
    {
        AssertThat(_state.Scores.LeftScore).IsEqual(0);
        AssertThat(_state.Scores.RightScore).IsEqual(0);
    }

    [TestCase]
    public void ResetClearsScores()
    {
        _state.Scores.IncrementLeft();
        _state.Scores.IncrementRight();

        _state.Reset();

        AssertThat(_state.Scores.LeftScore).IsEqual(0);
        AssertThat(_state.Scores.RightScore).IsEqual(0);
    }

    [TestCase]
    public void ResetKeepsPlayingState()
    {
        _state.Reset();

        AssertThat(_state.IsPlaying).IsTrue();
    }
}
