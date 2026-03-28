namespace CowsGraveyards.Tests.Audio;

using CowsGraveyards.Audio;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class AmbientAudioTest
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
    public void StartAmbientBeginsPlayback()
    {
        _manager.StartAmbient();

        AssertThat(_manager.IsAmbientPlaying).IsTrue();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StopAmbientStopsPlayback()
    {
        _manager.StartAmbient();
        _manager.StopAmbient();

        AssertThat(_manager.IsAmbientPlaying).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StopAmbientWithoutStartDoesNotThrow()
    {
        _manager.StopAmbient();

        AssertThat(_manager.IsAmbientPlaying).IsFalse();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void AmbientVolumeIsIndependent()
    {
        _manager.SetAmbientVolume(-10f);
        _manager.SetSfxVolume(0f);

        int ambientIdx = Godot.AudioServer.GetBusIndex("Ambient");
        int sfxIdx = Godot.AudioServer.GetBusIndex("SFX");

        float ambientDb = Godot.AudioServer.GetBusVolumeDb(ambientIdx);
        float sfxDb = Godot.AudioServer.GetBusVolumeDb(sfxIdx);

        AssertThat(ambientDb).IsNotEqual(sfxDb);
    }
}
