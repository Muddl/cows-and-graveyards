# Track Spec: UX Polish

**Track ID:** ux-polish_20260328
**Priority:** HIGH
**Status:** pending
**Created:** 2026-03-28
**Source:** Player testing findings (player-testing_20260328)

---

## Summary

Address the highest-friction UX issues surfaced during structured player testing. Covers four discrete problem areas: graveyard score-wipe confirmation, quick-resume for returning users, scene transition loading indicators, and pause button accessibility. These are targeted, self-contained improvements that can ship together without rearchitecting any core game systems.

---

## Motivation

Player testing returned a UX/Usability average of **2.3/5** — the lowest-scoring category across all evaluation dimensions. Four distinct friction points were flagged by multiple testers, affecting parents, young children, and repeat users alike.

### Findings

| Finding | Testers | Severity |
|---|---|---|
| Graveyard button silently zeroes score — feels like a bug | Morgan, Jordan + 2 others | Critical |
| No "continue last trip" — 3+ taps every session | Reese + 2 others | High |
| Save slot screen blocks non-readers (young children) | Milo, Morgan | High |
| No loading indicators — delays read as crashes | Reese | Medium |
| Pause button small, unlabeled, easy to mis-tap | Avery, Milo | Medium |

### Verbatim Feedback

- **Morgan:** "The graveyard button wiped everything. My kid had a meltdown in the backseat."
- **Jordan:** "I thought it was a bug. Nothing told me it was going to do that."
- **Reese:** "Every time I start a new trip I have to pick a save slot. It's like 2005-era UX."
- **Avery:** "I keep hitting the pause button by accident."
- **Milo:** "My daughter can't figure out which slot to pick — she can't read yet."

### Current UX Flow (for reference)

- **Start flow:** MainMenuScene → Start Trip → slot selection (3 slots) → GameScene
- **Pause menu:** Resume / Main Menu / Complete Trip / Quit
- **Graveyard button:** Directly calls score reset — no confirmation, no undo

---

## Goals

1. The graveyard button must require deliberate confirmation before wiping scores.
2. Returning users with an active trip can resume in one tap from the main menu.
3. First-time users (no existing saves) bypass slot selection entirely.
4. Any scene transition longer than an instant shows a visible loading indicator.
5. The pause button is large enough to hit intentionally and clearly labeled.

---

## Success Criteria

- [ ] **Graveyard confirmation:** Pressing the graveyard button opens a confirmation dialog or triggers a brief undo window before any score data is modified. Score wipe cannot happen with a single accidental tap.
- [ ] **Continue Trip button:** Main menu shows a "Continue Trip" button when an active save exists. Tapping it loads that trip directly, bypassing slot selection.
- [ ] **Simplified slot flow:** When no saves exist (first run), slot selection is skipped or replaced with a single "New Trip" action. Non-readers can start playing without navigating a slot menu.
- [ ] **Loading indicator:** Scene transitions show a loading spinner or animation. No silent blank-screen delay of any duration.
- [ ] **Pause button polish:** Pause button minimum touch target is 48x48dp, includes a visible text label or clear icon, and is positioned to minimize accidental taps.
- [ ] All new behavior is covered by gdUnit4 tests (TDD-first).
- [ ] No regressions in existing test suite.

---

## Out of Scope

- Full settings menu or options screen
- Account system or player profiles
- Cloud saves or cross-device sync
- Visual redesign of HUD or game scenes beyond the pause button
- Slot deletion or slot renaming UI
- Accessibility features beyond what is listed above (screen reader, font scaling, etc.)

---

## Dependencies

- Existing `GameScene`, `MainMenuScene`, `PauseMenu` scenes and their C# controllers
- Save slot system (FileAccess / `user://`)
- No new third-party dependencies required
