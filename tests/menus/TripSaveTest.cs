namespace CowsGraveyards.Tests.Menus;

using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class TripSaveTest
{
    // Task 1.1 — TripSave stores slot index (0–2), left score, right score

    [TestCase]
    public void DefaultConstructorStoresZeroValues()
    {
        var save = new TripSave(0, 0, 0);

        AssertThat(save.SlotIndex).IsEqual(0);
        AssertThat(save.LeftScore).IsEqual(0);
        AssertThat(save.RightScore).IsEqual(0);
    }

    [TestCase]
    public void StoresSlotIndexCorrectly()
    {
        var save = new TripSave(2, 5, 3);

        AssertThat(save.SlotIndex).IsEqual(2);
    }

    [TestCase]
    public void StoresLeftScoreCorrectly()
    {
        var save = new TripSave(0, 7, 0);

        AssertThat(save.LeftScore).IsEqual(7);
    }

    [TestCase]
    public void StoresRightScoreCorrectly()
    {
        var save = new TripSave(0, 0, 12);

        AssertThat(save.RightScore).IsEqual(12);
    }

    [TestCase]
    public void SlotIndexAcceptsMinimumValue()
    {
        var save = new TripSave(0, 0, 0);

        AssertThat(save.SlotIndex).IsEqual(0);
    }

    [TestCase]
    public void SlotIndexAcceptsMaximumValue()
    {
        var save = new TripSave(2, 0, 0);

        AssertThat(save.SlotIndex).IsEqual(2);
    }

    [TestCase]
    public void ToDictionaryContainsSlotIndex()
    {
        var save = new TripSave(1, 4, 6);
        var dict = save.ToDictionary();

        AssertThat(dict.ContainsKey("slot")).IsTrue();
        AssertThat(dict["slot"].AsInt32()).IsEqual(1);
    }

    [TestCase]
    public void ToDictionaryContainsLeftScore()
    {
        var save = new TripSave(0, 4, 6);
        var dict = save.ToDictionary();

        AssertThat(dict.ContainsKey("left")).IsTrue();
        AssertThat(dict["left"].AsInt32()).IsEqual(4);
    }

    [TestCase]
    public void ToDictionaryContainsRightScore()
    {
        var save = new TripSave(0, 4, 6);
        var dict = save.ToDictionary();

        AssertThat(dict.ContainsKey("right")).IsTrue();
        AssertThat(dict["right"].AsInt32()).IsEqual(6);
    }

    [TestCase]
    public void FromDictionaryReconstructsSlotIndex()
    {
        var original = new TripSave(2, 3, 9);
        var roundTripped = TripSave.FromDictionary(original.ToDictionary());

        AssertThat(roundTripped.SlotIndex).IsEqual(2);
    }

    [TestCase]
    public void FromDictionaryReconstructsLeftScore()
    {
        var original = new TripSave(0, 3, 9);
        var roundTripped = TripSave.FromDictionary(original.ToDictionary());

        AssertThat(roundTripped.LeftScore).IsEqual(3);
    }

    [TestCase]
    public void FromDictionaryReconstructsRightScore()
    {
        var original = new TripSave(0, 3, 9);
        var roundTripped = TripSave.FromDictionary(original.ToDictionary());

        AssertThat(roundTripped.RightScore).IsEqual(9);
    }

    [TestCase]
    public void RoundTripPreservesAllValues()
    {
        var original = new TripSave(1, 42, 17);
        var roundTripped = TripSave.FromDictionary(original.ToDictionary());

        AssertThat(roundTripped.SlotIndex).IsEqual(1);
        AssertThat(roundTripped.LeftScore).IsEqual(42);
        AssertThat(roundTripped.RightScore).IsEqual(17);
    }
}
