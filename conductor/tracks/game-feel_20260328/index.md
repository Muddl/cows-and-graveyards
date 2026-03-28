# Track: Game Feel Pass

**ID:** game-feel_20260328
**Branch:** `track/game-feel_20260328`
**Priority:** HIGH
**Status:** pending
**Created:** 2026-03-28

---

## Purpose

Add a juice and feedback layer to all player-facing interactions: haptics, tap ripple, score animation, eased entity movement, impact effects, and button pressed states.

Driven by player testing findings — Game Feel scored **2.0/5** (lowest category), with feedback describing the game as "dead" and "like a spreadsheet."

---

## Artifacts

| File | Description |
|------|-------------|
| [spec.md](./spec.md) | Requirements, motivation, success criteria, dependencies, out of scope |
| [plan.md](./plan.md) | Phased TDD implementation plan (4 phases, 10 tasks) |
| [metadata.json](./metadata.json) | Machine-readable status and task tracking |

---

## Phases at a Glance

| # | Phase | Tasks | Status |
|---|-------|-------|--------|
| 1 | Animation Easing | 2 | pending |
| 2 | Tap Feedback | 3 | pending |
| 3 | Score Animation | 2 | pending |
| 4 | Impact Effects | 3 | pending |

---

## Key Decisions

- Entity movement uses `Tween` with `EaseType.InOut` / `TransitionType.Cubic` rather than a `Curve` resource, to keep configuration in code and testable via timing assertions
- Haptic duration is a named constant (default 50 ms) to allow easy tuning
- Tap ripple is a lightweight `Control` node spawned at touch position and auto-freed via `Tween` completion signal — no pooling required at this scale
- Screen flash lives on a dedicated `CanvasLayer` above all game content to guarantee draw order
- No generalised shake/camera system — effects are single-purpose tweens scoped to this track

---

## Dependencies

- **audio-sfx_20260328** — coordinate scheduling; audio + feel together = stronger combined impact

## To Start

```bash
git checkout master
git pull
git checkout -b track/game-feel_20260328
```

Then follow phases in [plan.md](./plan.md), committing after each phase.
