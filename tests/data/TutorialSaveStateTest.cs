namespace CowsGraveyards.Tests.Data;

using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class TutorialSaveStateTest
{
    [TestCase]
    [RequireGodotRuntime]
    public void TutorialSeenFlagWrittenToSaveData()
    {
        var save = new TripSave(0, 5, 3, tutorialSeen: true, graveyardExplained: false);

        AssertThat(save.TutorialSeen).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void TutorialSeenDefaultsToFalse()
    {
        var save = new TripSave(0, 5, 3);

        AssertThat(save.TutorialSeen).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GraveyardExplainedFlagWrittenToSaveData()
    {
        var save = new TripSave(0, 5, 3, tutorialSeen: false, graveyardExplained: true);

        AssertThat(save.GraveyardExplained).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GraveyardExplainedDefaultsToFalse()
    {
        var save = new TripSave(0, 5, 3);

        AssertThat(save.GraveyardExplained).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void TutorialSeenSurvivesRoundTrip()
    {
        var original = new TripSave(1, 10, 20, tutorialSeen: true, graveyardExplained: false);
        var dict = original.ToDictionary();
        var restored = TripSave.FromDictionary(dict);

        AssertThat(restored.TutorialSeen).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void GraveyardExplainedSurvivesRoundTrip()
    {
        var original = new TripSave(2, 7, 4, tutorialSeen: false, graveyardExplained: true);
        var dict = original.ToDictionary();
        var restored = TripSave.FromDictionary(dict);

        AssertThat(restored.GraveyardExplained).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void BothFlagsSurviveRoundTrip()
    {
        var original = new TripSave(0, 1, 2, tutorialSeen: true, graveyardExplained: true);
        var dict = original.ToDictionary();
        var restored = TripSave.FromDictionary(dict);

        AssertThat(restored.TutorialSeen).IsTrue();
        AssertThat(restored.GraveyardExplained).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ResetTutorialClearsBothFlags()
    {
        var save = new TripSave(0, 5, 3, tutorialSeen: true, graveyardExplained: true);
        var reset = save.WithTutorialReset();

        AssertThat(reset.TutorialSeen).IsFalse();
        AssertThat(reset.GraveyardExplained).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void ResetTutorialPreservesScores()
    {
        var save = new TripSave(1, 10, 20, tutorialSeen: true, graveyardExplained: true);
        var reset = save.WithTutorialReset();

        AssertThat(reset.SlotIndex).IsEqual(1);
        AssertThat(reset.LeftScore).IsEqual(10);
        AssertThat(reset.RightScore).IsEqual(20);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void LegacySaveWithoutFlagsDefaultsToFalse()
    {
        // Simulate a save file from before tutorial flags existed
        var dict = new Godot.Collections.Dictionary
        {
            { "slot", 0 },
            { "left", 5 },
            { "right", 3 },
        };
        var restored = TripSave.FromDictionary(dict);

        AssertThat(restored.TutorialSeen).IsFalse();
        AssertThat(restored.GraveyardExplained).IsFalse();
    }
}
