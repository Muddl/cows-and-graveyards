namespace CowsGraveyards.Tests.Menus;

using CowsGraveyards.Menus;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class PauseMenuTest
{
    // Task 3.1 — ResumeRequested and MainMenuRequested signals

    [TestCase]
    public void ResumeRequestedSignalFiresOnResumePressed()
    {
        var menu = AutoFree(new PauseMenu())!;

        bool fired = false;
        menu.ResumeRequested += () => fired = true;
        menu.OnResumePressed();

        AssertThat(fired).IsTrue();
    }

    [TestCase]
    public void MainMenuRequestedSignalFiresOnMainMenuPressed()
    {
        var menu = AutoFree(new PauseMenu())!;

        bool fired = false;
        menu.MainMenuRequested += () => fired = true;
        menu.OnMainMenuPressed();

        AssertThat(fired).IsTrue();
    }

    [TestCase]
    public void ResumeDoesNotFireMainMenuRequested()
    {
        var menu = AutoFree(new PauseMenu())!;

        bool fired = false;
        menu.MainMenuRequested += () => fired = true;
        menu.OnResumePressed();

        AssertThat(fired).IsFalse();
    }

    [TestCase]
    public void MainMenuDoesNotFireResumeRequested()
    {
        var menu = AutoFree(new PauseMenu())!;

        bool fired = false;
        menu.ResumeRequested += () => fired = true;
        menu.OnMainMenuPressed();

        AssertThat(fired).IsFalse();
    }

    // Task 3.1 — Quit button absent on mobile

    [TestCase]
    public void QuitButtonVisibleOnDesktop()
    {
        var menu = AutoFree(new PauseMenu())!;
        menu.PlatformName = "Windows";

        AssertThat(menu.ShouldShowQuitButton()).IsTrue();
    }

    [TestCase]
    public void QuitButtonHiddenOnAndroid()
    {
        var menu = AutoFree(new PauseMenu())!;
        menu.PlatformName = "Android";

        AssertThat(menu.ShouldShowQuitButton()).IsFalse();
    }

    [TestCase]
    public void QuitButtonHiddenOnIos()
    {
        var menu = AutoFree(new PauseMenu())!;
        menu.PlatformName = "iOS";

        AssertThat(menu.ShouldShowQuitButton()).IsFalse();
    }

    // CompleteTrip signal

    [TestCase]
    public void CompleteTripRequestedSignalFiresOnCompleteTripPressed()
    {
        var menu = AutoFree(new PauseMenu())!;

        bool fired = false;
        menu.CompleteTripRequested += () => fired = true;
        menu.OnCompleteTripPressed();

        AssertThat(fired).IsTrue();
    }

    [TestCase]
    public void CompleteTripDoesNotFireResumeRequested()
    {
        var menu = AutoFree(new PauseMenu())!;

        bool fired = false;
        menu.ResumeRequested += () => fired = true;
        menu.OnCompleteTripPressed();

        AssertThat(fired).IsFalse();
    }
}
