# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Cows & Graveyards — a mobile game (Android + iOS) built with Godot 4.6, C# (.NET 9), Mobile renderer, and Jolt Physics. It's a visual aid for the classic cow-counting road trip car game.

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
- **Line endings:** LF everywhere (enforced via .gitattributes)
- **Charset:** UTF-8 (enforced via .editorconfig)

## Project Management

This project uses **Conductor** for task tracking. Use `/conductor:status` to see current state and `/conductor:new-track` to create feature tracks. Conductor artifacts live in `conductor/`.