namespace CowsGraveyards.Tests.Audio;

using CowsGraveyards.Audio;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class CoreSfxTest
{
    private AudioManager _manager = null!;

    [BeforeTest]
    public void Setup()
    {
        _manager = new AudioManager();
        _manager.RegisterSfx("tap", null);
        _manager.RegisterSfx("cow_moo", null);
        _manager.RegisterSfx("gravestone_thud", null);
    }

    [AfterTest]
    public void Teardown()
    {
        _manager.Free();
    }

    // ── Tap SFX ──────────────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxTapRecordsPlay()
    {
        _manager.PlaySfx("tap");

        AssertThat(_manager.LastPlayedSfxKey).IsEqual("tap");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxTapIncrementsTotalPlayed()
    {
        int before = _manager.TotalSfxPlayed;

        _manager.PlaySfx("tap");

        AssertThat(_manager.TotalSfxPlayed).IsEqual(before + 1);
    }

    // ── Cow moo SFX ─────────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxCowMooRecordsPlay()
    {
        _manager.PlaySfx("cow_moo");

        AssertThat(_manager.LastPlayedSfxKey).IsEqual("cow_moo");
    }

    // ── Gravestone thud SFX ──────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxGravestoneThudRecordsPlay()
    {
        _manager.PlaySfx("gravestone_thud");

        AssertThat(_manager.LastPlayedSfxKey).IsEqual("gravestone_thud");
    }

    // ── Unknown key handling ─────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxUnknownKeyDoesNotUpdateLastPlayed()
    {
        _manager.PlaySfx("tap");
        _manager.PlaySfx("unknown_key");

        AssertThat(_manager.LastPlayedSfxKey).IsEqual("tap");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxUnknownKeyDoesNotIncrementCount()
    {
        _manager.PlaySfx("tap");
        int countAfterTap = _manager.TotalSfxPlayed;

        _manager.PlaySfx("unknown_key");

        AssertThat(_manager.TotalSfxPlayed).IsEqual(countAfterTap);
    }
}
