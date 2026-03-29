# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Cows & Graveyards — a mobile game (Android + iOS) built with Godot 4.6, C# (.NET 9), Mobile renderer, and Jolt Physics. It's a visual aid for the classic cow-counting road trip car game.

## Current Feature State

Shipped (as of 2026-03-28):

- **Game Scene** — Cow spawning, side detection, score tracking, graveyard penalty mechanic
- **Menus & Persistence** — Main menu, pause menu, trip save/load, trip history
- **Visual Upgrade** — Medium-fidelity art pass, grass/stone shaders, entity visuals, effects
- **Audio & SFX** — AudioManager, ambient audio, music, core SFX, graveyard penalty sounds
- **Quality Harness** — Tea-leaves DevTools autoload, SceneValidator, project/shader/test linting

## Project Structure

```
scripts/          # 20 C# classes
  audio/          #   AudioManager
  devtools/       #   DevTools, SceneValidator
  game/           #   CowEntity, CowSpawner, GameScene, GameState, GraveyardButton,
                  #   GraveyardEntity, ScoreHud, ScoreTracker, Side, SideDetector
  menus/          #   MainMenuScene, PauseMenu, SaveManager, TripHistoryManager,
                  #   TripSave, PendingTrip, CompletedTripRecord
tests/            # 30 gdUnit4 test classes (mirrors scripts/ layout)
scenes/           # 7 .tscn files (game/ and menus/)
shaders/          # 2 .gdshader files (grass, stone)
tools/            # Tea-leaves CLI and linting scripts
conductor/        # Conductor project management artifacts
```

## Build & Run

```bash
# Build the C# project
dotnet build "Cows & Graveyards.csproj"

# Run tests (gdUnit4 via dotnet test, requires Godot binary)
dotnet test "Cows & Graveyards.csproj" --settings .runsettings

# Run a single test by fully-qualified name
dotnet test "Cows & Graveyards.csproj" --settings .runsettings --filter "FullyQualifiedName~MyNamespace.MyClass.TestMethod"

# Open in Godot editor
# Godot binary location: j:\DevStuff\Godot\engines\4.6.1\godot.exe
```

The `.runsettings` file configures gdUnit4 test runner with `GODOT_BIN` pointing to the local Godot engine. Tests run inside the Godot runtime.

## Tech Stack

- **Engine:** Godot 4.6.1 (Godot.NET.Sdk)
- **Language:** C# 13, .NET 9, nullable enabled
- **Root namespace:** `CowsGraveyards`
- **Test framework:** gdUnit4 5.1.0 (gdUnit4.api + gdUnit4.test.adapter + gdUnit4.analyzers)
- **Physics:** Jolt Physics
- **Renderer:** Mobile (D3D12 on Windows)

## Conventions

- **TDD workflow:** Write tests before implementation (strict)
- **Commits:** Conventional Commits format (feat/fix/test/refactor/chore)
- **Branching:** Each Conductor track gets a `track/{trackId}` branch from `master`
- **Phase commits:** Commit after every completed phase; push to origin
- **PR on completion:** Use `gh pr create` when a track finishes all phases
- **Line endings:** LF everywhere (enforced via .gitattributes)
- **Charset:** UTF-8 (enforced via .editorconfig)

## Quality Tooling

DevTools autoload and linting tools from [tea-leaves](https://github.com/cleak/tea-leaves) (MIT, by cleak).

```bash
# DevTools CLI — interact with running game instance
python tools/devtools.py ping                # Check if game is running
python tools/devtools.py screenshot          # Capture screenshot
python tools/devtools.py scene-tree          # Get node hierarchy
python tools/devtools.py performance         # Get FPS, memory stats
python tools/devtools.py validate-all        # Validate all scenes
python tools/devtools.py get-state --node "/root/GameScene"
python tools/devtools.py input tap my_action # Simulate input

# Linting — run via Godot headless or PowerShell
pwsh tools/godot.ps1 --headless -s tools/lint_project.gd   # UID/NodePath lint
pwsh tools/godot.ps1 --headless -s tools/lint_shaders.gd   # Shader compilation lint
pwsh tools/lint_tests.ps1                                    # gdUnit4 test convention lint
```

## Project Management

This project uses **Conductor** for task tracking. Use `/conductor:status` to see current state and `/conductor:new-track` to create feature tracks. Conductor artifacts live in `conductor/`.

## Recommended Agents

These specialized agents are available and useful for this project:

- **godot-csharp-dev** — Godot 4.x C# development with TDD, gdUnit4 tests. Use for feature implementation and test writing.
- **tdd-orchestrator** — Enforces red-green-refactor discipline across tasks. Use when implementing feature tracks.
- **debugger** — Debugging specialist for errors and test failures. Use when tests fail or unexpected behavior occurs.
- **code-reviewer** — Code review with security, performance, and quality analysis. Use before PRs.
- **architect-review** — Architecture review for system design decisions. Use for structural changes.
- **csharp-pro** — Advanced C# patterns, .NET optimization. Use for complex C# implementation.

For exploration and research, use the `Explore` subagent type directly rather than a named agent.