# Plan: Audio & Sound Effects

**Track ID:** audio-sfx_20260328
**Status:** In Progress

## Overview
Four phases moving from infrastructure through core SFX, ambient/music, and final polish. Each phase follows strict TDD: tests are written and failing before any implementation code is added.

---

## Phase 1: Audio Foundation

**Goal:** Establish AudioManager autoload, configure audio bus layout (Master → SFX, Music, Ambient), and verify the structure is testable.

### Task 1.1 — Write AudioManager tests
- [x] Write gdUnit4 tests in `tests/audio/AudioManagerTest.cs`
- Test: AudioManager autoload exists in scene tree
- Test: SFX bus exists with correct index
- Test: Music bus exists with correct index
- Test: Ambient bus exists with correct index
- Test: `PlaySfx(string key)` method exists and does not throw on valid key
- Test: `SetBusVolume(string bus, float db)` clamps to valid range
- Commit: `test: AudioManager bus structure and API contracts`

### Task 1.2 — Implement AudioManager autoload
- [x] Create `scripts/audio/AudioManager.cs` (autoload singleton)
- Configure `default_bus_layout.tres` with Master, SFX, Music, Ambient buses
- Register AudioManager in `project.godot` autoloads
- Wire `PlaySfx`, `SetBusVolume`, `SetMusicVolume`, `SetAmbientVolume` methods
- All Task 1.1 tests must pass
- Commit: `feat: AudioManager autoload with bus layout`

### Verification checkpoint
Run `dotnet test --filter "FullyQualifiedName~CowsGraveyards.AudioManagerTest"` — all green before proceeding.

---

## Phase 2: Core SFX

**Goal:** Add tap confirmation, cow moo, gravestone thud, and graveyard penalty sounds. Each sound is driven by a game event; tests verify the event-to-sound wiring.

### Task 2.1 — Write tap and entity spawn SFX tests
- [x] Write tests in `tests/audio/CoreSfxTest.cs`
- Test: tapping cow (left or right) emits `CowTapped` signal
- Test: `CowTapped` signal causes AudioManager to play "tap" SFX
- Test: cow entity `Spawned` signal causes AudioManager to play "cow_moo" SFX
- Test: gravestone entity `Spawned` signal causes AudioManager to play "gravestone_thud" SFX
- Test: `PlaySfx` with unknown key logs warning, does not throw
- Commit: `test: core SFX event wiring for tap, moo, thud`

### Task 2.2 — Implement tap and entity spawn SFX
- [x] Add placeholder CC0 audio files: `assets/audio/sfx/tap.ogg`, `cow_moo.ogg`, `gravestone_thud.ogg`
- Register sounds in AudioManager SFX dictionary
- Connect `CowTapped` and entity `Spawned` signals to AudioManager in GameScene
- All Task 2.1 tests must pass
- Commit: `feat: tap, cow moo, and gravestone thud SFX`

### Task 2.3 — Write graveyard penalty SFX tests
- [x] Write tests in `tests/audio/GraveyardPenaltySfxTest.cs`
- Test: graveyard score-wipe event causes AudioManager to play "graveyard_penalty" SFX
- Test: penalty sound is distinct from entity spawn sounds (different key)
- Commit: `test: graveyard penalty SFX wiring`

### Task 2.4 — Implement graveyard penalty SFX
- [x] Add placeholder CC0 audio file: `assets/audio/sfx/graveyard_penalty.ogg`
- Register in AudioManager; connect to graveyard wipe event in GameScene
- All Task 2.3 tests must pass
- Commit: `feat: graveyard penalty SFX`

### Verification checkpoint
Run `dotnet test --filter "FullyQualifiedName~CowsGraveyards.CoreSfxTest|CowsGraveyards.GraveyardPenaltySfxTest"` — all green before proceeding.

---

## Phase 3: Ambient & Music

**Goal:** Add looping road-noise ambient track and background music with independent volume controls.

