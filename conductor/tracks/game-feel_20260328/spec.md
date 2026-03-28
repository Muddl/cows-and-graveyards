# Track Spec: Game Feel Pass

**Track ID:** game-feel_20260328
**Priority:** HIGH
**Status:** pending
**Created:** 2026-03-28

---

## Summary

Add a juice and feedback layer to all player-facing interactions. This covers haptic vibration on tap, visual ripple at touch point, animated score changes, eased entity movement curves, impact effects on graveyard wipe, and pressed states on interactive buttons.

---

## Motivation

Game Feel scored the lowest category in player testing: **average 2.0/5**, with one agent giving **1/5**. Verbatim feedback described interactions as "dead" and "like a spreadsheet." Specific gaps identified:

- No haptic feedback on any tap — no vibration, no visual ripple, no flash
- Score display is a plain text update; numbers change with no bounce, pulse, or transition
- CowEntity and GraveyardEntity use linear interpolation over 3 seconds — motion feels mechanical and breaks the spatial illusion of objects approaching from a distance
- No screen shake or impact effect on the graveyard score wipe (the highest-stakes event in a round)
- No tap ripple or visual feedback at the touch point
- No button press animations on graveyard/pause buttons

The game mechanic itself tested well; the problem is entirely presentation. High-effort polish in this area is expected to meaningfully raise the Game Feel score.

---

## Success Criteria

All of the following must be met before this track closes:

1. **Haptic vibration** fires on every scored tap (cow tap, graveyard tap) via `Input.VibrateHandheld`
2. **Tap ripple** — a brief visual ring or flash appears at the exact screen position of each tap
3. **Score animation** — score label animates on every change: scale-bounce on increment, color flash on increment, shake or flash on zero (graveyard wipe)
4. **Eased entity movement** — CowEntity and GraveyardEntity replace linear lerp with ease-in-out (or a custom `Curve`) so the 3-second approach feels natural and spatial
5. **Graveyard wipe impact** — screen flash or brief camera nudge fires when an opponent's graveyard score resets your cows to zero
6. **Button pressed states** — graveyard and pause buttons have a visually distinct pressed/depressed state on touch-down
7. All new behaviors are covered by gdUnit4 tests written before implementation
8. No regression in existing tests

---

## Dependencies

- **audio-sfx_20260328** — ideally completed before or alongside this track. Audio cues and tactile feedback together produce a much stronger combined impact than either alone. This track is independently shippable, but schedule coordination is preferred.

---

## Out of Scope

- Particle effects overhaul (e.g., sparkle trails, persistent dust clouds)
- New animations for the cow 3D model (rig changes, blend trees)
- A generalised camera shake system (any shake in this track is a single-purpose effect)
- Accessibility settings for reduced-motion or vibration disable (tracked separately)
