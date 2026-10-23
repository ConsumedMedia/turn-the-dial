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
- **Breaking Flow (set to 0):** playing an Ad, taking Dead Air, certain rival effects, and every Drop
  card (below).
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
**Decided (brief §3).** A Drop card deals one number as one hit:

`Reach = floor((base + Hype + N × Flow) × Anthem × Burnout × Exposed × boss modifiers)`

- **No Flow multiplier.** Flow only enters through N × Flow.
- **Hype and Burnout apply only if the card is a Song.** For a non-song Drop card (e.g. Hype Man, a
  DJ Break) Hype counts as 0 and the Burnout factor is 1.
- **Anthem** (×1.5) applies only if the card is an Anthem Song that gets the bonus this turn
  (`formats.md`); otherwise 1.
- **Exposed** (×1.5) applies to every Drop card while the rival is Exposed.
- **Boss modifiers** multiply the whole number whenever they apply to the card (e.g. Algorithm ×0.5
  on a Drop Song of the targeted genre).
- Every multiplier applies to the whole number, including the N × Flow part, then one floor.
- Flow in the formula is the value before this card's own Flow update.
- **One hit:** Lock absorbs the whole number once and it converts to listeners once (`combat.md`).
  A Drop card's base Reach and its Drop part are never resolved separately.
- **The Drop resolves last.** On a Drop Song, the order is: Reach, then the song's own Flow
  update (ramp/streak), then the Drop sets Flow to 0, wiping that update. Flow is always 0 after a
  Drop card. The Drop Song still becomes the last song.

### Hype
- Each stack gives every Song +1 Reach, added to base before the Flow multiplier.
- −1 at end of your turn. Mailbag: no decay on a turn you played a DJ Break.
- Non-song cards ignore Hype (including non-song Drop cards).
- No cap. **Proposed** (the brief sets no cap; decay already limits it).

### Genre streak count
**Proposed.** Playlist Bot uses "your genre streak" as a number: the count of consecutive Songs of
your format's genre ending at the last song (resets on an off-genre song or an Ad clearing last
song). A single on-genre song counts as 1. Reasoning: the brief's streak is a yes/no Flow bonus, so a
count needs its own definition; counting from the first on-genre song makes Playlist Bot's Lock
visible and growing from the first song.

## Tunable values
| Key | Start |
| --- | --- |
| `flow.cap` | 10 |
| `flow.capLoop` | 15 |
| `flow.reachPerStack` | 0.10 |
| `flow.rampGain` | 1 |
| `flow.streakGain` | 1 |
| `hype.decayPerTurn` | 1 |

Drop N is per card (`keywords` data); Anthem, Exposed and boss factors live in `formats.md`,
`combat.md` and `bosses.md`.

## Edge cases
- Song played into its own Flow gain: the gain applies after its Reach (tested).
- Air Horn played, then an Ad, then a Song: Air Horn's bonus still applies to the next Song.
  The Ad cleared last song, so there's no ramp anyway; the bonus is consumed. **Proposed.**
- Two Air Horns before one Song: +2 tempo for the ramp check. **Proposed.**
- Loop: tempo 5 → tempo 1 is a ramp; tempo 5 → tempo 2 is not.
- Flow at cap gaining +2: stays at cap.
- Drop with Flow 0: deals base (+Hype if Song).
- Non-song Drop card with Hype 3: Hype is ignored (Hype Man at Flow 4: 3 + 2 × 4 = 11).
- Drop Song with Hype 2 and Flow 4 (Banger, Drop 3): (7 + 2 + 12) = 21, no Flow multiplier.
- Drop on an Exposed rival: ×1.5 on the whole number, including N × Flow (Hype Man at Flow 4: 16).
- Drop Song with 1 Burnout: ×0.75 on the whole number, including N × Flow.
- Drop card into Lock: Lock absorbs the whole number once; one listener conversion.
- Hype at 0 at end of turn: stays 0.
- Same-name songs share Burnout regardless of tempo (the Hip-Hop starter songs have distinct names
  so they don't).
- Banger (T5 Hip-Hop, Drop 3) after Rooftop Jam (T4 Hip-Hop) in a Hip-Hop Heat deck: earns the ramp
  and the genre streak (+2), then the Drop wipes it; Flow ends at 0 and the next song ramps from
  tempo 5.
- Genre streak count with one on-genre song as last song: 1. After an Ad: 0.

## Open questions (ask before building)
- Hype decrement under Mailbag: does a DJ Break played from the cue slot count? Proposed: yes.
- Hype has no cap. Proposed above.
- Genre streak count for Playlist Bot (a single on-genre song = 1). Proposed above.
- Air Horn: bonus survives an Ad and is consumed by the next Song; two Air Horns stack to +2.
  Proposed above.

## Done checklist
- [ ] Ramp, streak, first-song, lower-tempo and cap rules tested
- [ ] Ad breaks Flow and clears last song; Dead Air breaks Flow
- [ ] Loop 5→1 ramp and cap 15
- [ ] Air Horn ramp bonus, stacked and across an Ad
- [ ] Drop formula for Song and non-song as one number and one hit; every multiplier applies to the
      whole number; no Flow multiplier; Flow 0 after the song's own Flow update
- [ ] Hype added before multiplier, decay, Mailbag exception
- [ ] Genre streak count exposed for Playlist Bot
- [ ] Every edge case above has a test
- [ ] Reviewer sign-off
