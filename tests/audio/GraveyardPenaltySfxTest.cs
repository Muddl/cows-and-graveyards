namespace CowsGraveyards.Tests.Audio;

using CowsGraveyards.Audio;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class GraveyardPenaltySfxTest
{
    private AudioManager _manager = null!;

    [BeforeTest]
    public void Setup()
    {
        _manager = new AudioManager();
        _manager.RegisterSfx("gravestone_thud", null);
        _manager.RegisterSfx("graveyard_penalty", null);
    }

    [AfterTest]
    public void Teardown()
    {
        _manager.Free();
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PlaySfxGraveyardPenaltyRecordsPlay()
    {
        _manager.PlaySfx("graveyard_penalty");

        AssertThat(_manager.LastPlayedSfxKey).IsEqual("graveyard_penalty");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PenaltySoundKeyIsDifferentFromSpawnSound()
    {
        AssertThat("graveyard_penalty").IsNotEqual("gravestone_thud");
    }

    [TestCase]
    [RequireGodotRuntime]
    public void PenaltySoundPlaysIndependently()
    {
        _manager.PlaySfx("gravestone_thud");
        AssertThat(_manager.LastPlayedSfxKey).IsEqual("gravestone_thud");

        _manager.PlaySfx("graveyard_penalty");
        AssertThat(_manager.LastPlayedSfxKey).IsEqual("graveyard_penalty");
    }
}
