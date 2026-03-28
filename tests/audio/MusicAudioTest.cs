namespace CowsGraveyards.Tests.Audio;

using CowsGraveyards.Audio;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class MusicAudioTest
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

    [TestCase]
    [RequireGodotRuntime]
    public void PlayMusicBeginsPlayback()
    {
        _manager.PlayMusic("menu_theme");

        AssertThat(_manager.IsMusicPlaying).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PlayMusicRecordsCurrentTrack()
    {
        _manager.PlayMusic("gameplay_theme");

        AssertThat(_manager.CurrentMusicTrack).IsEqual("gameplay_theme");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StopMusicStopsPlayback()
    {
        _manager.PlayMusic("menu_theme");
        _manager.StopMusic();

        AssertThat(_manager.IsMusicPlaying).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StopMusicWithoutPlayDoesNotThrow()
    {
        _manager.StopMusic();

        AssertThat(_manager.IsMusicPlaying).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void MusicVolumeIsIndependentFromSfx()
    {
        _manager.SetMusicVolume(-15f);
        _manager.SetSfxVolume(0f);

        int musicIdx = Godot.AudioServer.GetBusIndex("Music");
        int sfxIdx = Godot.AudioServer.GetBusIndex("SFX");

        float musicDb = Godot.AudioServer.GetBusVolumeDb(musicIdx);
        float sfxDb = Godot.AudioServer.GetBusVolumeDb(sfxIdx);

        AssertThat(musicDb).IsNotEqual(sfxDb);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void SwitchingTrackUpdatesCurrentTrack()
    {
        _manager.PlayMusic("menu_theme");
        _manager.PlayMusic("gameplay_theme");

        AssertThat(_manager.CurrentMusicTrack).IsEqual("gameplay_theme");
    }
}
