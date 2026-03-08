# Workflow

## TDD Policy

**Strict** — Tests are required before implementation.

1. Write a failing test that describes the expected behavior
2. Write the minimum implementation to make the test pass
3. Refactor while keeping tests green

No production code without a corresponding test.

## Commit Strategy

**Conventional Commits** format:

- `feat:` — new feature
- `fix:` — bug fix
- `test:` — adding or updating tests
- `refactor:` — code restructuring without behavior change
- `chore:` — build, config, or tooling changes
- `docs:` — documentation updates

## Code Review

**Required for non-trivial changes.** Trivial changes (typos, formatting, config tweaks) may be self-reviewed.

## Verification Checkpoints

**After each phase completion.** Before moving to the next phase in a track, verify:

- All tests pass
- Code builds without warnings
- Implementation matches the spec

## Task Lifecycle

1. **Pending** — task created, not started
2. **In Progress** — actively being worked on
3. **Review** — implementation complete, awaiting review
4. **Done** — verified and merged
