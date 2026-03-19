namespace CowsGraveyards.Tests.Menus;

using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class TripHistoryManagerTest
{
    private const string TestHistoryPath = "user://test_trip_history.json";
    private TripHistoryManager _manager = null!;

    [BeforeTest]
    public void Setup()
    {
        _manager = new TripHistoryManager(TestHistoryPath);
        _manager.DeleteAll();
    }

    [AfterTest]
    public void Teardown()
    {
        _manager.DeleteAll();
    }

    [TestCase]
    public void LoadAllReturnsEmptyListWhenNoFile()
    {
        var all = _manager.LoadAll();

        AssertThat(all.Count).IsEqual(0);
    }

    [TestCase]
    public void AppendAndLoadAllRoundTripsOneRecord()
    {
        var record = new CompletedTripRecord(5, 3, 1700000000L);
        _manager.Append(record);

        var all = _manager.LoadAll();

        AssertThat(all.Count).IsEqual(1);
        AssertThat(all[0].LeftScore).IsEqual(5);
        AssertThat(all[0].RightScore).IsEqual(3);
        AssertThat(all[0].CompletedAt).IsEqual(1700000000L);
    }

    [TestCase]
    public void AppendMultipleRecordsPreservesOrder()
    {
        _manager.Append(new CompletedTripRecord(1, 2, 100L));
        _manager.Append(new CompletedTripRecord(3, 4, 200L));

        var all = _manager.LoadAll();

        AssertThat(all.Count).IsEqual(2);
        AssertThat(all[0].LeftScore).IsEqual(1);
        AssertThat(all[1].LeftScore).IsEqual(3);
    }

    [TestCase]
    public void AppendAccumulatesAcrossMultipleCalls()
    {
        _manager.Append(new CompletedTripRecord(1, 2, 100L));
        _manager.Append(new CompletedTripRecord(3, 4, 200L));
        _manager.Append(new CompletedTripRecord(5, 6, 300L));

        var all = _manager.LoadAll();

        AssertThat(all.Count).IsEqual(3);
    }

    [TestCase]
    public void LoadAllReturnsEmptyListOnCorruptFile()
    {
        using var file = Godot.FileAccess.Open(TestHistoryPath, Godot.FileAccess.ModeFlags.Write);
        file.StoreString("{ not valid json }");

        var all = _manager.LoadAll();

        AssertThat(all.Count).IsEqual(0);
    }

    [TestCase]
    public void DeleteAllRemovesAllRecords()
    {
        _manager.Append(new CompletedTripRecord(1, 2, 100L));
        _manager.DeleteAll();

        var all = _manager.LoadAll();

        AssertThat(all.Count).IsEqual(0);
    }

    [TestCase]
    public void RecordScoresRoundTripCorrectly()
    {
        _manager.Append(new CompletedTripRecord(42, 99, 9999L));

        var all = _manager.LoadAll();

        AssertThat(all[0].LeftScore).IsEqual(42);
        AssertThat(all[0].RightScore).IsEqual(99);
        AssertThat(all[0].CompletedAt).IsEqual(9999L);
    }
}
