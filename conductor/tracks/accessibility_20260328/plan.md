# Plan: Accessibility Foundations

**Track ID:** accessibility_20260328
**Status:** Pending

## Overview
Three phases moving from contrast auditing through accessible labels and finally score color differentiation. Each phase follows strict TDD: tests are written and failing before any implementation code is added.

---

## Phase 1: Contrast Audit

**Goal:** Audit contrast ratios for score text, buttons, pause button, cow entity, and gravestone entity against their backgrounds. Fix any elements that fall below WCAG AA thresholds. Establish a reusable contrast-checking utility for ongoing use.

### Task 1.1 — Write contrast utility tests
- Write gdUnit4 tests in `tests/accessibility/ContrastUtilityTest.cs`
- Test: `ContrastUtility.RelativeLuminance(Color c)` returns correct values for known sRGB inputs (white → 1.0, black → 0.0, mid-gray #808080 ≈ 0.216)
- Test: `ContrastUtility.ContrastRatio(Color a, Color b)` returns correct ratio for white/black (21:1) and two identical colors (1:1)
- Test: `ContrastUtility.MeetsWcagAA(Color fg, Color bg, bool largeText)` returns true for passing pairs and false for failing pairs
- Commit: `test: ContrastUtility WCAG AA helpers`

### Task 1.2 — Implement ContrastUtility
- Create `scripts/accessibility/ContrastUtility.cs` in namespace `CowsGraveyards.Accessibility`
- Implement `RelativeLuminance`, `ContrastRatio`, and `MeetsWcagAA` per WCAG 2.1 §1.4.3 (sRGB → linear conversion, luminance formula)
- All Task 1.1 tests must pass
- Commit: `feat: ContrastUtility WCAG AA contrast ratio helpers`

### Task 1.3 — Write contrast audit tests for UI elements
- Write gdUnit4 tests in `tests/accessibility/ContrastAuditTest.cs`
- Test: score text color against score panel background meets WCAG AA large-text (3:1)
- Test: pause button label/icon color against button background meets WCAG AA normal (4.5:1)
- Test: main menu button text against button background meets WCAG AA normal (4.5:1)
- Test: cow entity primary color against the median gameplay background color meets WCAG AA graphical (3:1)
- Test: gravestone entity primary color against the median gameplay background color meets WCAG AA graphical (3:1)
- Commit: `test: WCAG AA contrast audit for score, buttons, and entities`

### Task 1.4 — Fix contrast failures
- Review test failures from Task 1.3 and adjust theme colors, entity modulates, or background values until all audits pass
- Document any color changes in a comment referencing the WCAG criterion (e.g. `// WCAG AA 1.4.3 — contrast ratio 4.7:1`)
- All Task 1.3 tests must pass
- Commit: `fix: adjust UI and entity colors to meet WCAG AA contrast thresholds`

### Verification checkpoint
Run `dotnet test --filter "FullyQualifiedName~CowsGraveyards.ContrastUtilityTest|CowsGraveyards.ContrastAuditTest"` — all green before proceeding.

---

## Phase 2: Accessible Labels

**Goal:** Add accessible names to all interactive Control nodes so that screen readers have meaningful text to announce for every button and interactive label.

### Task 2.1 — Write accessible label tests
- Write gdUnit4 tests in `tests/accessibility/AccessibleLabelTest.cs`
- Test: PauseButton node has a non-empty `AccessibilityName` (or `tooltip_text` fallback) equal to "Pause game"
- Test: LeftScoreLabel node has a non-empty accessible name containing "Left score" or "Cows left"
- Test: RightScoreLabel node has a non-empty accessible name containing "Right score" or "Cows right"
- Test: all Button-derived nodes in MainMenuScene have non-empty `tooltip_text` or `AccessibilityName`
- Test: all Button-derived nodes in PauseMenu scene have non-empty `tooltip_text` or `AccessibilityName`
- Commit: `test: accessible names required on all interactive Control nodes`

### Task 2.2 — Implement accessible labels on game scene nodes
- Add `tooltip_text` (or `accessibility_name` if available in Godot 4.6 API) to PauseButton, LeftScoreLabel, RightScoreLabel in GameScene
- Use descriptive, action-oriented text: "Pause game", "Left player score", "Right player score"
- All corresponding Task 2.1 tests must pass
- Commit: `feat: accessible labels on GameScene interactive nodes`

### Task 2.3 — Implement accessible labels on menu nodes
- Add accessible labels to all interactive nodes in MainMenuScene and PauseMenu
- Ensure button labels describe the action, not just the visual text (e.g. "Start new game" not just "Start")
- All remaining Task 2.1 tests must pass
- Commit: `feat: accessible labels on MainMenuScene and PauseMenu nodes`

### Verification checkpoint
Run `dotnet test --filter "FullyQualifiedName~CowsGraveyards.AccessibleLabelTest"` — all green before proceeding.

---

## Phase 3: Score Color Differentiation

**Goal:** Add distinct color coding to the left and right score displays so that score identity is communicated through color in addition to position. Use a colorblind-safe palette that passes WCAG AA contrast requirements.

### Task 3.1 — Write score color differentiation tests
- Write gdUnit4 tests in `tests/accessibility/ScoreColorTest.cs`
- Test: LeftScoreLabel uses a distinct color value different from default white/gray
- Test: RightScoreLabel uses a distinct color value different from LeftScoreLabel color
- Test: left score color against score panel background meets WCAG AA normal (4.5:1) per ContrastUtility
- Test: right score color against score panel background meets WCAG AA normal (4.5:1) per ContrastUtility
- Test: left and right score colors are distinguishable in a deuteranopia simulation (hue difference > 30 degrees in LCH space, or explicitly verified against Wong palette constants)
- Commit: `test: score color differentiation — distinct, colorblind-safe, WCAG AA`

### Task 3.2 — Implement score color differentiation
- Define named color constants for left and right scores in a central `AccessibilityColors.cs` (namespace `CowsGraveyards.Accessibility`):
  - `LeftScoreColor`: blue, e.g. `#0072B2` (Wong palette — safe for deuteranopia, protanopia, tritanopia)
  - `RightScoreColor`: orange, e.g. `#E69F00` (Wong palette)
- Apply colors to LeftScoreLabel and RightScoreLabel in GameScene (via theme override or script-set `add_theme_color_override`)
- Verify colors visually do not clash with existing score panel background
- All Task 3.1 tests must pass
- Commit: `feat: colorblind-safe color coding for left/right scores`

### Task 3.3 — Final integration and lint
- Run full test suite: `dotnet test "Cows & Graveyards.csproj" --settings .runsettings`
- Run project lint: `pwsh tools/godot.ps1 --headless -s tools/lint_project.gd`
- Run test convention lint: `pwsh tools/lint_tests.ps1`
- Confirm all success criteria from spec.md are met
- Commit: `chore: accessibility track final verification`

### Verification checkpoint
All tests green. No lint errors. Manual smoke check: score labels display in distinct colors; pause button is labeled; contrast ratios verified.

---

## Summary

| Phase | Tasks | Focus |
|-------|-------|-------|
| 1 — Contrast Audit | 4 | Utility helpers, audit existing UI, fix failures |
| 2 — Accessible Labels | 3 | Accessible names on all interactive nodes |
| 3 — Score Color Differentiation | 3 | Colorblind-safe palette, WCAG AA contrast |
| **Total** | **10** | |
