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

## Key Dependencies

| Package | Version | Purpose |
| --- | --- | --- |
| Godot.NET.Sdk | 4.6.1 | Godot C# bindings |
| Microsoft.NET.Test.Sdk | 18.0.0 | Test infrastructure |
| gdUnit4.api | 5.1.0-rc3 | Test framework |
| gdUnit4.test.adapter | 3.0.0 | Test runner adapter |
| gdUnit4.analyzers | 1.0.0 | Code analyzers |
