namespace CowsGraveyards.Tests.Audio;

using CowsGraveyards.Audio;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class AudioManagerTest
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

    // ── Bus structure ────────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void SfxBusExistsWithCorrectIndex()
    {
        int index = AudioServer.GetBusIndex("SFX");
        AssertThat(index).IsGreaterEqual(0);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void MusicBusExistsWithCorrectIndex()
    {
        int index = AudioServer.GetBusIndex("Music");
        AssertThat(index).IsGreaterEqual(0);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void AmbientBusExistsWithCorrectIndex()
    {
        int index = AudioServer.GetBusIndex("Ambient");
        AssertThat(index).IsGreaterEqual(0);
    }

    // ── PlaySfx API ──────────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxDoesNotThrowOnValidKey()
    {
        // Register a key with null stream (no actual audio in test)
        _manager.RegisterSfx("tap", null);
        _manager.PlaySfx("tap");
        AssertThat(_manager).IsNotNull();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxDoesNotThrowOnUnknownKey()
    {
        // Should handle gracefully (log warning, no exception)
        _manager.PlaySfx("nonexistent_sound");
        AssertThat(_manager).IsNotNull();
    }

    // ── Volume control ───────────────────────────────────────────────────────

    [TestCase]
    [RequireGodotRuntime]
    public void SetBusVolumeClampsToValidRange()
    {
        _manager.SetBusVolume("SFX", 100f);
        float dbAfterHigh = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("SFX"));
        AssertThat(dbAfterHigh).IsLessEqual(6f);

        _manager.SetBusVolume("SFX", -100f);
        float dbAfterLow = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("SFX"));
        AssertThat(dbAfterLow).IsGreaterEqual(-80f);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void SetBusVolumeAffectsCorrectBus()
    {
        float originalMusic = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music"));

        _manager.SetBusVolume("SFX", -10f);

        float musicAfter = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music"));
        AssertThat(musicAfter).IsEqual(originalMusic);
    }
}
