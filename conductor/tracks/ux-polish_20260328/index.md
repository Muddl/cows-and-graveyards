# Track: UX Polish

**ID:** ux-polish_20260328
**Priority:** HIGH
**Status:** pending
**Branch:** `track/ux-polish_20260328`
**Created:** 2026-03-28

---

## Overview

Address the highest-friction UX issues surfaced during player testing. UX/Usability scored 2.3/5 — the lowest category. Four pain points affect parents, young children, and repeat users. This track fixes them.

---

## Artifacts

| File | Description |
|---|---|
| [spec.md](./spec.md) | Full specification — motivation, goals, success criteria, out of scope |
| [plan.md](./plan.md) | Phased implementation plan with TDD tasks and verification steps |
| [metadata.json](./metadata.json) | Machine-readable track state (status, phase/task tracking) |

---

## Phases

| # | Title | Tasks | Status |
|---|---|---|---|
| 1 | Graveyard Confirmation | 2 | pending |
| 2 | Quick Resume | 3 | pending |
| 3 | Transition & Loading Indicators | 2 | pending |
| 4 | Pause Button Polish | 2 | pending |

**Total tasks:** 10

---

## Key Problems Addressed

1. **Graveyard button silently wipes scores** — no confirmation, causes "backseat meltdowns" (Morgan, Jordan + 2 others)
2. **No quick resume** — 3+ taps to start every session, slot selection on every launch (Reese + 2 others)
3. **Slot screen blocks non-readers** — young children cannot navigate save slot selection (Milo, Morgan)
4. **No loading indicators** — scene delays read as crashes on mobile (Reese)
5. **Pause button too small and unlabeled** — easy to mis-tap, no affordance (Avery, Milo)

---

## Success Criteria (summary)

- Graveyard button requires confirmation before any score wipe
- "Continue Trip" appears on main menu for returning users; loads in one tap
- New users (no saves) skip slot selection entirely
- All scene transitions show a visible loading indicator
- Pause button meets 48x48dp minimum touch target with text label

---

## Starting This Track

```bash
git checkout master
git pull origin master
git checkout -b track/ux-polish_20260328
```

Then begin with Phase 1, Task 1.1 — write tests first (TDD is strict).

---

## Completion

When all 4 phases are done and `dotnet test` is fully green:

```bash
gh pr create --base master --head track/ux-polish_20260328 \
  --title "feat: UX polish — graveyard confirmation, quick resume, loading indicators, pause button"
```
