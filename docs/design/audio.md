# Audio

Source: GAME_BRIEF.md sections 11, 12. The brief wins if this doc disagrees.

## Purpose
Sound sells the broadcast: short original loops per format, all-hardware foley, and DJ chatter as
radio-filtered on-screen text. No licensed music, no voice acting.

## Rules
- **Music:** short original loops per format (Classic Rock, Hip-Hop Heat, Country Roads), plus title,
  map and boss variants. No licensed music. Source and budget are still open (brief "Still open").
- **DJ chatter:** on-screen text with a radio-filter visual treatment. No voice acting.
- **Foley is all hardware:** needle drops, fader clicks, tuner static, the on-air relay, crate flips,
  cash tin, clock tick.
- Audio reacts to Core events from the Game layer; Core never references audio.
- Mix buses: Master, Music, Foley, UI. Each has a slider in Settings.
- Game speed 2× shortens foley waits but never pitches music.

### Event → sound map (v1)
| Core event | Sound |
| --- | --- |
| Card played (Song) | Needle drop, then the song layer swells |
| Card played (other) | Fader click / mic switch |
| Flow up | Rising tone step on the mixer |
| Flow broken | Record scratch |
| Dead Air | Silence gap, then hiss |
| Rival intent revealed | Tuner static sweep |
| Steal hits you | Low thud + needle dip |
| Ad played | Cash tin |
| Turn end | Clock tick |
| On-air warning | Relay chatter (paired with the light flicker, never alone) |
| Win / loss | Station sting / power-down |

## Tunable values
| Key | Start |
| --- | --- |
| `audio.musicLoopLength` | 30–60 s |
| `audio.defaultMix` | Master 80, Music 70, Foley 80, UI 70 |
| `audio.flowStepSemitones` | 1 |

## Edge cases
- Many events in one frame (multi-hit Big Segment): sounds queue with a short stagger, capped voices.
- Muted Music: gameplay cues stay audible on Foley.
- Every audio cue that carries meaning also has a visual (accessibility).

## Open questions (ask before building)
- Music source and budget (brief, still open). Blocks final music, not the first playable.

## Done checklist
- [ ] Audio buses and Settings sliders
- [ ] Event → sound map wired through Game-layer listeners
- [ ] Placeholder loops per format; final loops once the source is decided
- [ ] Files in `audio/clean` trimmed and loudness-normalized
- [ ] Reviewer sign-off
