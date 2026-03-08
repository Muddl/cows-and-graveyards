# C# Style Guide

## General

- **Charset:** UTF-8 (enforced via `.editorconfig`)
- **Line endings:** LF (enforced via `.gitattributes`)
- **Nullable:** Enabled — use nullable reference types throughout
- **Root namespace:** `CowsGraveyards`

## Naming Conventions

| Element | Style | Example |
| --- | --- | --- |
| Namespace | PascalCase | `CowsGraveyards.Models` |
| Class / Struct | PascalCase | `PlayerScore` |
| Interface | IPascalCase | `IScoreTracker` |
| Method | PascalCase | `AddCow()` |
| Property | PascalCase | `TotalScore` |
| Public field | PascalCase | `MaxPlayers` |
| Private field | _camelCase | `_currentScore` |
| Parameter | camelCase | `playerName` |
| Local variable | camelCase | `cowCount` |
| Constant | PascalCase | `DefaultLives` |
| Enum | PascalCase | `GameState.Playing` |
| Event | PascalCase | `ScoreChanged` |

## Code Organization

### File Structure

- One primary type per file
- File name matches the type name (`PlayerScore.cs`)
- Group by feature, not by type (e.g., `Scoring/` not `Models/`)

### Using Directives

- Place at top of file, outside namespace
- Sort alphabetically: `System` first, then third-party, then project

### Class Member Order

1. Constants and static fields
2. Private fields
3. Constructors
4. Public properties
5. Public methods
6. Private methods

## Godot-Specific

### Node References

- Use `[Export]` for inspector-configured references
- Use `GetNode<T>()` in `_Ready()` for scene-tree references
- Store node references in private fields

### Signals

- Define signals using the Godot `[Signal]` attribute
- Use `EmitSignal()` to emit
- Connect in `_Ready()` or via the editor

### Lifecycle Methods

- Override `_Ready()` for initialization
- Override `_Process(double delta)` for per-frame logic
- Override `_PhysicsProcess(double delta)` for physics logic

## Formatting

- Indentation: 4 spaces (no tabs)
- Braces: Allman style (opening brace on new line)
- Max line length: 120 characters (soft limit)
- Single blank line between methods
- No trailing whitespace

## Patterns

### Prefer

- Expression-bodied members for simple getters/methods
- Pattern matching over type checking + casting
- `var` for obvious types, explicit types for clarity
- String interpolation over concatenation
- Collection expressions where applicable

### Avoid

- `#region` blocks
- Nested ternary expressions
- Magic numbers (use named constants)
- Catching `Exception` base type (catch specific exceptions)
