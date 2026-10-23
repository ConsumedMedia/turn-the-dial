# UI

Source: GAME_BRIEF.md sections 10, 11, 13. The brief wins if this doc disagrees.

## Purpose
Two kinds of view. **Fights use the desk view**: a fixed seat in the booth where booth objects are the
HUD. **Everything else is a diorama**: high three-quarter isometric scenes. Readable everywhere (pillar 5).

## Rules

### Global
- Every screen works with mouse, keyboard and gamepad, with no exceptions.
- No text below 12 px at 1280×800.
- Every keyword has a tooltip. Every number that comes from a calculation has a tooltip showing the math.
- The color rule (brief section 11) is a hard requirement: **you = steady round amber dot on the left;
  rival = blinking teal diamond on the right.** Every you-versus-rival signal uses color, shape, side
  and motion. Rival teal is dimmer and deeper than your amber. Red means danger only.
- Every daypart wash passes a grayscale check.
- The UI only reads Core state and sends commands; it never computes a rule.

### Desk view (fight)
Layout follows `docs/refs/booth/KEY_booth_*.png`. Where the keys' tuner markers differ from the color
rule, the color rule wins.

| Booth object | Shows |
| --- | --- |
| Tuner (top of window) | Your frequency left (amber dot), rival's right (teal diamond). Needle drifts toward whoever leads on share. Rival intent and Monitor peeks above the rival's frequency. |
| Listener / audience readouts | Your listeners at the left end of the tuner, rival audience at the right end, as numbers. |
| Wall clock | Turns remaining (one tick per turn) and boss countdowns. |
| On-air light (red) | Steady while you're on air. Flickers and dims as a warning when ending the turn now would cause Dead Air, or when the next steal would end the run. |
| Corkboard (left wall) | Your statuses as sticky notes: Hype, Burnout on songs in play. Icon + stack count. |
| Sticky notes by the rival's frequency | Buzz, Lock, Exposed. Icon + stack count. |
| Desk mat | Your hand. |
| Left turntable | Cue slot. |
| Right turntable | The rival's current show (placard, boss/elite rules). |
| Mixer VU meters | Flow (left), Hype (center), Loyalty (right). Faders show Airtime remaining. |
| Record crate (left) | Draw pile count; select to view. Static count next to it. |
| Clipboard (right) | Discard pile and a log of this turn's plays. |
| Cash tin | Cash. |
| Shelf and desk props | Gear; select one for its tooltip. |
| Board by the door | Staff (3 slots). |

### Diorama screens
| Screen | Contents |
| --- | --- |
| Title | Station on its patch of ground. The tuner is the menu: Continue / New Run / Collection / Settings / Quit. |
| New run | Format choice (starter deck preview, max listeners, signature) and market choice (rule list). |
| Town map | Isometric town for the current book; nodes are real places with icons and labels; path lines, current position, listeners, Cash, Request tokens, deck/gear/staff buttons. |
| Reward | Cash gained, 1-of-3 card pick with Skip and Reroll (token), gear if any. |
| Sponsor Meeting | Diner booth with the sponsor. Stock with prices, sale tag, cut service, Reroll. |
| Production Room | Recover (exact amount) or Remaster (each card next to its RM version). |
| Station Event | Diorama of the place, text in a radio-filtered panel, choices with exact costs and odds. |
| Remote Broadcast | Van at the location. Pick 1 of 2 gear. |
| Deck / Collection | Crate browser. Filter by type, genre, tempo; remaster previews. |
| Run end | "Ratings book" report: books reached, fights won, Ratings Points, unlocks. |
| Settings | Game speed toggle, text size, controller remap, screen shake, colorblind preview, audio mix. |

### Input
- Input System action maps: `UI`, `Desk`, `Map`. Gamepad: d-pad/stick moves focus between booth objects
  and cards; A plays/confirms, B backs out, X cues, Y opens tooltip, Start ends turn (with confirm when
  it would cause Dead Air). Keyboard mirrors gamepad; mouse hover shows tooltips. **Proposed** (the
  button map and the Dead Air confirm; the brief requires only full controller support).
- Focus is always visible. No dead ends: every screen has a focusable way back.
- Glyphs switch to the last-used device.

### Guided first fight
First run only: a scripted Chill Coastal Village fight with callouts on the tuner, hand, Flow meter,
intent and on-air light. Rules are identical to a normal fight.

## Tunable values
| Key | Start |
| --- | --- |
| `ui.minTextPx` | 12 at 1280×800 |
| `ui.gameSpeeds` | 1×, 2× *(proposed)* |
| `ui.textScale` | 100%, 125%, 150% *(proposed)* |
| `ui.rivalBlinkHz` | 1.0 *(proposed)* |
| `ui.needleLerp` | 0.5 s *(proposed)* |

The brief requires a speed toggle, a text-size setting, a blinking rival and a drifting needle, but
gives no values; every value above except `ui.minTextPx` is **Proposed**.

## Edge cases
- 10 cards in hand at 1280×800 with 150% text: the desk mat fans/overlaps cards but each stays selectable.
- Monitor showing 3+ ghosted intents: they shrink, never overlap the rival's readout.
- Colorblind preview modes: every you-versus-rival signal still distinguishable.
- Gamepad disconnect mid-fight: pause and prompt.

## Open questions (ask before building)
- Gamepad button map (A play, B back, X cue, Y tooltip, Start end turn) and the end-turn confirm when
  it would cause Dead Air. Proposed above.
- Game speeds 1× and 2×; text scales 100 / 125 / 150%; rival blink 1.0 Hz; needle drift 0.5 s.
  Proposed above.
- Is a fight node's rival shown on the town map? See `map.md` (proposed: no).

## Done checklist
- [ ] Every screen in the tables built and navigable with mouse, keyboard and gamepad
- [ ] Text-size audit at 1280×800 (no text under 12 px)
- [ ] Grayscale check passed for all four washes
- [ ] Color rule audit: every you-vs-rival signal uses all four cues
- [ ] Tooltips on every keyword and calculated number
- [ ] Guided first fight
- [ ] Steam Deck test pass (`steam.md`)
- [ ] Reviewer sign-off
