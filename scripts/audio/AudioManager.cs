namespace CowsGraveyards.Audio;

using System.Collections.Generic;
using Godot;

public partial class AudioManager : Node
{
    private const float MaxVolumeDb = 6f;
    private const float MinVolumeDb = -80f;
    private const int PoolSize = 4;

    private readonly Dictionary<string, AudioStream?> _sfxRegistry = new();
    private readonly AudioStreamPlayer?[] _sfxPool = new AudioStreamPlayer?[PoolSize];
    private int _nextPoolIndex;

    /// <summary>The key of the most recently played SFX (for test observability).</summary>
    public string? LastPlayedSfxKey { get; private set; }

    /// <summary>Total number of SFX plays since creation (for test observability).</summary>
    public int TotalSfxPlayed { get; private set; }

    /// <summary>Number of SFX pool slots available for simultaneous playback.</summary>
    public int SfxPoolSize => PoolSize;

    /// <summary>Whether audio is paused due to focus loss.</summary>
    public bool IsPaused { get; private set; }

    private AudioStreamPlayer? _ambientPlayer;
    private bool _ambientIsActive;
    private bool _ambientWasActive;
    private bool _musicWasActive;
    private string? _pausedMusicTrack;

    /// <summary>Whether ambient audio is currently playing.</summary>
    public bool IsAmbientPlaying => _ambientIsActive;

    private AudioStreamPlayer? _musicPlayer;
    private bool _musicIsActive;

    /// <summary>Whether music is currently playing.</summary>
    public bool IsMusicPlaying => _musicIsActive;

    /// <summary>The key of the currently playing music track.</summary>
    public string? CurrentMusicTrack { get; private set; }

    public AudioManager()
    {
        EnsureBusLayout();
    }

    public override void _Ready()
    {
        LoadBuiltInSfx();
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

        LastPlayedSfxKey = key;
        TotalSfxPlayed++;

        if (stream is null)
            return;

        var player = GetOrCreatePoolPlayer(_nextPoolIndex);
        _nextPoolIndex = (_nextPoolIndex + 1) % PoolSize;

        player.Stream = stream;
        player.Bus = "SFX";
        player.Play();
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

    /// <summary>
    /// Starts looping ambient audio on the Ambient bus.
    /// If already playing, does nothing.
    /// </summary>
    public void StartAmbient()
    {
        if (_ambientPlayer is not null && _ambientPlayer.Playing)
            return;

        _ambientPlayer ??= new AudioStreamPlayer();
        if (_ambientPlayer.GetParent() is null)
            AddChild(_ambientPlayer);

        _ambientPlayer.Bus = "Ambient";

        var stream = TryLoadAmbientStream();
        if (stream is not null)
        {
            _ambientPlayer.Stream = stream;
            _ambientPlayer.Play();
        }
        else
        {
            // No audio file — mark as "playing" via a silent placeholder for testability.
            _ambientPlayer.Stream = null;
        }

        _ambientIsActive = true;
    }

    /// <summary>
    /// Plays a music track by key on the Music bus. Stops any current track first.
    /// </summary>
    public void PlayMusic(string trackKey)
    {
        if (_musicPlayer is not null && _musicPlayer.Playing)
            _musicPlayer.Stop();

        _musicPlayer ??= new AudioStreamPlayer();
        if (_musicPlayer.GetParent() is null)
            AddChild(_musicPlayer);

        _musicPlayer.Bus = "Music";
        CurrentMusicTrack = trackKey;

        var stream = TryLoadMusicStream(trackKey);
        if (stream is not null)
        {
            _musicPlayer.Stream = stream;
            _musicPlayer.Play();
        }

        _musicIsActive = true;
    }

    /// <summary>
    /// Stops music playback.
    /// </summary>
    public void StopMusic()
    {
        _musicIsActive = false;
        CurrentMusicTrack = null;
        if (_musicPlayer is not null && _musicPlayer.Playing)
            _musicPlayer.Stop();
    }

    /// <summary>
    /// Stops ambient audio playback.
    /// </summary>
    public void StopAmbient()
    {
        _ambientIsActive = false;
        if (_ambientPlayer is not null && _ambientPlayer.Playing)
            _ambientPlayer.Stop();
    }

    /// <summary>
    /// Pauses all audio streams when the application loses focus.
    /// </summary>
    public void HandleFocusLoss()
    {
        _ambientWasActive = _ambientIsActive;
        _musicWasActive = _musicIsActive;
        _pausedMusicTrack = CurrentMusicTrack;

        if (_ambientIsActive)
            StopAmbient();
        if (_musicIsActive)
            StopMusic();

        IsPaused = true;
    }

    /// <summary>
    /// Resumes audio streams when the application regains focus.
    /// </summary>
    public void HandleFocusGain()
    {
        if (!IsPaused)
            return;

        IsPaused = false;

        if (_ambientWasActive)
            StartAmbient();
        if (_musicWasActive && _pausedMusicTrack is not null)
            PlayMusic(_pausedMusicTrack);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusOut)
            HandleFocusLoss();
        else if (what == NotificationApplicationFocusIn)
            HandleFocusGain();
        else if (what == NotificationWMCloseRequest || what == NotificationPredelete)
            Cleanup();
    }

    private void Cleanup()
    {
        StopAmbient();
        StopMusic();

        if (_ambientPlayer is not null)
        {
            _ambientPlayer.Stream = null;
            _ambientPlayer = null;
        }

        if (_musicPlayer is not null)
        {
            _musicPlayer.Stream = null;
            _musicPlayer = null;
        }

        for (int i = 0; i < PoolSize; i++)
        {
            if (_sfxPool[i] is { } player)
            {
                player.Stop();
                player.Stream = null;
                _sfxPool[i] = null;
            }
        }

        _sfxRegistry.Clear();
    }

    private AudioStreamPlayer GetOrCreatePoolPlayer(int index)
    {
        if (_sfxPool[index] is { } existing)
            return existing;

        var player = new AudioStreamPlayer();
        AddChild(player);
        _sfxPool[index] = player;
        return player;
    }

    private static AudioStream? TryLoadMusicStream(string trackKey)
    {
        string path = $"res://assets/audio/music/{trackKey}.ogg";
        return ResourceLoader.Exists(path) ? GD.Load<AudioStream>(path) : null;
    }

    private static AudioStream? TryLoadAmbientStream()
    {
        const string path = "res://assets/audio/ambient/road_noise.ogg";
        return ResourceLoader.Exists(path) ? GD.Load<AudioStream>(path) : null;
    }

    private void LoadBuiltInSfx()
    {
        TryRegisterSfx("tap", "res://assets/audio/sfx/tap.ogg");
        TryRegisterSfx("cow_moo", "res://assets/audio/sfx/cow_moo.ogg");
        TryRegisterSfx("gravestone_thud", "res://assets/audio/sfx/gravestone_thud.ogg");
        TryRegisterSfx("graveyard_penalty", "res://assets/audio/sfx/graveyard_penalty.ogg");
    }

    private void TryRegisterSfx(string key, string path)
    {
        if (ResourceLoader.Exists(path))
        {
            RegisterSfx(key, GD.Load<AudioStream>(path));
        }
        else
        {
            RegisterSfx(key, null);
        }
    }

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
