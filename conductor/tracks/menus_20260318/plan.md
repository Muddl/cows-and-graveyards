# Implementation Plan: Main Menu, Pause Menu & Trip Persistence

**Track ID:** menus_20260318
**Spec:** [spec.md](./spec.md)
**Created:** 2026-03-18
**Status:** [x] Complete

## Overview

Build bottom-up: data layer first (TripSave + SaveManager), then the two
menu scenes, then wire input isolation and trip state flow into GameScene,
and finish with a shared theme applied across all UI. Every logic class
gets tests before implementation; the input-isolation boundary and
save/load round-trip get explicit integration test coverage.

Later extended to add trip completion (Complete Trip flow, history
persistence), a redesigned main menu (Start/Load Trip pattern with slot
panel), scene navigation wiring (PendingTrip handoff), and GUI polish.

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

- [x] Task 2.1: Write unit tests for `MainMenuScene` — slot labels, Quit
      button visibility, `NewTripRequested` and `LoadTripRequested` signals
- [x] Task 2.2: Implement `MainMenuScene.cs` + `MainMenuScene.tscn`
- [x] Task 2.3: Set `MainMenuScene.tscn` as the project startup scene
      in `project.godot`

### Verification

- [x] All MainMenuScene tests pass
- [x] Launching the project opens the main menu, not the game directly
- [x] `dotnet build` succeeds with no warnings

## Phase 3: Pause Menu & Input Isolation

The in-game overlay. Input isolation is the critical deliverable —
game taps must be provably blocked while any menu is open.

### Tasks

- [x] Task 3.1: Write unit tests for `PauseMenu` — `ResumeRequested` and
      `MainMenuRequested` signals fire on correct button press, Quit button
      absent on mobile
- [x] Task 3.2: Implement `PauseMenu.cs` + `PauseMenu.tscn`; add a
      persistent Pause button to `GameScene.tscn` CanvasLayer
- [x] Task 3.3: Write integration tests for input isolation
- [x] Task 3.4: Implement pause/resume wiring in `GameScene.cs`

### Verification

- [x] All PauseMenu and input-isolation tests pass
- [x] Pause button visible during play; tapping it shows PauseMenu
- [x] Game inputs are blocked while PauseMenu is open
- [x] `dotnet build` succeeds with no warnings

## Phase 4: Trip State Integration & Scene Flow

Wire trip slot data through the full new-game and return-to-menu flows.

### Tasks

- [x] Task 4.1: Write integration tests — `GameScene` initialised with a
      `TripSave` slot correctly seeds `ScoreTracker` left/right scores
- [x] Task 4.2: Write integration tests — `GameScene.SaveAndExit()` writes
      scores back and changes scene
- [x] Task 4.3: Implement `GameScene` trip slot initialisation
- [x] Task 4.4: Implement `GameScene.SaveAndExit()` and scene wiring
- [x] Task 4.5: Add `PendingTrip` static handoff so `MainMenuScene` can
      pass slot data to `GameScene` across `ChangeSceneToFile`; wire
      `MainMenuScene` own signals to `NavigateToGame`

### Verification

- [x] All trip integration tests pass
- [x] New trip starts with zero scores; loaded trip restores correct scores
- [x] Returning to main menu saves current state
- [x] `dotnet build` succeeds with no warnings

## Phase 5: Shared Theme

Apply a consistent design language across all UI.

### Tasks

- [x] Task 5.1: Create `res://assets/ui/game_theme.tres`
- [x] Task 5.2: Apply `game_theme.tres` to all menu and HUD scenes

### Verification

- [x] All menus and the HUD share a visually consistent style
- [x] `dotnet build` succeeds with no warnings

## Phase 6: Menu Redesign & Trip Completion

Redesign main menu for a cleaner UX; add Complete Trip flow with
persistent history; GUI polish pass.

### Tasks

- [x] Task 6.1: Write tests for `HasAvailableSlot`, `HasAnySave`, and
      `OnStartTripPressed(slots)` overload on `MainMenuScene`
- [x] Task 6.2: Redesign `MainMenuScene` — replace per-slot button rows
      with Start Trip / Load Trip / Quit; Load Trip opens a slot select
      panel showing only populated slots; Start Trip shows a timed error
      when all slots are full and hides itself during the error
- [x] Task 6.3: Write tests for `SaveManager.DeleteSlot`
- [x] Task 6.4: Implement `SaveManager.DeleteSlot`
- [x] Task 6.5: Write tests for `CompletedTripRecord` and
      `TripHistoryManager` (append, load-all, corrupt-file resilience)
- [x] Task 6.6: Implement `CompletedTripRecord.cs` and
      `TripHistoryManager.cs` — persist completed trips to
      `user://trip_history.json`
- [x] Task 6.7: Write tests for `PauseMenu.CompleteTripRequested` signal
- [x] Task 6.8: Add Complete Trip button to `PauseMenu`; wire
      `GameScene.CompleteAndExit()` — appends history record, deletes
      active slot, returns to main menu
- [x] Task 6.9: Move pause button to top-center of screen
- [x] Task 6.10: Fix overlapping panels (hide MainPanel when SlotSelectPanel
      is open) and move ErrorLabel outside VBox to prevent layout shifts

### Verification

- [x] All 156 tests pass
- [x] Start Trip finds first free slot; shows timed error and hides button
      when all 3 slots are full
- [x] Load Trip panel shows only populated slots
- [x] Complete Trip archives the trip and removes it from active slots
- [x] No layout shifts when error label appears
- [x] No panel overlap between main buttons and slot select panel
- [x] `dotnet build` succeeds with no warnings

## Final Verification

- [x] All acceptance criteria met
- [x] All 156 tests passing (`dotnet test`)
- [x] `dotnet build` succeeds with no warnings
- [x] Quit button absent on Android/iOS, present on desktop
- [x] Project file structure follows Godot conventions
- [x] Ready for review

---

_Generated by Conductor. Tasks will be marked [~] in progress and [x] complete._
