# Track Plan: Onboarding & Tutorial [x] Complete

**Track ID:** onboarding_20260328
**Total Phases:** 4
**Total Tasks:** 13
**Workflow:** Strict TDD — tests written before implementation in every phase.

---

## Phase 1: Score Label Improvement

**Goal:** Replace ambiguous "L: X" / "R: X" labels with clear, color-differentiated labels that communicate side identity at a glance.

### ~~Task 1.1 — Test: Score label content and color~~ [x]

Write gdUnit4 tests that assert:
- The left score label text contains a human-readable identifier (not just "L").
- The right score label text contains a human-readable identifier (not just "R").
- Each label node has a distinct color modulate (left ≠ right).
- Labels update correctly when score changes.

**TDD steps:**
1. Write `ScoreLabelTest.cs` under `tests/ui/`.
2. Run tests — confirm all fail (red).

### ~~Task 1.2 — Implement: Update score label nodes and binding~~ [x]

- Update the score label nodes in the game scene to use the new text format (e.g., "Left: {n}" / "Right: {n}").
- Assign distinct color modulates to left and right labels.
- Update the C# score-binding logic to write the new format.

**TDD steps:**
1. Implement label changes.
2. Run `ScoreLabelTest.cs` — confirm all pass (green).
3. Visually verify in editor / DevTools screenshot.

### ~~Task 1.3 — Commit~~ [x]

Commit with message: `feat: improve score label clarity and color differentiation`

**Verification:** `pwsh tools/lint_tests.ps1` passes. No regressions.

---

## Phase 2: First-Play Tutorial Overlay

**Goal:** Display a sequential coach-mark overlay on first launch (per save slot) that teaches the four core interactions.

### ~~Task 2.1 — Test: Tutorial overlay visibility and step progression~~ [x]

Write gdUnit4 tests that assert:
- `TutorialOverlay` node is visible when `TutorialSeen` is `false` in save data.
- `TutorialOverlay` is hidden when `TutorialSeen` is `true` in save data.
- Overlay exposes a method to advance to the next step.
- After all steps are completed, `TutorialSeen` is set to `true`.
- The overlay emits a signal (or calls a callback) when dismissed.

**TDD steps:**
1. Write `TutorialOverlayTest.cs` under `tests/ui/`.
2. Run tests — confirm all fail (red).

### ~~Task 2.2 — Implement: TutorialOverlay scene and controller~~ [x]

- Create `scenes/ui/TutorialOverlay.tscn` with coach-mark panels for each step:
  1. "Tap the left side to count cows on your side!" (highlight left tap zone)
  2. "Tap the right side to count cows on your side!" (highlight right tap zone)
  3. "These are your scores — left player and right player." (highlight score labels)
  4. "The graveyard button zeros your opponent's cows — use it wisely!" (highlight graveyard button)
- Implement `TutorialOverlay.cs` controller with step progression and dismissal logic.
- Wire `TutorialSeen` flag read/write to save data.

**TDD steps:**
1. Implement scene and controller.
2. Run `TutorialOverlayTest.cs` — confirm all pass (green).

### ~~Task 2.3 — Integrate: Add overlay to GameScene~~ [x]

- Instance `TutorialOverlay` in `GameScene`.
- On scene ready, check save data — show overlay if not seen.
- Block gameplay input while tutorial is active.

**TDD steps:**
1. Write integration test asserting gameplay tap is ignored while tutorial overlay is active.
2. Implement input block.
3. Run all tests — confirm green.

### ~~Task 2.4 — Commit~~ [x]

Commit with message: `feat: add first-play tutorial overlay with coach marks`

**Verification:** `python tools/devtools.py screenshot` — confirm overlay appears on fresh save slot.

---

## Phase 3: Graveyard First-Use Explanation

**Goal:** Explain the graveyard mechanic the first time it is triggered, so it feels intentional rather than a bug.

### ~~Task 3.1 — Test: Graveyard first-use tooltip display~~ [x]

