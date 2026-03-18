# Implementation Plan: Main Menu, Pause Menu & Trip Persistence

**Track ID:** menus_20260318
**Spec:** [spec.md](./spec.md)
**Created:** 2026-03-18
**Status:** [ ] Not Started

## Overview

Build bottom-up: data layer first (TripSave + SaveManager), then the two
menu scenes, then wire input isolation and trip state flow into GameScene,
and finish with a shared theme applied across all UI. Every logic class
gets tests before implementation; the input-isolation boundary and
save/load round-trip get explicit integration test coverage.

## Phase 1: Trip Persistence

Pure data layer — no Godot nodes, fully unit-testable. Must be solid
before any menu scene touches it.

### Tasks

- [x] Task 1.1: Write failing unit tests for `TripSave` — stores slot
      index (0–2), left score, right score; serialises to/from a JSON
      dictionary correctly
- [x] Task 1.2: Write failing unit tests for `SaveManager` — save a slot,
      load a slot, load all slots, graceful empty return on missing file,
      graceful empty return on corrupt/invalid JSON
- [x] Task 1.3: Implement `TripSave.cs` and `SaveManager.cs` to pass all
      tests; persist to `user://trips.json` via `FileAccess`

### Verification

- [x] All TripSave and SaveManager tests pass
- [x] `dotnet build` succeeds with no warnings

## Phase 2: Main Menu

The first screen the player sees. Wires directly to SaveManager for slot
display and sets the startup scene.

### Tasks

- [ ] Task 2.1: Write unit tests for `MainMenuScene` — three slot buttons
      show correct labels (empty vs saved scores), Quit button absent when
      `OS.GetName()` is "Android" or "iOS", `NewTripRequested` signal
      carries slot index, `LoadTripRequested` signal carries a `TripSave`
- [ ] Task 2.2: Implement `MainMenuScene.cs` + `MainMenuScene.tscn` with
      New Trip slots, Load Trip slots, and conditional Quit button
- [ ] Task 2.3: Set `MainMenuScene.tscn` as the project startup scene
      in `project.godot`

### Verification

- [ ] All MainMenuScene tests pass
- [ ] Launching the project opens the main menu, not the game directly
- [ ] `dotnet build` succeeds with no warnings

## Phase 3: Pause Menu & Input Isolation

The in-game overlay. Input isolation is the critical deliverable —
game taps must be provably blocked while any menu is open.

### Tasks

- [ ] Task 3.1: Write unit tests for `PauseMenu` — `ResumeRequested` and
      `MainMenuRequested` signals fire on correct button press, Quit button
      absent on mobile
- [ ] Task 3.2: Implement `PauseMenu.cs` + `PauseMenu.tscn`; add a
      persistent Pause button to `GameScene.tscn` CanvasLayer
      (MOUSE_FILTER_STOP, always on top)
- [ ] Task 3.3: Write integration tests for input isolation — after
      `GameScene.SetProcessInput(false)`, simulated cow taps and graveyard
      presses produce no score change; after `SetProcessInput(true)` they
      do
- [ ] Task 3.4: Implement pause/resume wiring in `GameScene.cs` — connect
      Pause button and `PauseMenu` signals, toggle `SetProcessInput` and
      PauseMenu visibility

### Verification

- [ ] All PauseMenu and input-isolation tests pass
- [ ] Pause button visible during play; tapping it shows PauseMenu
- [ ] Game inputs are blocked while PauseMenu is open
- [ ] `dotnet build` succeeds with no warnings

## Phase 4: Trip State Integration & Scene Flow

Wire trip slot data through the full new-game and return-to-menu flows.
Tests written before the wiring code to lock down the save/restore
contract.

### Tasks

- [ ] Task 4.1: Write integration tests — `GameScene` initialised with a
      `TripSave` slot correctly seeds `ScoreTracker` left/right scores and
      stores the active slot index
- [ ] Task 4.2: Write integration tests — calling `GameScene.SaveAndExit()`
      writes current scores back to the correct slot via `SaveManager`,
      then signals scene change
- [ ] Task 4.3: Implement `GameScene` trip slot initialisation — accept
      slot index + optional `TripSave` on entry, seed `ScoreTracker`
      accordingly
- [ ] Task 4.4: Implement `GameScene.SaveAndExit()` — saves current scores
      to active slot and changes scene to `MainMenuScene`; connect to
      `PauseMenu.MainMenuRequested`

### Verification

- [ ] All trip integration tests pass
- [ ] New trip starts with zero scores; loaded trip restores correct scores
- [ ] Returning to main menu saves current state; re-loading the slot
      shows the saved scores
- [ ] `dotnet build` succeeds with no warnings

## Phase 5: Shared Theme

Apply a consistent design language across all UI. Placeholder aesthetics
are acceptable — consistency is the goal.

### Tasks

- [ ] Task 5.1: Create `res://assets/ui/game_theme.tres` — define shared
      styles for Button (normal/hover/pressed), Label, and Panel nodes
- [ ] Task 5.2: Apply `game_theme.tres` to `MainMenuScene.tscn`,
      `PauseMenu.tscn`, and `ScoreHud.tscn`

### Verification

- [ ] All menus and the HUD share a visually consistent style
- [ ] `dotnet build` succeeds with no warnings

## Final Verification

- [ ] All acceptance criteria met
- [ ] All tests passing (`dotnet test`)
- [ ] `dotnet build` succeeds with no warnings
- [ ] Quit button absent on Android/iOS, present on desktop
- [ ] Project file structure follows Godot conventions
- [ ] Ready for review

---

_Generated by Conductor. Tasks will be marked [~] in progress and [x] complete._
