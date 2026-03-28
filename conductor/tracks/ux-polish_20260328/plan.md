# Implementation Plan: UX Polish

**Track ID:** ux-polish_20260328
**Total Phases:** 4
**Total Tasks:** 10

---

## Phase 1: Graveyard Confirmation

**Goal:** Prevent accidental score wipe. The graveyard button must require explicit confirmation before resetting scores.

**Context:** Currently the graveyard button calls score reset directly with no confirmation or undo. Multiple testers reported this felt like a bug and caused real frustration ("backseat meltdowns"). This is the highest-priority fix.

### Task 1.1 — Write tests for graveyard confirmation behavior (TDD)

Write gdUnit4 tests that:
- Verify pressing the graveyard button does NOT immediately reset scores
- Verify a confirmation dialog (or undo timer node) becomes visible after the button press
- Verify confirming the dialog results in score reset
- Verify cancelling the dialog leaves scores unchanged
- Verify the undo window (if timer-based) resets scores after expiry with no confirm action
- Namespace: `CowsGraveyards`

Commit: `test: graveyard confirmation dialog behavior`

### Task 1.2 — Implement graveyard confirmation

Implement the confirmation flow to pass the tests:
- Add a `ConfirmationDialog` node (or equivalent undo-timer overlay) to the graveyard button's parent scene
- Wire the graveyard button to show confirmation rather than directly resetting
- Score reset only executes on explicit confirm (or undo timer expiry)
- Cancel dismisses with no data change

Commit: `feat: add graveyard confirmation before score wipe`

**Verification:** Run `dotnet test` — all Phase 1 tests green. Manually verify in-editor: graveyard button no longer silently wipes scores.

---

## Phase 2: Quick Resume

**Goal:** Returning users with an active trip can resume in one tap. First-time users skip slot selection entirely.

**Context:** Tester Reese called the current slot-selection-every-session flow "2005-era UX." Three testers flagged the multi-tap start flow. Milo and Morgan flagged that young children (non-readers) cannot navigate slot selection. This phase addresses both.

### Task 2.1 — Write tests for quick resume and simplified slot flow (TDD)

Write gdUnit4 tests that:
- Verify a "Continue Trip" button is visible on the main menu when at least one active save exists
- Verify "Continue Trip" is hidden (or replaced with "New Trip") when no saves exist
- Verify tapping "Continue Trip" loads the most recently active save slot directly, without showing the slot selection screen
- Verify a new user (no saves) tapping "Start Trip" does not navigate to slot selection — they are taken directly to a new game (or auto-assigned slot 1)
- Test save detection logic in isolation (pure unit tests where possible)
- Namespace: `CowsGraveyards`

Commit: `test: quick resume and simplified slot flow behavior`

### Task 2.2 — Implement quick resume and simplified slot flow

Implement the logic to pass the tests:
- Read existing saves on `MainMenuScene` ready
- Show "Continue Trip" button if an active save is found; bind it to load most-recent save directly into `GameScene`
- For new users (no saves): bypass slot selection, auto-create in first available slot
- Existing multi-slot selection remains accessible for users who want to switch trips (e.g. via a secondary "Switch Trip" or "All Trips" path)

Commit: `feat: add quick resume and simplified new-user slot flow`

### Task 2.3 — Verify slot flow edge cases

Manually verify:
- First launch (no saves): Start Trip → game directly, no slot screen
- One active save: Continue Trip button visible, loads immediately
- Multiple saves: most recent loads via Continue; slot selection still reachable
- All slots full: expected behavior (no crash, appropriate message or slot-select shown)

Commit: `test: slot flow edge case coverage` (if additional tests needed)

**Verification:** Run `dotnet test` — all Phase 2 tests green. Walk through all slot-flow scenarios in editor.

---

## Phase 3: Transition & Loading Indicators

**Goal:** Any scene transition with a perceptible delay shows a visible loading indicator. No blank-screen hangs.

**Context:** Tester Reese reported that any delay between scenes reads as a crash on mobile. Even short transitions need visual feedback so users know the app is working.

### Task 3.1 — Write tests for loading indicator presence (TDD)

Write gdUnit4 tests that:
- Verify a loading indicator node exists and is accessible in the scene tree during scene transitions
- Verify the loading indicator becomes visible when a scene load is initiated
- Verify the loading indicator is hidden (or removed) once the target scene is ready
- Test the loading manager / transition controller in isolation
- Namespace: `CowsGraveyards`

Commit: `test: loading indicator visibility during scene transitions`

### Task 3.2 — Implement scene transition loading indicator

Implement to pass the tests:
- Add a `LoadingOverlay` (or `CanvasLayer` + `AnimationPlayer`) autoload or singleton that can be shown/hidden during scene changes
- Hook into scene transition calls (wherever `GetTree().ChangeSceneToFile()` is called) to show overlay on start and hide on completion
- Indicator can be a simple spinner, progress bar, or fade — keep it minimal for Mobile renderer performance
- Ensure the overlay renders above all game content (high `layer` value on `CanvasLayer`)

Commit: `feat: add loading overlay for scene transitions`

**Verification:** Run `dotnet test` — all Phase 3 tests green. Test transitions between MainMenu → GameScene and GameScene → MainMenu in editor; loading indicator appears and disappears cleanly.

---

## Phase 4: Pause Button Polish

**Goal:** Pause button is large enough to hit intentionally, clearly labeled, and positioned to minimize accidental taps.

**Context:** Testers Avery and Milo both reported mis-tapping the pause button. It is currently small and unlabeled. On a mobile device with a moving vehicle context, touch targets need to be generous and clearly afforded.

### Task 4.1 — Write tests for pause button hit target and label (TDD)

Write gdUnit4 tests that:
- Verify the pause button's minimum size meets or exceeds 48x48 logical pixels (dp)
- Verify the pause button has a non-empty text label or a named/tagged icon that identifies it as a pause control
- Verify the pause button remains functional (emits signal / triggers pause) after resize
- Namespace: `CowsGraveyards`

Commit: `test: pause button size and label requirements`

### Task 4.2 — Implement pause button enlargement and label

Implement to pass the tests:
- Increase pause button `custom_minimum_size` to at least `Vector2(48, 48)` (or larger, matching HUD design)
- Add a visible text label ("Pause" or "II") or ensure icon is clearly legible at target size
- Review button placement — consider moving away from corners where accidental edge-swipe taps are common on mobile
- Ensure touch target area does not overlap other interactive HUD elements

Commit: `feat: enlarge pause button and add text label`

**Verification:** Run `dotnet test` — all Phase 4 tests green. Test on Mobile renderer preview at common phone resolutions (1080x1920, 390x844). Confirm button is easy to tap intentionally and difficult to hit by accident.

---

## Completion Checklist

- [ ] Phase 1 complete — graveyard confirmation implemented and tested
- [ ] Phase 2 complete — quick resume and simplified slot flow implemented and tested
- [ ] Phase 3 complete — loading indicators on all scene transitions
- [ ] Phase 4 complete — pause button enlarged and labeled
- [ ] Full test suite passes (`dotnet test`)
- [ ] No regressions in existing tests
- [ ] PR opened: `gh pr create` from `track/ux-polish_20260328` → `master`
