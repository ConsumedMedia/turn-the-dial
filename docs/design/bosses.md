# Bosses (Ratings Books)

Source: GAME_BRIEF.md sections 2, 7, 9. The brief wins if this doc disagrees.

## Purpose
One boss ends each book. Each boss has one telegraphed rule shown from turn 1 on a placard and the
wall clock, and an answer you can reasonably build (pillar 2). In Big Metro (market 6) every boss
adds a second telegraphed rule.

## Rules
- 8 turns. Audience 200 / 280 / 360. Intents use that book's values.
- Losing on ratings ends the run. Winning books 1 and 2: 50 Cash (`reward.cash.boss`, `nodes.md`),
  a 1-of-3 rare card pick, and 1 of 3 boss gear. Winning book 3 wins the run.
- Every boss rule and countdown is visible from turn 1.

### Book 1: The Big Wake-Up (syndicated national morning show)
**Satellite Feed:** every 3rd turn (turns 3 and 6) is a giant Big Segment. The wall clock counts down.
*Answer:* save Loyalty for it, or cash a Drop right before it.

**Proposed:** Satellite Feed 24 is above the brief's book 1 Big Segment range (2× Steal, at most 18).
The brief calls it a "giant" Big Segment, so this doc reads the boss rule as allowed to exceed the
range; if the range is meant to bind bosses too, this is a brief change or the Feed drops to 18.

| T1 | T2 | T3 | T4 | T5 | T6 | T7 | T8 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| S 8 | HR 6 | Satellite Feed 24 | S 9 | C 8 | Satellite Feed 24 | S 9 | HR 6 |

**Big Metro second rule — Network Promo:** in the rival phase before each Satellite Feed (turns 2
and 5), the boss also gains Buzz 2, so each Feed lands at Buzz 1 (24 → shown 36). Shown on the clock.
*Answer:* the same Loyalty plan, bigger; or end the fight before turn 6.

### Book 2: Endless Stream (streaming giant)
**Algorithm:** the genre you played most last turn gets −50% Reach this turn (shown on the placard).
*Answer:* mix genres, use DJ breaks, use Hype.

| T1 | T2 | T3 | T4 | T5 | T6 | T7 | T8 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| S 11 | HR 9 | S 12 | I 2 Static → draw | S 12 | HR 9 | BS 2 hits × 12 | S 13 |

**Big Metro second rule — Recommended for You.** The Algorithm's −50% also applies to
whichever genre you have played most this daypart overall (shown on the placard next to last
turn's genre). Only finished turns count: the placard is set at the start of each turn and doesn't
change during it. If two or more genres are tied for most played, no extra penalty applies that turn.
A genre targeted by both rules still takes −50% once; the penalties don't stack. **Decided** (by
the user, commit 1d38661: "Decide Recommended for You timing and ties").
*Answer:* still mixing genres, but planned across the whole fight, not just turn to turn.

### Book 3: The Flip (corporate format flip)
**The Flip:** at turn 5, falling tempo builds Flow instead of rising tempo, and your genre streak
stops counting. The countdown shows from turn 1.
*Answer:* decks that don't only depend on Flow; cue a card for the flip.

| T1 | T2 | T3 | T4 | T5 | T6 | T7 | T8 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| S 13 | HR 12 | S 14 | T 3 | BS 2 hits × 14 (shown 2 × 21) | C 16 | S 15 | BS 2 hits × 15 |

**Big Metro second rule — New Management:** from turn 5, you draw 4 cards per turn instead of 5.
*Answer:* Hold, the cue slot, and draw cards.

## Tunable values
| Key | Start |
| --- | --- |
| `boss.audience` | 200 / 280 / 360 |
| `turns.boss` | 8 (defined in `combat.md`) |
| `reward.cash.boss` | 50 (defined in `nodes.md`) |
| `satelliteFeed.every / amount` | 3 / 24 |
| `networkPromo.buzz` | 2 |
| `algorithm.reachFactor` | 0.5 |
| `flip.turn` | 5 |
| `newManagement.draw` | 4 |

## Edge cases
- Algorithm on turn 1: no previous turn, no penalty.
- Algorithm tie (two genres equally most): both get −50% (**Proposed** default; see open question for
  the no-penalty alternative). Shown on the placard.
- Algorithm with only non-song cards played last turn: no penalty this turn.
- Recommended for You on turn 1: no songs played yet this daypart, no penalty.
- Recommended for You: songs played this turn don't change this turn's target; they count from
  next turn. The placard tooltip shows the per-genre counts from finished turns.
- Recommended for You tie (two or more genres equally most): no extra penalty that turn. The
  Algorithm's own last-turn penalty still applies.
- The Flip: Loop's "T1 after T5" rule becomes "T5 after T1" counts as falling? Proposed: Loop's
  special case mirrors (T5 right after T1 counts as a falling ramp).
- The Flip with Air Horn: Air Horn no longer changes tempo ("your next Song counts as a rising
  ramp", user 2026-10-23). **Decided** (user, 2026-10-23): after the flip, the Air-Horned Song still counts as a ramp
  (the falling ramp the Flip rewards), +1 Flow.
- Flow earned before turn 5 is kept at the flip. **Proposed.**
- Algorithm on a Drop Song of the targeted genre: ×0.5 multiplies the whole Drop number (base + Hype
  + N × Flow). **Decided** (brief §3: boss modifiers multiply the whole Drop number).
- Algorithm and a non-song Drop card (Hype Man): no genre, so no penalty.
- Satellite Feed fired while the boss has Lock: Lock doesn't affect steals.
- New Management turn 5 draw happens at turn start, so it applies to turn 5's draw.

## Open questions (ask before building)
- Boss names above are working names. Confirm or rename.
- Algorithm ties. Proposed default: both tied genres get −50% (edge case above). Alternative: a tie
  gives no penalty that turn. The alternative is consistent with Recommended for You's tie rule
  (Decided by the user, commit 1d38661: a tie gives no extra penalty), and this doc recommends it.
  The user decides.
- Loop under The Flip (proposal above). Air Horn under The Flip is Decided.
- Satellite Feed size (24 at book 1) — tune in the sim against Classic Rock's Loyalty. It exceeds the
  brief's book 1 Big Segment range (max 18); Proposed: allowed as a boss rule (see above).
- Flow earned before turn 5 is kept at the flip. Proposed: yes.

## Done checklist
- [ ] Boss data assets with rule ids and patterns
- [ ] Each base rule tested, including the edge cases
- [ ] Each Big Metro rule tested and only active in Big Metro
- [ ] Placard and clock show every rule and countdown from turn 1
- [ ] Sim: each boss beatable by at least one reasonable deck per format
- [ ] Reviewer sign-off
