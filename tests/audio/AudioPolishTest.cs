namespace CowsGraveyards.Tests.Audio;

using CowsGraveyards.Audio;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class AudioPolishTest
{
    private AudioManager _manager = null!;

    [BeforeTest]
    public void Setup()
    {
        _manager = new AudioManager();
    }

    [AfterTest]
    public void Teardown()
    {
        _manager.Free();
    }

    // ── SFX Pooling ──────────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void RapidSfxPlaysAreNotCutOff()
    {
        // Register a test SFX key (null stream is fine — we check play counts)
        _manager.RegisterSfx("tap", null);

        // Fire 4 rapid taps in succession
        _manager.PlaySfx("tap");
        _manager.PlaySfx("tap");
        _manager.PlaySfx("tap");
        _manager.PlaySfx("tap");

        // All 4 should have been counted (none cut off)
        AssertThat(_manager.TotalSfxPlayed).IsEqual(4);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void SfxPoolSizeIsAtLeastFour()
    {
        // The pool should support at least 4 simultaneous SFX players
        AssertThat(_manager.SfxPoolSize).IsGreaterEqual(4);
    }

    // ── Audio Focus ──────────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void HandleFocusLossPausesAllStreams()
    {
        _manager.RegisterSfx("tap", null);
        _manager.StartAmbient();
        _manager.PlayMusic("menu_theme");

        _manager.HandleFocusLoss();

        AssertThat(_manager.IsAmbientPlaying).IsFalse();
        AssertThat(_manager.IsMusicPlaying).IsFalse();
        AssertThat(_manager.IsPaused).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void HandleFocusGainResumesStreams()
    {
        _manager.StartAmbient();
        _manager.PlayMusic("gameplay_theme");

        _manager.HandleFocusLoss();
        _manager.HandleFocusGain();

        AssertThat(_manager.IsAmbientPlaying).IsTrue();
        AssertThat(_manager.IsMusicPlaying).IsTrue();
        AssertThat(_manager.IsPaused).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void HandleFocusGainWithoutLossDoesNotThrow()
    {
        // Should be safe to call even when not paused
        _manager.HandleFocusGain();
        AssertThat(_manager.IsPaused).IsFalse();
    }

    // ── Bus Init Defaults ────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void AllBusesDefaultToValidDbValues()
    {
        int sfxIndex = AudioServer.GetBusIndex("SFX");
        int musicIndex = AudioServer.GetBusIndex("Music");
        int ambientIndex = AudioServer.GetBusIndex("Ambient");

        // All buses should exist
        AssertThat(sfxIndex).IsGreaterEqual(0);
        AssertThat(musicIndex).IsGreaterEqual(0);
        AssertThat(ambientIndex).IsGreaterEqual(0);

        // None should be at -inf (which is below -80 dB)
        float sfxDb = AudioServer.GetBusVolumeDb(sfxIndex);
        float musicDb = AudioServer.GetBusVolumeDb(musicIndex);
        float ambientDb = AudioServer.GetBusVolumeDb(ambientIndex);

        AssertThat(sfxDb).IsGreater(-80f);
        AssertThat(musicDb).IsGreater(-80f);
        AssertThat(ambientDb).IsGreater(-80f);
    }
}
