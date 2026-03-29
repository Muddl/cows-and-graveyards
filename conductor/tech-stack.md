# Tech Stack

## Engine

- **Godot 4.6.1** (Godot.NET.Sdk)
- **Renderer:** Mobile
- **Physics:** Jolt Physics

## Language

- **C# 13** (.NET 9)
- Nullable reference types enabled
- Root namespace: `CowsGraveyards`

## Testing

- **gdUnit4 5.1.0** (gdUnit4.api + gdUnit4.test.adapter + gdUnit4.analyzers)
- Tests run inside Godot runtime via `dotnet test`

## Data Persistence

- Godot built-in save system (JSON/binary via `FileAccess`, `user://`)

## Deployment Targets

- **Android** — Google Play Store
- **iOS** — Apple App Store

## Quality Tooling

- **Tea-leaves** — AI-assisted quality harness ([cleak/tea-leaves](https://github.com/cleak/tea-leaves), MIT)
  - `DevTools.cs` — Autoload providing runtime inspection via TCP (scene tree, performance, screenshots)
  - `SceneValidator.cs` — Static scene validation (node types, required components)
  - `devtools.py` — Python CLI for DevTools interaction
  - `lint_project.gd` — UID/NodePath validation (headless Godot)
  - `lint_shaders.gd` — Shader compilation linting (headless Godot)
  - `lint_tests.ps1` — gdUnit4 test convention linting
  - `godot.ps1` — PowerShell Godot launcher wrapper

## Key Dependencies

| Package | Version | Purpose |
| --- | --- | --- |
| Godot.NET.Sdk | 4.6.1 | Godot C# bindings |
| Microsoft.NET.Test.Sdk | 18.0.0 | Test infrastructure |
| gdUnit4.api | 5.1.0-rc3 | Test framework |
| gdUnit4.test.adapter | 3.0.0 | Test runner adapter |
| gdUnit4.analyzers | 1.0.0 | Code analyzers |
