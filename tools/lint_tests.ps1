# Lint gdUnit4 C# test files for problematic patterns
# Usage: pwsh ./tools/lint_tests.ps1
#
# Exit codes:
#   0 = All checks passed
#   1 = Warnings found (review recommended)
#   2 = Errors found (must fix)
#
# This script detects:
#   - Missing [TestSuite] attribute
#   - Tests without assertions
#   - Missing [TestCase] attribute on test methods
#
# Adapted from tea-leaves (MIT, github.com/cleak/tea-leaves) for Cows & Graveyards.

param(
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"
$script:exitCode = 0
$script:warnings = @()
$script:errors = @()

function Add-Warning($file, $line, $message) {
    $script:warnings += [PSCustomObject]@{
        File = $file
        Line = $line
        Message = $message
    }
    if ($script:exitCode -lt 1) { $script:exitCode = 1 }
}

function Add-Error($file, $line, $message) {
    $script:errors += [PSCustomObject]@{
        File = $file
        Line = $line
        Message = $message
    }
    $script:exitCode = 2
}

Write-Host "Linting gdUnit4 C# test files..." -ForegroundColor Cyan

# Get all test files (convention: *Test.cs in tests/ directory)
$testDir = "tests"
$testFiles = @()
if (Test-Path $testDir) {
    $testFiles = Get-ChildItem -Path $testDir -Filter "*Test.cs" -Recurse -ErrorAction SilentlyContinue
}

if ($testFiles.Count -eq 0) {
    Write-Host "No test files found (looking for *Test.cs in tests/)" -ForegroundColor Yellow
    exit 0
}

Write-Host "  Found $($testFiles.Count) test files" -ForegroundColor Gray

# Pattern 1: Check for [TestSuite] attribute
Write-Host "  Checking for [TestSuite] attribute..." -ForegroundColor Gray
foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    if ($content -notmatch '\[TestSuite\]') {
        Add-Error $file.Name 1 "Test file must have [TestSuite] attribute"
    }
}

# Pattern 2: Check for [TestCase] methods
Write-Host "  Checking for [TestCase] methods..." -ForegroundColor Gray
foreach ($file in $testFiles) {
    $content = Get-Content $file.FullName -Raw
    if ($content -notmatch '\[TestCase\]') {
        Add-Warning $file.Name 1 "No [TestCase] methods found"
    }
}

# Pattern 3: Check test methods have assertions
Write-Host "  Checking for assertions in tests..." -ForegroundColor Gray
foreach ($file in $testFiles) {
    $lines = Get-Content $file.FullName
    $inTestMethod = $false
    $testMethodName = ""
    $hasAssertion = $false
    $braceDepth = 0
    $enteredBody = $false

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]

        if ($line -match '\[TestCase\]') {
            # Next method is a test
            $inTestMethod = $true
            $hasAssertion = $false
            $braceDepth = 0
            $enteredBody = $false
            continue
        }

        if ($inTestMethod -and $line -match 'public\s+void\s+(\w+)') {
            $testMethodName = $Matches[1]
        }

        if ($inTestMethod) {
            if ($line -match 'AssertThat|AssertThrown|AssertSignal|AssertBool|AssertInt|AssertFloat|AssertString|AssertArray|AssertObject') {
                $hasAssertion = $true
            }

            $braceDepth += ($line.Split('{').Count - 1)
            $braceDepth -= ($line.Split('}').Count - 1)

            if ($braceDepth -gt 0) {
                $enteredBody = $true
            }

            if ($enteredBody -and $braceDepth -le 0 -and $testMethodName -ne "") {
                if (-not $hasAssertion) {
                    Add-Warning $file.Name ($i + 1) "Test '$testMethodName' has no assertions"
                }
                $inTestMethod = $false
                $testMethodName = ""
                $enteredBody = $false
            }
        }
    }
}

# Pattern 4: Check for infinite loops
Write-Host "  Checking for potential infinite loops..." -ForegroundColor Gray
foreach ($file in $testFiles) {
    $lines = Get-Content $file.FullName
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match 'while\s*\(\s*true\s*\)') {
            Add-Error $file.Name ($i + 1) "Infinite loop 'while (true)' detected - ensure break condition exists"
        }
    }
}

# Report results
Write-Host ""
if ($script:errors.Count -gt 0) {
    Write-Host "ERRORS ($($script:errors.Count)):" -ForegroundColor Red
    foreach ($err in $script:errors) {
        Write-Host "  $($err.File):$($err.Line): $($err.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

if ($script:warnings.Count -gt 0) {
    Write-Host "WARNINGS ($($script:warnings.Count)):" -ForegroundColor Yellow
    foreach ($warn in $script:warnings) {
        Write-Host "  $($warn.File):$($warn.Line): $($warn.Message)" -ForegroundColor Yellow
    }
    Write-Host ""
}

if ($script:exitCode -eq 0) {
    Write-Host "All test lint checks passed!" -ForegroundColor Green
} else {
    Write-Host "Test lint found issues." -ForegroundColor $(if ($script:exitCode -eq 2) { "Red" } else { "Yellow" })
}

exit $script:exitCode
