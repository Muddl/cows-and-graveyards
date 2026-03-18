namespace CowsGraveyards.Tests.Menus;

using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class SaveManagerTest
{
    private const string TestSavePath = "user://test_trips.json";
    private SaveManager _manager = null!;

    [BeforeTest]
    public void Setup()
    {
        _manager = new SaveManager(TestSavePath);
        _manager.DeleteAll();
    }

    [AfterTest]
    public void Teardown()
    {
        _manager.DeleteAll();
    }

    // Task 1.2 — save a slot, load a slot, load all slots

    [TestCase]
    public void SaveAndLoadSlotRoundTripsScores()
    {
        var save = new TripSave(0, 5, 3);
        _manager.SaveSlot(save);

        var loaded = _manager.LoadSlot(0);

        AssertThat(loaded).IsNotNull();
        AssertThat(loaded!.LeftScore).IsEqual(5);
        AssertThat(loaded.RightScore).IsEqual(3);
    }

    [TestCase]
    public void SaveAndLoadSlotPreservesSlotIndex()
    {
        var save = new TripSave(1, 2, 4);
        _manager.SaveSlot(save);

        var loaded = _manager.LoadSlot(1);

        AssertThat(loaded).IsNotNull();
        AssertThat(loaded!.SlotIndex).IsEqual(1);
    }

    [TestCase]
    public void OverwritingSlotReplacesData()
    {
        _manager.SaveSlot(new TripSave(0, 1, 1));
        _manager.SaveSlot(new TripSave(0, 99, 88));

        var loaded = _manager.LoadSlot(0);

        AssertThat(loaded!.LeftScore).IsEqual(99);
        AssertThat(loaded.RightScore).IsEqual(88);
    }

    [TestCase]
    public void LoadAllSlotsReturnsAllSavedSlots()
    {
        _manager.SaveSlot(new TripSave(0, 1, 2));
        _manager.SaveSlot(new TripSave(1, 3, 4));
        _manager.SaveSlot(new TripSave(2, 5, 6));

        var all = _manager.LoadAllSlots();

        AssertThat(all.Count).IsEqual(3);
    }

    [TestCase]
    public void LoadAllSlotsReturnsCorrectDataPerSlot()
    {
        _manager.SaveSlot(new TripSave(0, 10, 20));
        _manager.SaveSlot(new TripSave(2, 30, 40));

        var all = _manager.LoadAllSlots();

        AssertThat(all[0]!.LeftScore).IsEqual(10);
        AssertThat(all[2]!.LeftScore).IsEqual(30);
    }

    [TestCase]
    public void LoadSlotReturnsNullOnMissingFile()
    {
        // No saves written — file does not exist
        var loaded = _manager.LoadSlot(0);

        AssertThat(loaded).IsNull();
    }

    [TestCase]
    public void LoadAllSlotsReturnsNullsForEmptySlots()
    {
        _manager.SaveSlot(new TripSave(1, 5, 5));

        var all = _manager.LoadAllSlots();

        AssertThat(all[0]).IsNull();
        AssertThat(all[1]).IsNotNull();
        AssertThat(all[2]).IsNull();
    }

    [TestCase]
    public void LoadSlotReturnsNullOnCorruptFile()
    {
        // Write garbage JSON to the save file
        using var file = Godot.FileAccess.Open(TestSavePath, Godot.FileAccess.ModeFlags.Write);
        file.StoreString("{ this is not valid json !!!");

        var loaded = _manager.LoadSlot(0);

        AssertThat(loaded).IsNull();
    }

    [TestCase]
    public void LoadAllSlotsReturnsNullsOnCorruptFile()
    {
        using var file = Godot.FileAccess.Open(TestSavePath, Godot.FileAccess.ModeFlags.Write);
        file.StoreString("corrupt data here");

        var all = _manager.LoadAllSlots();

        AssertThat(all[0]).IsNull();
        AssertThat(all[1]).IsNull();
        AssertThat(all[2]).IsNull();
    }

    [TestCase]
    public void MultipleSlotsAreIndependent()
    {
        _manager.SaveSlot(new TripSave(0, 1, 2));
        _manager.SaveSlot(new TripSave(1, 3, 4));
        _manager.SaveSlot(new TripSave(2, 5, 6));

        var s0 = _manager.LoadSlot(0)!;
        var s1 = _manager.LoadSlot(1)!;
        var s2 = _manager.LoadSlot(2)!;

        AssertThat(s0.LeftScore).IsEqual(1);
        AssertThat(s1.LeftScore).IsEqual(3);
        AssertThat(s2.LeftScore).IsEqual(5);
    }
}
