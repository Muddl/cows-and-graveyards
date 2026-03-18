namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class SideDetectorTest
{
    private SideDetector _detector = null!;

    [BeforeTest]
    public void Setup()
    {
        _detector = new SideDetector();
    }

    [TestCase]
    public void TapOnLeftSideReturnLeft()
    {
        var result = _detector.Detect(100f, 1080f);

        AssertThat(result).IsEqual(TapSide.Left);
    }

    [TestCase]
    public void TapOnRightSideReturnRight()
    {
        var result = _detector.Detect(800f, 1080f);

        AssertThat(result).IsEqual(TapSide.Right);
    }

    [TestCase]
    public void TapAtExactCenterReturnRight()
    {
        var result = _detector.Detect(540f, 1080f);

        AssertThat(result).IsEqual(TapSide.Right);
    }

    [TestCase]
    public void TapJustLeftOfCenterReturnLeft()
    {
        var result = _detector.Detect(539f, 1080f);

        AssertThat(result).IsEqual(TapSide.Left);
    }

    [TestCase]
    public void TapAtLeftEdgeReturnLeft()
    {
        var result = _detector.Detect(0f, 1080f);

        AssertThat(result).IsEqual(TapSide.Left);
    }

    [TestCase]
    public void TapAtRightEdgeReturnRight()
    {
        var result = _detector.Detect(1079f, 1080f);

        AssertThat(result).IsEqual(TapSide.Right);
    }

    [TestCase]
    public void WorksWithDifferentScreenWidths()
    {
        var result = _detector.Detect(100f, 720f);

        AssertThat(result).IsEqual(TapSide.Left);
    }
}
