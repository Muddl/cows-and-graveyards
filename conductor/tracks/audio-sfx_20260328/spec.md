# Specification: Audio & Sound Effects

**Track ID:** audio-sfx_20260328
**Type:** Feature
**Created:** 2026-03-28
**Status:** Draft

## Summary
Add core audio feedback to the game: tap confirmation sounds, cow moo on spawn, gravestone thud, ambient road noise, and basic background music. This was the #1 critical finding from Round 1 player testing — all 7 agents flagged the silence.

## Context
Cows & Graveyards is a mobile road trip game. Player testing round 1 (2026-03-28) revealed that the complete absence of audio makes every interaction feel dead and unconfirmed. On mobile, sound is half the game feel.

## Motivation
- Flagged by ALL 7 test agents as the #1 issue
- Game Feel category scored lowest (avg 2.0/5) largely due to missing audio
- Children found the silence "devastating" — no moo when cow appears
- Commuters doubted whether their taps registered without audio confirmation

## Success Criteria
- [ ] Tap sound plays on cow count (left/right tap)
- [ ] Cow moo sound plays when cow entity spawns
- [ ] Gravestone thud/crumble plays when gravestone spawns
- [ ] Graveyard score-wipe has a distinct warning/penalty sound
- [ ] Ambient road noise loop plays during gameplay
- [ ] Background music plays (menu and/or gameplay, with separate volume)
- [ ] Audio respects device mute/silent mode
- [ ] Audio bus structure allows independent volume control (SFX, Music, Ambient)
- [ ] No audio pops, clicks, or distortion on rapid tapping

## Dependencies
- Current GameScene, CowEntity, GraveyardEntity, MainMenuScene code
- Audio asset files (can use placeholder/CC0 sounds initially)

## Out of Scope
- Voice acting or narration
- Spatial/3D audio positioning
- Audio settings UI (future track)
- Procedural audio generation

## Technical Notes
- Use Godot AudioStreamPlayer / AudioStreamPlayer2D nodes
- Set up audio bus layout: Master → SFX, Music, Ambient
- Use .ogg or .wav formats (Godot preference for mobile)
- Pool AudioStreamPlayers for rapid tap sounds to avoid cutoff
- Check AudioServer.IsBusFocused for device mute respect
