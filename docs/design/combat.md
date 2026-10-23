# Combat (Daypart fights)

Source: GAME_BRIEF.md sections 2, 3, 7. The brief wins if this doc disagrees.

## Purpose
One daypart is one fight against one rival station over a fixed number of turns. You win by
pulling the rival's audience to 0 or by beating it on share at the ratings check. Combat owns the
turn loop, Reach and steal resolution, Dead Air, and win/loss. It lives in `TurnTheDial.Core`.

## Rules

### Fight length
| Fight | Turns |
| --- | --- |
| Daypart (regular) | 6 |
| Prime Time (elite) | 7 |
| Ratings Book (boss) | 8 |

One turn = one player phase, then one rival phase.

### Daypart start (in order)
1. Shuffle the deck into the draw pile. Flow, Hype, Burnout, Loyalty and the Dead Air count start
   at 0. The cue slot is empty. "Last song" is empty.
2. Apply market modifiers (see `markets.md`).
3. Apply daypart-start gear and staff effects: gear in acquisition order, then staff in slot order
   (left to right). **Proposed.** The same gear-before-staff order applies on every trigger they
   share (turn start, card played, end of turn, daypart won, and so on; `gear.md`, `staff.md`).
4. Reveal the rival's turn-1 intent.

### Player phase (in order)
1. **Turn start.** Loyalty clears to 0. Airtime is set to 3 plus modifiers. Start-of-turn gear and
   staff effects trigger.
2. **Draw 5.** If the draw pile is empty mid-draw, shuffle the discard pile into it and continue.
   Every shuffle uses the `rng.shuffle` stream (see `save.md`, RNG streams). Hand limit is 10; each
   extra draw goes straight to the discard pile. The cued card stays in the cue slot and can be
   played from there. The cued card does not count toward the hand limit. **Proposed.**
3. **Program.** Play cards in any order by paying their Airtime. Each card resolves fully, then win
   and loss are checked.
4. **Cue (optional, free).** Once per turn, at any time during Program, if the cue slot is empty,
   move 1 card from hand into it. Costs no Airtime and doesn't end your plays. A card cued this turn
   can't be played until your next turn. **Decided** (user, 2026-10-23), brief §2.
5. **End turn** (in order):
   1. Dead Air check: if no card with On Air was played this turn, take Dead Air.
   2. End-of-turn card effects resolve (e.g. Complaint Call in hand: lose 3 listeners).
   3. Discard the hand except Hold cards and the cued card. Unspent Airtime is lost.
   4. Hype −1 (unless Mailbag applies, see `formats.md`).

Win and loss are also checked after each end-turn step.

### Rival phase (in order)
1. The rival's Lock clears to 0.
2. The rival performs the intent you saw (see `rivals.md`).
3. The rival's Buzz −1 (if above 0).
4. If this was the last turn, run the ratings check. Otherwise reveal the next intent.

### Reach resolution
**Song (no Drop):**
`Reach = floor((base + Hype) × (1 + 0.1 × Flow) × Anthem × Burnout × Exposed × boss modifiers)`
- Anthem factor 1.5 when it applies (see `formats.md`), else 1.
- Burnout factor `max(0.25, 1 − 0.25 × stacks)`.
- Exposed factor 1.5 while the rival's shown intent is Commercial Break, else 1.
- Boss modifiers: e.g. Algorithm 0.5 on the targeted genre (see `bosses.md`).

**Non-song card with Reach:** `Reach = floor(base × Exposed)`. Skips Hype and the Flow multiplier.

**Drop N card (Song or not):** one number, `floor((base + Hype + N × Flow) × Anthem × Burnout ×
Exposed × boss modifiers)`, no Flow multiplier; Hype and Burnout only for Songs (`flow.md`).

**Applying Reach (every source):** each card's Reach is one hit, applied once. A Drop card's base
and Drop parts are one number, never two hits.
1. Lock absorbs Reach first.
2. Whatever is left lowers the rival's audience (not below 0).
3. You gain listeners equal to 10% (rounded down) of the Reach that got past Lock, up to your
   maximum (20% with Mailbag). The full post-Lock Reach counts, even past the rival's remaining audience.
4. For a Song: then update Flow from this song (`flow.md`). A song never benefits from the Flow it
   earns itself.
5. For a Drop card: then set Flow to 0 (after step 4 on a Drop Song). Flow is always 0 after a Drop
   card.

### Steal resolution
- A Steal of X (Buzz already included) is absorbed by Loyalty first; the rest lowers your listeners.
- A Big Segment shown as N hits resolves each hit separately through Loyalty.
- Steals don't add to the rival's audience.
- **Loyalty only absorbs rival steals. Decided** (brief §3 Loyalty row; LESSONS Loyalty line). Ad listener costs, Dead Air, Complaint Call and
  every other listener loss ignore Loyalty.

