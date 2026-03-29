# Track Spec: Onboarding & Tutorial

**Track ID:** onboarding_20260328
**Created:** 2026-03-28
**Priority:** CRITICAL (#2 finding from player testing)
**Status:** pending

---

## Summary

Add a first-play tutorial overlay, improve score label clarity, and explain the graveyard mechanic so that new players understand the game without prior knowledge of the road trip cow-counting tradition.

---

## Motivation

Player testing with 7 agents revealed that 6 of 7 flagged zero onboarding as a critical problem. The tester persona "Jordan" (first-time player, no road trip tradition background) could not understand the game at all. Specific failure points observed:

- No rules explanation: the cow-counting road trip tradition is not universal — players unfamiliar with it have no frame of reference.
- "L: X" / "R: X" score labels are completely unexplained — it is unclear whether the game is solo or multiplayer, and what L/R refers to.
- The graveyard button silently zeros the opponent's score with no explanation — multiple testers reported it "felt like a bug" rather than an intentional mechanic.
- No indication of what tapping the left or right side of the screen does on first launch.

This is the second-highest priority finding from the player testing track (onboarding_20260328) and must be addressed before wider distribution.

---

## Success Criteria

1. **First-play tutorial overlay** — on first launch after a new save slot is selected, a coach-mark / overlay sequence explains:
   - Tap the left side to count cows on your side of the road.
   - Tap the right side to count cows on your side of the road.
   - Score display: what each side's score represents.
   - Graveyard button: what it does and when to use it.
2. **Clearer score labels** — "L: X" / "R: X" replaced with labels that communicate side identity (e.g., "Left: X" / "Right: X", or player-icon + color differentiation). Color is used to distinguish the two sides.
3. **Graveyard first-use explanation** — the first time the graveyard button is pressed in a session/lifetime, a brief tooltip, animation, or confirmation pop-up explains the effect ("This zeros your opponent's cow count!").
4. **Tutorial can be replayed** — players can re-trigger the tutorial from the main menu or settings screen.
5. **Tutorial does not re-show on subsequent launches** — the "tutorial seen" flag is persisted to the save slot and respected on subsequent runs.
6. **All new UI and logic covered by gdUnit4 tests written before implementation** (strict TDD).

---

## Out of Scope

- Full in-game help system or help screen (future track).
- Video tutorials or animated explainer sequences beyond simple coach marks.
- Localization / i18n of tutorial text (future track).
- Onboarding for multiplayer modes not yet implemented.
- Accessibility audit of tutorial UI (future track).

---

## Reference: Current Game State

- Tap left half of screen → spawns cow on left side, increments left score.
- Tap right half of screen → spawns cow on right side, increments right score.
- Graveyard button → zeros the opponent's current cow score (no animation, no explanation).
- Save slot selection screen appears before gameplay.
- Score display: "L: {n}" and "R: {n}" with no further context.

---

## Linked Findings

- Player Testing Track: `player-testing_20260328` — finding #2 (zero onboarding).
