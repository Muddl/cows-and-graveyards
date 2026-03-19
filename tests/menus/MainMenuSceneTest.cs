namespace CowsGraveyards.Tests.Menus;

using System.Collections.Generic;
using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class MainMenuSceneTest
{
    // ── Slot label tests ─────────────────────────────────────────────────────

    // Task 2.1 — slot buttons show correct labels

    [TestCase]
    public void EmptySlotLabelSaysEmpty()
    {
        var menu = AutoFree(new MainMenuScene())!;

        string label = menu.GetSlotLabel(null);

        AssertThat(label).IsEqual("Empty");
    }

    [TestCase]
    public void SavedSlotLabelShowsLeftAndRightScores()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var save = new TripSave(0, 5, 3);

        string label = menu.GetSlotLabel(save);

        AssertThat(label).IsEqual("L: 5  R: 3");
    }

    [TestCase]
    public void SavedSlotLabelShowsZeroScoresWhenBothAreZero()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var save = new TripSave(1, 0, 0);

        string label = menu.GetSlotLabel(save);

        AssertThat(label).IsEqual("L: 0  R: 0");
    }

    [TestCase]
    public void SavedSlotLabelReflectsHighScores()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var save = new TripSave(2, 99, 42);

        string label = menu.GetSlotLabel(save);

        AssertThat(label).IsEqual("L: 99  R: 42");
    }

    // ── Quit button visibility tests ─────────────────────────────────────────

    [TestCase]
    public void QuitButtonVisibleOnDesktop()
    {
        var menu = AutoFree(new MainMenuScene())!;
        menu.PlatformName = "Windows";

        AssertThat(menu.ShouldShowQuitButton()).IsTrue();
    }

    [TestCase]
    public void QuitButtonHiddenOnAndroid()
    {
        var menu = AutoFree(new MainMenuScene())!;
        menu.PlatformName = "Android";

        AssertThat(menu.ShouldShowQuitButton()).IsFalse();
    }

    [TestCase]
    public void QuitButtonHiddenOnIos()
    {
        var menu = AutoFree(new MainMenuScene())!;
        menu.PlatformName = "iOS";

        AssertThat(menu.ShouldShowQuitButton()).IsFalse();
    }

    [TestCase]
    public void QuitButtonVisibleOnLinux()
    {
        var menu = AutoFree(new MainMenuScene())!;
        menu.PlatformName = "Linux";

        AssertThat(menu.ShouldShowQuitButton()).IsTrue();
    }

    [TestCase]
    public void QuitButtonVisibleOnMacOS()
    {
        var menu = AutoFree(new MainMenuScene())!;
        menu.PlatformName = "macOS";

        AssertThat(menu.ShouldShowQuitButton()).IsTrue();
    }

    // ── Signal: NewTripRequested ──────────────────────────────────────────────

    [TestCase]
    public void NewTripRequestedSignalCarriesSlotIndex()
    {
        var menu = AutoFree(new MainMenuScene())!;

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnNewTripSlotPressed(0);

        AssertThat(received).IsNotNull();
        AssertThat(received).IsEqual(0);
    }

    [TestCase]
    public void NewTripRequestedSignalCarriesCorrectSlotForSlot1()
    {
        var menu = AutoFree(new MainMenuScene())!;

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnNewTripSlotPressed(1);

        AssertThat(received).IsEqual(1);
    }

    [TestCase]
    public void NewTripRequestedSignalCarriesCorrectSlotForSlot2()
    {
        var menu = AutoFree(new MainMenuScene())!;

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnNewTripSlotPressed(2);

        AssertThat(received).IsEqual(2);
    }

    // ── Signal: LoadTripRequested ─────────────────────────────────────────────

    [TestCase]
    public void LoadTripRequestedSignalCarriesTripSave()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var save = new TripSave(1, 7, 4);

        TripSave? received = null;
        menu.LoadTripRequested += (TripSave s) => received = s;
        menu.OnLoadTripSlotPressed(save);

        AssertThat(received).IsNotNull();
    }

    [TestCase]
    public void LoadTripRequestedSignalCarriesCorrectSlotIndex()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var save = new TripSave(2, 3, 8);

        TripSave? received = null;
        menu.LoadTripRequested += (TripSave s) => received = s;
        menu.OnLoadTripSlotPressed(save);

        AssertThat(received!.SlotIndex).IsEqual(2);
    }

    [TestCase]
    public void LoadTripRequestedSignalCarriesCorrectScores()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var save = new TripSave(0, 11, 22);

        TripSave? received = null;
        menu.LoadTripRequested += (TripSave s) => received = s;
        menu.OnLoadTripSlotPressed(save);

        AssertThat(received!.LeftScore).IsEqual(11);
        AssertThat(received.RightScore).IsEqual(22);
    }

    // ── HasAvailableSlot ──────────────────────────────────────────────────────

    [TestCase]
    public void HasAvailableSlotReturnsTrueWhenAllSlotsNull()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { null, null, null };

        AssertThat(menu.HasAvailableSlot(slots)).IsTrue();
    }

    [TestCase]
    public void HasAvailableSlotReturnsTrueWhenSomeSlotsNull()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { new TripSave(0, 1, 2), null, new TripSave(2, 3, 4) };

        AssertThat(menu.HasAvailableSlot(slots)).IsTrue();
    }

    [TestCase]
    public void HasAvailableSlotReturnsFalseWhenNoSlotsNull()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { new TripSave(0, 1, 2), new TripSave(1, 3, 4), new TripSave(2, 5, 6) };

        AssertThat(menu.HasAvailableSlot(slots)).IsFalse();
    }

    // ── HasAnySave ────────────────────────────────────────────────────────────

    [TestCase]
    public void HasAnySaveReturnsTrueWhenOneSaveExists()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { null, new TripSave(1, 5, 3), null };

        AssertThat(menu.HasAnySave(slots)).IsTrue();
    }

    [TestCase]
    public void HasAnySaveReturnsFalseWhenAllNull()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { null, null, null };

        AssertThat(menu.HasAnySave(slots)).IsFalse();
    }

    [TestCase]
    public void HasAnySaveReturnsTrueWhenAllSlotsPopulated()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { new TripSave(0, 1, 2), new TripSave(1, 3, 4), new TripSave(2, 5, 6) };

        AssertThat(menu.HasAnySave(slots)).IsTrue();
    }

    // ── OnStartTripPressed(slots) ─────────────────────────────────────────────

    [TestCase]
    public void OnStartTripPressedEmitsNewTripRequestedWithFirstFreeSlot()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { new TripSave(0, 1, 2), null, null };

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnStartTripPressed(slots);

        AssertThat(received).IsEqual(1);
    }

    [TestCase]
    public void OnStartTripPressedUsesFirstFreeSlotWhenSlot0IsFree()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { null, new TripSave(1, 3, 4), null };

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnStartTripPressed(slots);

        AssertThat(received).IsEqual(0);
    }

    [TestCase]
    public void OnStartTripPressedUsesLastSlotWhenOnlyLastIsFree()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { new TripSave(0, 1, 2), new TripSave(1, 3, 4), null };

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnStartTripPressed(slots);

        AssertThat(received).IsEqual(2);
    }

    [TestCase]
    public void OnStartTripPressedDoesNotEmitWhenAllSlotsFull()
    {
        var menu = AutoFree(new MainMenuScene())!;
        var slots = new List<TripSave?> { new TripSave(0, 1, 2), new TripSave(1, 3, 4), new TripSave(2, 5, 6) };

        int? received = null;
        menu.NewTripRequested += (int slot) => received = slot;
        menu.OnStartTripPressed(slots);

        AssertThat(received).IsNull();
    }
}
