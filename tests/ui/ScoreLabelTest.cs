namespace CowsGraveyards.Tests.UI;

using CowsGraveyards.Game;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class ScoreLabelTest
{
    private ScoreHud _hud = null!;

    [BeforeTest]
    public void Setup()
    {
        _hud = AutoFree(new ScoreHud())!;
    }

    [TestCase]
    [RequireGodotRuntime]
    public void LeftLabelContainsHumanReadableIdentifier()
    {
        AssertThat(_hud.LeftScoreText).Contains("Left");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void RightLabelContainsHumanReadableIdentifier()
    {
        AssertThat(_hud.RightScoreText).Contains("Right");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void LeftAndRightLabelsHaveDistinctColors()
    {
        AssertThat(_hud.LeftLabelColor).IsNotEqual(_hud.RightLabelColor);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void LabelsUpdateCorrectlyWhenScoreChanges()
    {
        _hud.UpdateScores(5, 3);

        AssertThat(_hud.LeftScoreText).IsEqual("Left: 5");
        AssertThat(_hud.RightScoreText).IsEqual("Right: 3");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void LabelsReflectZeroScoresWithNewFormat()
    {
        _hud.UpdateScores(0, 0);

        AssertThat(_hud.LeftScoreText).IsEqual("Left: 0");
        AssertThat(_hud.RightScoreText).IsEqual("Right: 0");
    }
}
