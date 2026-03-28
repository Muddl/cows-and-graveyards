# Specification: Initial Player Testing & Feedback

**Track ID:** player-testing_20260328
**Type:** Chore
**Created:** 2026-03-28
**Status:** Draft

## Summary

Build a repeatable player testing harness and conduct the first round of testing using diverse player agents with varying goals and skill levels. Validate the core gameplay loop, visual quality, and overall game feel. Produce a summary report of findings for triage.

## Context

Cows & Graveyards has completed its initial game scene, graveyard mechanic, menus, visual upgrade, and quality harness tracks. The game needs baseline player feedback before further feature development — and a way to re-run that feedback process after future changes.

## Motivation

Validate the core gameplay loop and non-functional qualities (look, feel, visual correctness) before investing in additional features. Establish a clean baseline where all testers evaluate the same build. Make this process repeatable so future development can be compared against prior rounds.

## Success Criteria

- [ ] Repeatable testing harness created (agent profiles, evaluation rubric, report template, runner script or command)
- [ ] Fixed build identified and documented (commit hash / version)
- [ ] Diverse set of player agents with different profiles (goals, skills, gaming experience) defined and used
- [ ] Each agent plays and evaluates the same build
- [ ] Feedback collected covers: core gameplay loop, UX/usability, visual quality, game feel, visual correctness
- [ ] Summary report produced with categorized findings
- [ ] Critical/high-priority issues identified for follow-up tracks
- [ ] Harness is documented so future rounds can be kicked off with a single command or slash command

## Dependencies

- DevTools quality harness (complete) — used for screenshots, scene validation, performance data
- Current `master` branch as the fixed test build

## Out of Scope

- Code fixes or changes during this track
- Store listing preparation
- External (human) playtesting logistics

## Technical Notes

- Player agents should represent a variety of personas (e.g., casual mobile gamer, hardcore gamer, young child, accessibility-focused user, first-time player). Each agent evaluates using a consistent rubric to enable comparison.
- The harness should live in `conductor/` or `tools/` and include: agent profile definitions, evaluation rubric, report template, and a runner that orchestrates a full round.
- Output from each round should be timestamped and stored for cross-round comparison.
