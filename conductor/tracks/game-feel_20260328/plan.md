# Track Plan: Game Feel Pass

**Track ID:** game-feel_20260328
**Total Phases:** 4
**Total Tasks:** 10

---

## Phase 1: Animation Easing

Replace the linear lerp on entity movement with ease-in-out curves so the 3-second cow/gravestone approach reads as natural spatial motion rather than mechanical translation.

### Task 1.1 — Write easing tests for CowEntity

**TDD step: RED**

Write gdUnit4 tests that assert:
- Given a `CowEntity` configured for its standard 3-second approach, the position at t=0.5s is closer to the origin than the midpoint of the linear path (ease-in characteristic)
- The position at t=2.5s is closer to the camera than the midpoint of the linear path (ease-out characteristic)
- The entity reaches the final position at or before the configured duration

Files: `tests/game/CowEntityEasingTest.cs`

**Verification:** Tests compile and fail (RED) before any implementation change.

### Task 1.2 — Implement ease-in-out curve on CowEntity and GraveyardEntity

**TDD step: GREEN + REFACTOR**

Replace the linear `Lerp` call with a `Tween` using `Tween.EaseType.InOut` / `Tween.TransitionType.Cubic`, or apply a custom `Curve` resource. Apply the same change to GraveyardEntity if it shares the same movement pattern.

Files: `scripts/game/CowEntity.cs`, `scripts/game/GraveyardEntity.cs` (if applicable)

**Verification:** Task 1.1 tests pass (GREEN). Run full test suite; no regressions.

**Commit:** `feat: replace linear lerp with ease-in-out curves on entity movement`

---

## Phase 2: Tap Feedback

Add haptic vibration and a visual ripple/flash at the touch point. Add pressed states to the graveyard and pause buttons.

### Task 2.1 — Write tap feedback tests

**TDD step: RED**

Write gdUnit4 tests that assert:
- A tap on the cow/graveyard area triggers a call to `Input.VibrateHandheld` (spy/mock via gdUnit4 signal or method tracking)
- A tap spawns a ripple node at the tap world/screen position and that node is visible for at least one frame
- The ripple node auto-frees (queue_free or visibility off) within a configured duration (e.g. 0.5s)
- The graveyard and pause buttons expose a `Pressed` visual state that differs from their default state

Files: `tests/game/TapFeedbackTest.cs`

**Verification:** Tests compile and fail (RED).

### Task 2.2 — Implement haptic vibration

**TDD step: GREEN**

Add `Input.VibrateHandheld(50)` (50 ms, tuneable constant) to the tap handler for cow and graveyard interactions.

Files: `scripts/game/GameScene.cs` (or whichever node owns tap input)

**Verification:** Haptic tests pass.

### Task 2.3 — Implement tap ripple and button pressed states

**TDD step: GREEN + REFACTOR**

- Spawn a `TapRipple` `Control` or `Node2D` at the touch position on each tap. Animate alpha and scale via `Tween`, then `QueueFree`.
- Add a `StyleBoxFlat` or `NinePatchRect` swap on graveyard and pause buttons for the `pressed` theme state.

Files: `scripts/game/TapRipple.cs`, `scenes/game/TapRipple.tscn`, `scripts/game/GameScene.cs`, relevant button scene files

**Verification:** All Task 2.1 tests pass. Manual smoke: ripple visible on-device or in editor Play mode.

**Commit:** `feat: add haptic vibration, tap ripple, and button pressed states`

---

## Phase 3: Score Animation

Animate the score label on every change: bounce on increment, flash on increment, shake/flash on zero.

### Task 3.1 — Write score animation tests

**TDD step: RED**

Write gdUnit4 tests that assert:
- When the score increments, the score label's `Scale` exceeds `Vector2.One` within the same frame or the next (bounce start)
- When the score increments, the score label's `Modulate` deviates from white within the same frame or the next (color flash start)
- When the score is set to zero (graveyard wipe), a distinct "shake" or "flash" signal/state is emitted by the score display node
- All animations self-resolve: scale returns to `Vector2.One` and modulate returns to white within a configured duration

Files: `tests/game/ScoreAnimationTest.cs`

**Verification:** Tests compile and fail (RED).

### Task 3.2 — Implement score animation on ScoreDisplay

**TDD step: GREEN + REFACTOR**

- On increment: run a `Tween` that scales the label to 1.3x then back to 1.0x (bounce) and flashes `Modulate` to a highlight color then back to white, over ~0.3s.
- On zero (wipe): run a brief horizontal position shake tween (±4px, 3 cycles, ~0.25s) and/or a red flash on `Modulate`.

Files: `scripts/game/ScoreDisplay.cs` (or equivalent), `scenes/game/ScoreDisplay.tscn`

**Verification:** All Task 3.1 tests pass. Full test suite green.

**Commit:** `feat: animate score label on increment and graveyard wipe`

---

## Phase 4: Impact Effects

Add a screen flash and/or camera nudge on the graveyard wipe event — the highest-stakes moment in a round.

### Task 4.1 — Write impact effect tests

**TDD step: RED**

Write gdUnit4 tests that assert:
- When a graveyard wipe event fires, a full-screen flash `ColorRect` (or equivalent) becomes visible for at least one frame
- The flash node's alpha reaches a peak value above 0 and returns to 0 within a configured duration (e.g. 0.4s)
- A spawn "pop" (brief overshoot scale then settle to 1.0) occurs on CowEntity and GraveyardEntity when they first appear at the horizon

Files: `tests/game/ImpactEffectsTest.cs`

**Verification:** Tests compile and fail (RED).

### Task 4.2 — Implement screen flash on graveyard wipe

**TDD step: GREEN**

- Add a `CanvasLayer`-rooted `ColorRect` (white or red, alpha starts at 0) to the game scene.
- On graveyard wipe event: tween alpha 0 → 0.6 → 0 over ~0.4s.

Files: `scripts/game/ImpactFlash.cs`, `scenes/game/GameScene.tscn`

### Task 4.3 — Implement spawn pop on entity arrival

**TDD step: GREEN + REFACTOR**

- When CowEntity/GraveyardEntity becomes visible (reaches the near position threshold), run a scale tween: 0 → 1.15 → 1.0 over ~0.2s.

Files: `scripts/game/CowEntity.cs`, `scripts/game/GraveyardEntity.cs`

**Verification:** All Task 4.1 tests pass. Full test suite green.

**Commit:** `feat: add screen flash on graveyard wipe and spawn pop on entity arrival`

---

## Track Completion

- All 4 phases committed and pushed
- Full test suite green (`dotnet test`)
- Shader and UID lint clean (`pwsh tools/godot.ps1 --headless -s tools/lint_project.gd`)
- Manual smoke on device or editor Play mode confirms all feel improvements
- PR opened: `gh pr create --base master --head track/game-feel_20260328`
