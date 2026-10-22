# Flow, Drop and Hype

Source: GAME_BRIEF.md sections 2, 3, 5. The brief wins if this doc disagrees.

## Purpose
Flow is the combo system: sequencing songs well multiplies Reach. Drop cashes Flow in for a burst.
Hype is a flat bonus built by Promos. Together they are the radio-programming skill of the game.

## Rules

### Flow
- Range 0 to the Flow cap. Cap is 10 (15 with Loop; gear and cards can raise it).
- Each stack gives Songs +10% Reach: multiplier `1 + 0.1 × Flow`.
- Lasts the whole daypart; resets to 0 at daypart start.
- **Gaining Flow** (after a Song resolves its Reach):
  - +1 **tempo ramp** if its tempo is higher than the last song's tempo.
  - +1 **genre streak** if both it and the last song are your format's genre.
  - The first song of a daypart (no last song) gains nothing.
  - A lower or equal tempo only misses the ramp bonus; it doesn't break Flow.
- **Breaking Flow (set to 0):** playing an Ad, taking Dead Air, certain rival effects.
- **Last song:** the most recent Song played this daypart. Non-song cards don't change it,
  **except an Ad, which clears it** (the next song has no last song).
- Flow over the cap is clamped to the cap.

### Signature modifiers
- **Loop (Hip-Hop Heat):** cap 15. A tempo 1 song right after a tempo 5 song counts as a rising ramp.
- **Air Horn:** your next Song counts as +1 tempo for the ramp check only (not for display or
  other rules). Air Horn RM also draws 1.
- **The Flip (boss 3):** from turn 5, falling tempo builds the ramp instead of rising tempo, and the
  genre streak stops counting (see `bosses.md`).

### Drop
`Drop N`: deal the card's base Reach + N × Flow, ignoring the Flow multiplier, then set Flow to 0.
- If the card is a Song, Hype and Burnout still apply:
  `Reach = floor((base + Hype + N × Flow) × Anthem × Burnout × Exposed × boss modifiers)`
- If not a Song: `Reach = floor((base + N × Flow) × Exposed)`.
- Exposed applies (it affects all your Reach).

### Hype
- Each stack gives every Song +1 Reach, added to base before the Flow multiplier.
- −1 at end of your turn. Mailbag: no decay on a turn you played a DJ Break.
- Non-song cards ignore Hype.
- No cap.

### Genre streak count
Playlist Bot uses "your genre streak" as a number: the count of consecutive Songs of your
format's genre ending at the last song (resets on an off-genre song or an Ad clearing last song).

## Tunable values
| Key | Start |
| --- | --- |
| `flow.cap` | 10 |
| `flow.capLoop` | 15 |
| `flow.reachPerStack` | 0.10 |
| `flow.rampGain` | 1 |
| `flow.streakGain` | 1 |
| `hype.decayPerTurn` | 1 |

## Edge cases
- Song played into its own Flow gain: the gain applies after its Reach (tested).
- Air Horn played, then an Ad, then a Song: Air Horn's bonus still applies to the next Song.
  The Ad cleared last song, so there's no ramp anyway; the bonus is consumed.
- Two Air Horns before one Song: +2 tempo for the ramp check.
- Loop: tempo 5 → tempo 1 is a ramp; tempo 5 → tempo 2 is not.
- Flow at cap gaining +2: stays at cap.
- Drop with Flow 0: deals base (+Hype if Song).
- Hype at 0 at end of turn: stays 0.
- Same-name songs with different tempos (Block Party T1–T4) still share Burnout.

## Open questions (ask before building)
- **Drop Songs (Banger):** Drop sets Flow to 0, but the brief also says a Song updates Flow after its
  Reach. Does Banger leave Flow at 0, or reset then gain (ending at 1–2)? Proposed: reset last, Flow ends at 0;
  last song becomes Banger.
- Hype decrement under Mailbag: does a DJ Break played from the cue slot count? Proposed: yes.

## Done checklist
- [ ] Ramp, streak, first-song, lower-tempo and cap rules tested
- [ ] Ad breaks Flow and clears last song; Dead Air breaks Flow
- [ ] Loop 5→1 ramp and cap 15
- [ ] Air Horn ramp bonus, stacked and across an Ad
- [ ] Drop formula for Song and non-song, Flow reset
- [ ] Hype added before multiplier, decay, Mailbag exception
- [ ] Genre streak count exposed for Playlist Bot
- [ ] Reviewer sign-off