### Dead Air
Triggers at end of turn if no On Air card was played that turn.
- Lose 5 listeners the first time in a daypart, 10 the second, 15 each time after.
- Ignores Loyalty. Sets Flow to 0.
- Blue-Collar Heartland and up: 8 / 13 / 18.

### Win / loss
- Rival audience hits 0: you win immediately.
- Your listeners hit 0: the run ends immediately, in any fight.
- Both hit 0 from the same card: you win the fight and your listeners are set to 1. **Decided** (user, 2026-10-23), brief §2.
- **Ratings check** after the final rival phase: your share = listeners ÷ maximum listeners;
  rival share = audience ÷ its starting audience. Higher share wins; a tie goes to you.
- **Ratings loss** (regular or Prime Time): no rewards, lose 10% of current listeners (rounded up),
  maximum unchanged, the run continues. **Boss ratings loss ends the run.**

## Tunable values
| Key | Start |
| --- | --- |
| `turns.daypart / primeTime / boss` | 6 / 7 / 8 |
| `airtime.perTurn` | 3 |
| `draw.perTurn` | 5 |
| `hand.limit` | 10 |
| `reach.listenerConversion` | 0.10 |
| `exposed.factor` | 1.5 |
| `deadAir.steps` | 5 / 10 / 15 |
| `ratingsLoss.penalty` | 0.10 (rounded up) |
| `cueSlots` | 1 |

## Edge cases
- Rival hits 0 in the middle of a multi-effect card: the card finishes resolving, then you win.
- One card drops the rival to 0 and your listeners to 0 (e.g. Reach plus a listener cost): you win,
  listeners set to 1.
- Cue, then keep playing: allowed; the cued card stays in the slot and can't be played this turn.
- A second cue in the same turn (after playing last turn's cued card from the slot): not allowed,
  once per turn.
- Your listeners hit 0 from your own Ad, Dead Air, or Complaint Call: the run ends.
- Ratings-loss penalty reduces listeners to 0 (e.g. 1 listener, 10% rounded up = 1): the run ends.
- Draw pile and discard pile both empty: stop drawing, no penalty.
- Hand is full (10) with a cued card: the cued card doesn't count; extra draws go to discard.
- A turn with no On Air card but a cued card: still Dead Air (cueing isn't playing).
- Reach of 0 after floor: no audience change, no listeners gained, Flow still updates for a Song.
- All Reach absorbed by Lock: 0 listeners gained.
- Listener gain at maximum: capped, the excess is lost.
- Tie on share at the ratings check: you win.
- Commercial Break is the shown intent during the player phase in which Exposed applies.
- Drop card into Lock: one absorb of the combined number, one listener conversion (`flow.md`).
- Daypart-start gear or staff with a Loyalty effect: the turn-1 Loyalty clear (brief §2 order) wipes
  it before any steal. Proposed: such content is not allowed (see open question).

## Open questions (ask before building)
- **Daypart-start Loyalty.** Brief §2 runs daypart-start gear and staff (step 3) before turn 1's turn
  start, which clears Loyalty, so any Loyalty granted at daypart start is wasted. **Proposed:**
  daypart-start effects grant no Loyalty (the content validator rejects `GainLoyalty` on the
  daypart-start trigger; use the turn-start trigger instead). Alternative: grant daypart-start Loyalty
  after the turn-1 clear. That contradicts the brief's order and **needs a brief change**.
- On every trigger shared by gear and staff (daypart start, turn start, card played, end of turn,
  daypart won, and so on), gear resolves first in acquisition order, then staff left to right.
  Proposed above.
- The cued card doesn't count toward the hand limit. Proposed above.

## Done checklist
- [ ] Turn loop runs headless for 6, 7 and 8 turns with the exact phase order above
- [ ] Reach formula tested for Song, non-song, Drop (one hit), Exposed, Burnout floor, Anthem, Lock absorb
- [ ] Shuffles use only `rng.shuffle` (`save.md`)
- [ ] Listener conversion tested at 10%, 20%, rounding down and the maximum cap
- [ ] Steal, Big Segment hits and Loyalty absorb tested; Ad costs and Complaint Call ignore Loyalty
- [ ] Dead Air 5/10/15 and 8/13/18, ignoring Loyalty, breaking Flow
- [ ] Win/loss after every card and every end-turn step
- [ ] Ratings check, tie rule, ratings-loss penalty, boss loss ends run
- [ ] Every edge case above has a test
- [ ] Same seed + same inputs gives the same fight log
- [ ] Reviewer sign-off
