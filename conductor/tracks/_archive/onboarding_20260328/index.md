# Track: Onboarding & Tutorial

**Track ID:** onboarding_20260328
**Branch:** `track/onboarding_20260328`
**Status:** pending
**Priority:** CRITICAL
**Created:** 2026-03-28

---

## Overview

Addresses the #2 critical finding from player testing: 6 of 7 test agents flagged zero onboarding. First-time players cannot understand the game without prior knowledge of the road trip cow-counting tradition.

See [`spec.md`](./spec.md) for full requirements and success criteria.
See [`plan.md`](./plan.md) for phased TDD implementation plan.
See [`metadata.json`](./metadata.json) for machine-readable status and task tracking.

---

## Phases

| # | Title | Status |
|---|-------|--------|
| 1 | Score Label Improvement | pending |
| 2 | First-Play Tutorial Overlay | pending |
| 3 | Graveyard First-Use Explanation | pending |
| 4 | Tutorial Persistence & Replay | pending |

**Total tasks:** 13

---

## Quick Start

```bash
# Create and switch to track branch
git checkout -b track/onboarding_20260328

# After all phases complete, open PR
gh pr create --title "feat: onboarding & tutorial" --body "Closes onboarding_20260328 track. Adds first-play tutorial, clearer score labels, graveyard explanation, and tutorial replay."
```

---

## Linked Artifacts

- Player testing findings: [`conductor/tracks/player-testing_20260328/`](../player-testing_20260328/)
- Spec: [`spec.md`](./spec.md)
- Plan: [`plan.md`](./plan.md)
- Metadata: [`metadata.json`](./metadata.json)
