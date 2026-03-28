namespace CowsGraveyards.Audio;

using System.Collections.Generic;
using Godot;

public partial class AudioManager : Node
{
    private const float MaxVolumeDb = 6f;
    private const float MinVolumeDb = -80f;

    private readonly Dictionary<string, AudioStream?> _sfxRegistry = new();

    public AudioManager()
    {
        EnsureBusLayout();
    }

    public override void _Ready()
    {
    }

    /// <summary>
    /// Plays a registered SFX by key on the SFX bus.
    /// Logs a warning if the key is not registered.
    /// </summary>
    public void PlaySfx(string key)
    {
        if (!_sfxRegistry.TryGetValue(key, out var stream))
        {
            GD.PushWarning($"AudioManager: unknown SFX key '{key}'");
            return;
        }

        if (stream is null)
        {
            return;
        }

        var player = new AudioStreamPlayer();
        player.Stream = stream;
        player.Bus = "SFX";
        AddChild(player);
        player.Play();
        player.Finished += () => player.QueueFree();
    }

    /// <summary>
    /// Registers an audio stream for a given SFX key.
    /// </summary>
    public void RegisterSfx(string key, AudioStream? stream)
    {
        _sfxRegistry[key] = stream;
    }

    /// <summary>
    /// Sets the volume for a named audio bus, clamping to a valid dB range.
    /// </summary>
    public void SetBusVolume(string busName, float db)
    {
        int index = AudioServer.GetBusIndex(busName);
        if (index < 0)
        {
            GD.PushWarning($"AudioManager: unknown bus '{busName}'");
            return;
        }

        float clamped = Mathf.Clamp(db, MinVolumeDb, MaxVolumeDb);
        AudioServer.SetBusVolumeDb(index, clamped);
    }

    /// <summary>Sets SFX bus volume.</summary>
    public void SetSfxVolume(float db) => SetBusVolume("SFX", db);

    /// <summary>Sets Music bus volume.</summary>
    public void SetMusicVolume(float db) => SetBusVolume("Music", db);

    /// <summary>Sets Ambient bus volume.</summary>
    public void SetAmbientVolume(float db) => SetBusVolume("Ambient", db);

    private static void EnsureBusLayout()
    {
        EnsureBus("SFX");
        EnsureBus("Music");
        EnsureBus("Ambient");
    }

    private static void EnsureBus(string busName)
    {
        if (AudioServer.GetBusIndex(busName) >= 0)
        {
            return;
        }

        int count = AudioServer.BusCount;
        AudioServer.AddBus();
        AudioServer.SetBusName(count, busName);
        AudioServer.SetBusSend(count, "Master");
    }
}