### Task 3.1 — Write ambient loop tests
- [x] Write tests in `tests/audio/AmbientAudioTest.cs`
- Test: `StartAmbient()` begins playback on the Ambient bus
- Test: ambient audio loops (stream is set to loop)
- Test: `StopAmbient()` stops playback without error
- Test: ambient volume is independently controllable via `SetAmbientVolume`
- Commit: `test: ambient road noise loop API`

### Task 3.2 — Implement ambient road noise loop
- [x] Add placeholder CC0 audio file: `assets/audio/ambient/road_noise.ogg` (looping)
- Implement `StartAmbient()` / `StopAmbient()` in AudioManager
- Call `StartAmbient()` on GameScene ready; `StopAmbient()` on exit
- All Task 3.1 tests must pass
- Commit: `feat: ambient road noise loop`

### Task 3.3 — Write background music tests
- [x] Write tests in `tests/audio/MusicAudioTest.cs`
- Test: `PlayMusic(string trackKey)` begins playback on the Music bus
- Test: music loops by default
- Test: `StopMusic()` stops without error
- Test: `SetMusicVolume` affects Music bus only (not SFX or Ambient)
- Test: transitioning to a different music track crossfades without pop
- Commit: `test: background music API`

### Task 3.4 — Implement background music
- [x] Add placeholder CC0 audio files: `assets/audio/music/menu_theme.ogg`, `gameplay_theme.ogg`
- Implement `PlayMusic` / `StopMusic` / crossfade logic in AudioManager
- Play `menu_theme` on MainMenuScene ready; `gameplay_theme` on GameScene ready
- All Task 3.3 tests must pass
- Commit: `feat: background music with scene-based track switching`

### Verification checkpoint
Run `dotnet test --filter "FullyQualifiedName~CowsGraveyards.AmbientAudioTest|CowsGraveyards.MusicAudioTest"` — all green before proceeding.

---

## Phase 4: Polish & Integration

**Goal:** Audio pooling for rapid taps, device mute respect, pop/click prevention, and full end-to-end verification.

### Task 4.1 — Write audio pool and mute tests
- [x] Write tests in `tests/audio/AudioPolishTest.cs`
- Test: rapid calls to `PlaySfx("tap")` in quick succession do not cut off each other (pool size >= 4)
- Test: when device audio focus is lost, `AudioManager.HandleFocusLoss()` pauses all streams
- Test: when audio focus is regained, streams resume
- Test: all buses default to valid dB values (not -inf on init)
- Commit: `test: audio pooling, focus handling, and init sanity`

### Task 4.2 — Implement audio pooling for SFX
- [x] Replace single AudioStreamPlayer per SFX with a pool of N players (N=4 minimum)
- [x] `PlaySfx` picks the next available player from the pool (round-robin)
- [x] All Task 4.1 pool tests must pass
- Commit: `feat: AudioStreamPlayer pool for rapid-fire SFX`

### Task 4.3 — Implement device mute / audio focus handling
- [x] Connect to Godot's `ApplicationFocusOut` / `ApplicationFocusIn` notifications
- [x] Implement `HandleFocusLoss()` and `HandleFocusGain()` in AudioManager
- [x] All Task 4.1 focus tests must pass
- Commit: `feat: pause/resume audio on application focus change`

### Task 4.4 — Final integration and lint
- [x] Run full test suite: `dotnet test "Cows & Graveyards.csproj" --settings .runsettings`
- [x] Run shader lint: `pwsh tools/godot.ps1 --headless -s tools/lint_project.gd`
- [x] Run test convention lint: `pwsh tools/lint_tests.ps1`
- [x] Confirm all audio success criteria from spec.md are met
- Commit: `chore: audio track final verification`

### Verification checkpoint
All tests green. No lint errors. Manual smoke test: tap cow, hear sound; see graveyard wipe, hear penalty; ambient plays; music plays.

---

## Summary

| Phase | Tasks | Focus |
|-------|-------|-------|
| 1 — Foundation | 2 | AudioManager autoload, bus layout |
| 2 — Core SFX | 4 | Tap, moo, thud, penalty sounds |
| 3 — Ambient & Music | 4 | Road noise loop, background music |
| 4 — Polish | 4 | Pooling, mute handling, final QA |
| **Total** | **14** | |
