# Workflow

## TDD Policy

**Strict** — Tests are required before implementation.

1. Write a failing test that describes the expected behavior
2. Write the minimum implementation to make the test pass
3. Refactor while keeping tests green

No production code without a corresponding test.

## Branching Strategy

**Feature branches per track.** Each Conductor track gets its own branch:

1. **Branch creation** — When starting a track, create a branch from `master`:
   ```
   git checkout -b track/{trackId}
   ```
2. **Phase commits** — Commit at the end of every completed phase (after verification passes). Use the format:
   ```
   {prefix}: {phase summary} ({trackId})
   ```
   A phase commit bundles all task-level work (tests, implementation, refactors) for that phase.
3. **Push after each phase** — Push the branch to origin after each phase commit to preserve progress remotely:
   ```
   git push -u origin track/{trackId}
   ```
4. **PR on track completion** — After all phases pass final verification, create a pull request via `gh` CLI:
   ```
   gh pr create --base master --head track/{trackId} --title "{Track title}" --body "..."
   ```
   The PR body should include a summary of changes, acceptance criteria status, and test results.

## Commit Strategy

**Conventional Commits** format:

- `feat:` — new feature
- `fix:` — bug fix
- `test:` — adding or updating tests
- `refactor:` — code restructuring without behavior change
- `chore:` — build, config, or tooling changes
- `docs:` — documentation updates

Commit granularity:
- **Within a phase:** Commit per task (test file, implementation, refactor) for fine-grained history.
- **Phase boundary:** A phase-level commit is required after verification passes. If task-level commits were already made, the phase boundary is the last task commit of that phase.

## Code Review

**Required for non-trivial changes.** Trivial changes (typos, formatting, config tweaks) may be self-reviewed. Feature track PRs always require review.

## Verification Checkpoints

**After each phase completion.** Before moving to the next phase in a track, verify:

- All tests pass
- Code builds without warnings
- Implementation matches the spec

## Chore & Documentation Tracks

Chore tracks (documentation, config, tooling updates) follow the same Conductor workflow but **skip TDD** since there is no production code to test. Verification is manual: build succeeds, docs are consistent, no broken cross-references.

## Quality Gates

Run these before marking a phase or track complete:

```bash
# Build
dotnet build "Cows & Graveyards.csproj"

# Tests (when applicable)
dotnet test "Cows & Graveyards.csproj" --settings .runsettings

# Linting (when applicable)
pwsh tools/godot.ps1 --headless -s tools/lint_project.gd
pwsh tools/godot.ps1 --headless -s tools/lint_shaders.gd
pwsh tools/lint_tests.ps1
```

## Task Lifecycle

1. **Pending** — task created, not started
2. **In Progress** — actively being worked on
3. **Review** — implementation complete, awaiting review
4. **Done** — verified and merged
