# Specification: Accessibility Foundations

**Track ID:** accessibility_20260328
**Type:** Feature
**Created:** 2026-03-28
**Status:** Draft

## Summary
Add foundational accessibility support to the game: contrast audit and fixes for key UI elements, accessible names and labels on all interactive Control nodes, and color differentiation for left/right scores so that score identity does not rely solely on screen position.

## Context
Cows & Graveyards is a mobile road trip game targeting Android and iOS. Accessibility agent Avery evaluated the game on 2026-03-28 and rated it 2/5 overall, identifying a complete absence of accessibility accommodations as the primary concern. The game has zero accessibility settings and no semantic metadata on interactive elements.

## Motivation
- Avery (accessibility agent) gave 2/5 overall — lowest score of all player testing agents
- No accessibility settings exist: no font size toggle, no contrast mode, no colorblind palette, no screen reader support
- Pause button is small and carries no accessible name — screen readers have nothing to announce
- No alt-text or accessible labels on any interactive element
- Score differentiation relies entirely on position ("L:" left corner, "R:" right corner) with no color coding — fails users who cannot rely on spatial layout
- White cow against varied outdoor background may blend for contrast-sensitive users
- Gray gravestone against road/hills may be difficult to distinguish for low-contrast vision
- Positives to preserve: large tap zones are naturally motor-accessible, 72pt score font is readable, static camera helps vestibular sensitivity

## Success Criteria
- [ ] All interactive Control nodes (buttons, score labels, pause button) have `AccessibilityName` or equivalent accessible metadata set
- [ ] Pause button has a descriptive accessible label (e.g. "Pause game")
- [ ] Left score and right score are visually differentiated by color in addition to position, using a colorblind-safe two-color palette
- [ ] Score color palette passes WCAG AA contrast ratio (4.5:1) against the score panel background
- [ ] Cow entity visual contrast meets WCAG AA large-text threshold (3:1) against the expected background color range
- [ ] Gravestone entity visual contrast meets WCAG AA large-text threshold (3:1) against the expected background color range
- [ ] Score text (72pt) contrast meets WCAG AA large-text threshold (3:1) against score panel background
- [ ] Button/control contrast meets WCAG AA normal threshold (4.5:1) against their backgrounds

## Dependencies
- Current GameScene, CowEntity, GraveyardEntity, MainMenuScene, PauseMenu code
- Overlap with **ux-polish_20260328**: that track addresses pause button sizing; this track adds the accessible label — coordinate to avoid conflicting edits to the pause button node. Communicate via spec notes or branch ordering when both tracks are active.

## Out of Scope
- Full accessibility settings menu (font size slider, contrast mode toggle, colorblind palette selector) — planned as a future track
- Screen reader narration of gameplay events (cow spawned, score changed) — beyond foundational scope
- Reduced motion mode
- Font size customization
- Audio descriptions or haptic feedback accessibility features

## Technical Notes
- Godot 4 exposes `Control.tooltip_text` and the `accessibility_name` property (via theme or script) for screen reader integration; use these to label interactive nodes
- WCAG AA contrast ratios: 4.5:1 for normal text/UI components, 3:1 for large text (18pt+ or 14pt+ bold) and graphical objects
- Use an established colorblind-safe two-color pair for left/right scores — e.g. blue (#0072B2) and orange (#E69F00) from the Wong palette, which is safe for deuteranopia, protanopia, and tritanopia
- Contrast ratios can be verified programmatically: relative luminance formula per WCAG 2.1 §1.4.3
- Godot Color values map to sRGB; convert to linear before computing relative luminance
