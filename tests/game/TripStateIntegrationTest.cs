namespace CowsGraveyards.Tests.Game;

using CowsGraveyards.Game;
using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Integration tests for trip slot initialisation and save-on-exit flow.
/// Tests the contract between GameScene, TripSave, and SaveManager.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class TripStateIntegrationTest
{
    private const string TestSavePath = "user://test_trip_state.json";
    private GameScene _game = null!;
    private SaveManager _saveManager = null!;

    [BeforeTest]
    public void Setup()
    {
        _game = AutoFree(new GameScene())!;
        _saveManager = new SaveManager(TestSavePath);
        _saveManager.DeleteAll();
    }

    [AfterTest]
    public void Teardown()
    {
        _saveManager.DeleteAll();
    }

    // ── Task 4.1 — InitTripSlot seeds ScoreTracker ───────────────────────────

    [TestCase]
    public void InitTripSlotWithSaveRestoresLeftScore()
    {
        var save = new TripSave(0, 7, 3);
        _game.InitTripSlot(0, save, _saveManager);

        AssertThat(_game.Scores.LeftScore).IsEqual(7);
    }

    [TestCase]
    public void InitTripSlotWithSaveRestoresRightScore()
    {
        var save = new TripSave(0, 7, 3);
        _game.InitTripSlot(0, save, _saveManager);

        AssertThat(_game.Scores.RightScore).IsEqual(3);
    }

    [TestCase]
    public void InitTripSlotWithNullSaveStartsAtZero()
    {
        _game.InitTripSlot(1, null, _saveManager);

        AssertThat(_game.Scores.LeftScore).IsEqual(0);
        AssertThat(_game.Scores.RightScore).IsEqual(0);
    }

    [TestCase]
    public void InitTripSlotStoresActiveSlotIndex()
    {
        _game.InitTripSlot(2, null, _saveManager);

        AssertThat(_game.ActiveSlot).IsEqual(2);
    }

    [TestCase]
    public void InitTripSlotSlot0StoresCorrectIndex()
    {
        _game.InitTripSlot(0, new TripSave(0, 1, 2), _saveManager);

        AssertThat(_game.ActiveSlot).IsEqual(0);
    }

    [TestCase]
    public void InitTripSlotSlot1StoresCorrectIndex()
    {
        _game.InitTripSlot(1, new TripSave(1, 5, 9), _saveManager);

        AssertThat(_game.ActiveSlot).IsEqual(1);
    }

    [TestCase]
    public void InitTripSlotAfterPlayingScoresArePreserved()
    {
        var save = new TripSave(0, 10, 5);
        _game.InitTripSlot(0, save, _saveManager);

        // Play a bit more after loading
        _game.SimulateCowTap(100f); // left + 1

        AssertThat(_game.Scores.LeftScore).IsEqual(11);
        AssertThat(_game.Scores.RightScore).IsEqual(5);
    }

    // ── Task 4.2 — SaveAndExit persists scores to SaveManager ────────────────

    [TestCase]
    public void SaveAndExitWritesLeftScoreToCorrectSlot()
    {
        _game.InitTripSlot(0, null, _saveManager);
        _game.SimulateCowTap(100f); // left = 1
        _game.SimulateCowTap(100f); // left = 2

        _game.SaveAndExit();

        var saved = _saveManager.LoadSlot(0);
        AssertThat(saved).IsNotNull();
        AssertThat(saved!.LeftScore).IsEqual(2);
    }

    [TestCase]
    public void SaveAndExitWritesRightScoreToCorrectSlot()
    {
        _game.InitTripSlot(0, null, _saveManager);
        _game.SimulateCowTap(800f); // right = 1
        _game.SimulateCowTap(800f); // right = 2
        _game.SimulateCowTap(800f); // right = 3

        _game.SaveAndExit();

        var saved = _saveManager.LoadSlot(0);
        AssertThat(saved!.RightScore).IsEqual(3);
    }

    [TestCase]
    public void SaveAndExitSavesToActiveSlotIndex()
    {
        _game.InitTripSlot(2, null, _saveManager);
        _game.SimulateCowTap(100f); // left = 1

        _game.SaveAndExit();

        AssertThat(_saveManager.LoadSlot(0)).IsNull();
        AssertThat(_saveManager.LoadSlot(1)).IsNull();
        AssertThat(_saveManager.LoadSlot(2)).IsNotNull();
    }

    [TestCase]
    public void SaveAndExitOverwritesPreviousSave()
    {
        _saveManager.SaveSlot(new TripSave(1, 99, 88));
        _game.InitTripSlot(1, new TripSave(1, 99, 88), _saveManager);
        _game.SimulateCowTap(100f); // left was 99, now 100

        _game.SaveAndExit();

        var saved = _saveManager.LoadSlot(1);
        AssertThat(saved!.LeftScore).IsEqual(100);
    }

    [TestCase]
    public void SaveAndExitPreservesSlotIndexInSaveData()
    {
        _game.InitTripSlot(2, null, _saveManager);
        _game.SaveAndExit();

        var saved = _saveManager.LoadSlot(2);
        AssertThat(saved!.SlotIndex).IsEqual(2);
    }
}
