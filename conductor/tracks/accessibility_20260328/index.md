# Track: Accessibility Foundations

**Track ID:** accessibility_20260328
**Type:** Feature
**Status:** Pending
**Created:** 2026-03-28

## Summary
Add foundational accessibility support to the game — contrast audit and fixes for key UI and entity visuals, accessible names on all interactive Control nodes, and colorblind-safe color coding for left/right score differentiation. Driven by Avery's 2/5 player testing score: the game currently has zero accessibility accommodations beyond naturally large tap targets.

## Artifacts
- [Specification](./spec.md) — requirements, success criteria, dependencies, out of scope
- [Plan](./plan.md) — 3 phases, 10 tasks, TDD workflow

## Phases
| # | Phase | Status |
|---|-------|--------|
| 1 | Contrast Audit | Pending |
| 2 | Accessible Labels | Pending |
| 3 | Score Color Differentiation | Pending |

## Branch
`track/accessibility_20260328` (from `master`)

## Notes
- All phases follow strict TDD: tests written and failing before implementation
- Coordinate with **ux-polish_20260328** on the pause button node — ux-polish owns sizing, this track adds the accessible label; avoid conflicting edits
- Color constants for score differentiation live in `AccessibilityColors.cs` so they can be referenced from tests and UI code consistently
- Wong colorblind-safe palette is used for score colors: blue `#0072B2` (left) and orange `#E69F00` (right)
