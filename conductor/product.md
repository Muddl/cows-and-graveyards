# Product Definition

## Project Name

Cows & Graveyards

## Description

A mobile game that serves as a visual aid for the classic cow-counting road trip car game.

## Problem Statement

Players need a simple, fun tool to replace mental scorekeeping during long car rides.

## Target Users

Families and friends on road trips looking for a fun car game companion.

## Key Goals

- **Simple, thumb-friendly mobile experience** — easy to use while riding in a car
- **Fun visual feedback** — make scorekeeping engaging, not tedious
- **Quick to pick up and play** — minimal onboarding, instant fun

## Current Feature State

Shipped features (as of 2026-03-28):

- **Game Scene** — Core gameplay with cow spawning, side detection, score tracking, and graveyard penalty mechanic
- **Menus & Persistence** — Main menu, pause menu, trip save/load via `SaveManager`, trip history
- **Visual Upgrade** — Medium-fidelity art pass with grass/stone shaders, cow and graveyard entity visuals, visual effects
- **Audio & SFX** — AudioManager with ambient audio, music, core SFX, graveyard penalty sounds, and audio polish
- **Quality Harness** — Tea-leaves integration: DevTools autoload, SceneValidator, project/shader linting, test convention linting
- **Onboarding & Tutorial** — First-play tutorial overlay (4-step coach marks), graveyard first-use tooltip, improved score labels with color differentiation, replay tutorial button, tutorial state persistence per save slot
