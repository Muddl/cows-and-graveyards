<#
Forwards all args to the Godot 4.6.x Mono editor executable.

Resolution order (first found wins):
1) Environment variable GODOT4_MONO_EXE
2) Project-specific path: j:\DevStuff\Godot\engines\4.6.1\godot.exe

This script can be dot-sourced to use Resolve-GodotPath function:
  . ./tools/godot.ps1 -ResolveOnly
  $godot = Resolve-GodotPath

Adapted from tea-leaves (MIT, github.com/cleak/tea-leaves) for Cows & Graveyards.
#>

param(
    [switch]$ResolveOnly
)

$ErrorActionPreference = "Stop"

function Resolve-GodotPath {
    $envCandidate = $env:GODOT4_MONO_EXE
    $projectPath = "j:\DevStuff\Godot\engines\4.6.1\godot.exe"

    if ($envCandidate -and (Test-Path -LiteralPath $envCandidate)) { return $envCandidate }
    if (Test-Path -LiteralPath $projectPath) { return $projectPath }
    throw "Godot executable not found. Set GODOT4_MONO_EXE or install to '$projectPath'."
}

# If -ResolveOnly, just define the function and exit (for dot-sourcing)
if ($ResolveOnly) { return }

# Otherwise, run Godot with provided args
$godot = Resolve-GodotPath
& $godot @args
exit $LASTEXITCODE