Write gdUnit4 tests that assert:
- `GraveyardTooltip` node is shown the first time the graveyard button is pressed when `GraveyardExplained` is `false`.
- `GraveyardTooltip` is not shown on subsequent presses when `GraveyardExplained` is `true`.
- After the tooltip is dismissed, `GraveyardExplained` is set to `true` in save data.
- The graveyard action (zeroing opponent score) does not execute until the tooltip is dismissed on first use.

**TDD steps:**
1. Write `GraveyardTooltipTest.cs` under `tests/ui/`.
2. Run tests — confirm all fail (red).

### ~~Task 3.2 — Implement: GraveyardTooltip scene and controller~~ [x]

- Create `scenes/ui/GraveyardTooltip.tscn` — a brief pop-up/panel that reads:
  "Graveyard! Your opponent loses all their cows. Are you sure?" with a Confirm button.
- Implement `GraveyardTooltip.cs` with first-use gate logic.
- On first use: show tooltip, pause the graveyard action, execute on confirm.
- On subsequent uses: execute graveyard action immediately (no tooltip).
- Wire `GraveyardExplained` flag to save data.

**TDD steps:**
1. Implement scene and controller.
2. Run `GraveyardTooltipTest.cs` — confirm all pass (green).

### ~~Task 3.3 — Commit~~ [x]

Commit with message: `feat: add graveyard first-use explanation tooltip`

**Verification:** `python tools/devtools.py screenshot` — confirm tooltip appears on first graveyard press.

---

## Phase 4: Tutorial Persistence & Replay

**Goal:** Persist the tutorial-seen state across launches and allow the player to replay the tutorial from the menu.

### ~~Task 4.1 — Test: Tutorial state persistence in save data~~ [x]

Write gdUnit4 tests that assert:
- `TutorialSeen` flag is written to save data when tutorial is dismissed.
- `TutorialSeen` flag is read correctly on game load.
- `GraveyardExplained` flag is written and read correctly.
- Resetting tutorial state (for replay) clears both flags.

**TDD steps:**
1. Write `TutorialSaveStateTest.cs` under `tests/data/`.
2. Run tests — confirm all fail (red).

### ~~Task 4.2 — Implement: Tutorial save state in SaveData model~~ [x]

- Add `TutorialSeen` (bool) and `GraveyardExplained` (bool) fields to the save data model/schema.
- Implement read/write in the save system.
- Implement `ResetTutorial()` method that clears both flags.

**TDD steps:**
1. Implement save data changes.
2. Run `TutorialSaveStateTest.cs` — confirm all pass (green).

### ~~Task 4.3 — Implement: Replay tutorial entry point in menu/settings~~ [x]

- Add a "Replay Tutorial" button to the main menu or settings screen.
- On press: call `ResetTutorial()`, then navigate to (or reload) the game scene so the overlay fires.

**TDD steps:**
1. Write test asserting "Replay Tutorial" button calls `ResetTutorial()` and triggers navigation.
2. Implement button and handler.
3. Run all tests — confirm green.

### ~~Task 4.4 — Commit~~ [x]

Commit with message: `feat: persist tutorial state and add replay option`

**Verification:**
- Launch game twice — tutorial shows on first launch, not on second.
- Press "Replay Tutorial" — tutorial shows again.
- `python tools/devtools.py validate-all` passes.
- `pwsh tools/lint_tests.ps1` passes.

---

## Final Track Verification

Before creating the PR:

1. `dotnet build "Cows & Graveyards.csproj"` — clean build, no warnings.
2. `dotnet test "Cows & Graveyards.csproj" --settings .runsettings` — all tests pass.
3. `pwsh tools/lint_tests.ps1` — no lint violations.
4. `pwsh tools/godot.ps1 --headless -s tools/lint_project.gd` — no UID/NodePath issues.
5. `python tools/devtools.py validate-all` — all scenes valid.
6. Manual smoke test: fresh save slot shows tutorial; second launch does not; graveyard first press shows tooltip; "Replay Tutorial" resets flow.
