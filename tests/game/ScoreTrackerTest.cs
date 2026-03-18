namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class ScoreTrackerTest
{
    private ScoreTracker _tracker = null!;

    [BeforeTest]
    public void Setup()
    {
        _tracker = new ScoreTracker();
    }

    [TestCase]
    public void InitializesLeftScoreToZero()
    {
        AssertThat(_tracker.LeftScore).IsEqual(0);
    }

    [TestCase]
    public void InitializesRightScoreToZero()
    {
        AssertThat(_tracker.RightScore).IsEqual(0);
    }

    [TestCase]
    public void IncrementLeftScore()
    {
        _tracker.IncrementLeft();

        AssertThat(_tracker.LeftScore).IsEqual(1);
    }

    [TestCase]
    public void IncrementRightScore()
    {
        _tracker.IncrementRight();

        AssertThat(_tracker.RightScore).IsEqual(1);
    }

    [TestCase]
    public void TracksLeftAndRightIndependently()
    {
        _tracker.IncrementLeft();
        _tracker.IncrementLeft();
        _tracker.IncrementRight();

        AssertThat(_tracker.LeftScore).IsEqual(2);
        AssertThat(_tracker.RightScore).IsEqual(1);
    }

    [TestCase]
    public void IncrementMultipleTimes()
    {
        for (int i = 0; i < 5; i++)
        {
            _tracker.IncrementLeft();
        }

        AssertThat(_tracker.LeftScore).IsEqual(5);
        AssertThat(_tracker.RightScore).IsEqual(0);
    }

    [TestCase]
    public void ResetScores()
    {
        _tracker.IncrementLeft();
        _tracker.IncrementRight();
        _tracker.IncrementRight();

        _tracker.Reset();

        AssertThat(_tracker.LeftScore).IsEqual(0);
        AssertThat(_tracker.RightScore).IsEqual(0);
    }

    // Task 1.1 — ZeroLeft

    [TestCase]
    public void ZeroLeftSetsLeftScoreToZero()
    {
        _tracker.IncrementLeft();
        _tracker.IncrementLeft();
        _tracker.IncrementLeft();

        _tracker.ZeroLeft();

        AssertThat(_tracker.LeftScore).IsEqual(0);
    }

    [TestCase]
    public void ZeroLeftPreservesRightScore()
    {
        _tracker.IncrementRight();
        _tracker.IncrementRight();
        _tracker.IncrementLeft();

        _tracker.ZeroLeft();

        AssertThat(_tracker.RightScore).IsEqual(2);
    }

    [TestCase]
    public void ZeroLeftWhenAlreadyZeroIsIdempotent()
    {
        _tracker.ZeroLeft();

        AssertThat(_tracker.LeftScore).IsEqual(0);
        AssertThat(_tracker.RightScore).IsEqual(0);
    }

    // Task 1.2 — ZeroRight

    [TestCase]
    public void ZeroRightSetsRightScoreToZero()
    {
        _tracker.IncrementRight();
        _tracker.IncrementRight();
        _tracker.IncrementRight();

        _tracker.ZeroRight();

        AssertThat(_tracker.RightScore).IsEqual(0);
    }

    [TestCase]
    public void ZeroRightPreservesLeftScore()
    {
        _tracker.IncrementLeft();
        _tracker.IncrementLeft();
        _tracker.IncrementRight();

        _tracker.ZeroRight();

        AssertThat(_tracker.LeftScore).IsEqual(2);
    }

    [TestCase]
    public void ZeroRightWhenAlreadyZeroIsIdempotent()
    {
        _tracker.ZeroRight();

        AssertThat(_tracker.LeftScore).IsEqual(0);
        AssertThat(_tracker.RightScore).IsEqual(0);
    }
}
