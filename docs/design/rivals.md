# Rivals and Prime Time elites

Source: GAME_BRIEF.md sections 2, 3, 7. The brief wins if this doc disagrees.
Bosses are in `bosses.md`.

## Purpose
Each rival is a show with one clear personality, expressed through a fixed, readable intent pattern.
No hidden randomness: the pattern is the rival. Players learn it and program around it (pillar 1).

## Rules

### Intents
| Code | Intent | Effect | Book 1 / 2 / 3 *(tune)* |
| --- | --- | --- | --- |
| S | Steal | Take X listeners; Loyalty absorbs first | 6–9 / 9–13 / 12–16 |
| BS | Big Segment | Large steal, 2× Steal; may show as N hits | 2× Steal |
| CB | Commercial Break | Skips its move; rival is Exposed during your turn | — |
| C | Contest | Rival regains X audience, not above its start | 8 / 12 / 16 |
| HR | Heavy Rotation | Rival gains X Lock | 6 / 9 / 12 |
| I | Interference | Adds N Static or Complaint Calls to your draw or discard pile | 1–2 / 2–3 / 3 |
| T | Trending | Rival gains Buzz N | 2 / 2 / 3 |

- The shown number is final: Buzz and Exposed already included. A tooltip shows the math.
- Patterns are listed by turn. Values below are base values before Buzz. If a fight lasts longer than
  the pattern, it loops from turn 1. **Proposed** (loop rule).
- Monitor reveals the next intents as smaller ghosted icons after the current one.
- Market modifiers (College Town +10% audience, True Crime Suburbia Buzz 1) apply on top.

### Regular rivals (12, 4 per book, 6 turns)
**Decided** (by the user, commit c1a8390: "swap Playlist Bot (book 2) and Oldies Gold 1490
(book 1)"): Playlist Bot is in book 2. Oldies Gold 1490 moved to book 1 in its place: it is the
gentlest of the book 2 shows (mostly Lock, small Steals, no Buzz or Interference), so book 1 stays the
easiest. Both were rescaled to their new book's values.

**Book 1** (audience 45–55)
| Rival | Aud. | Personality | T1 | T2 | T3 | T4 | T5 | T6 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Morning Zoo | 50 | Chaotic. Heavy Interference, small mixed Steals. | I 2 Static → draw | S 6 | I 1 Complaint Call → discard | S 7 | I 2 Static → draw | BS 2 hits × 6 |
| Smooth Jazz 98.3 | 55 | Quietly heals. A ratings grind. | HR 6 | S 6 | C 8 | HR 6 | C 8 | S 7 |
| Sports Talk Guys | 45 | Hits hard, then burns out. | BS 2 hits × 8 | CB | BS 2 hits × 8 | CB | BS 2 hits × 9 | CB |
| Oldies Gold 1490 | 50 | Never changes the playlist. A wall of Lock, small Steals. | HR 6 | HR 6 | S 7 | C 8 | HR 6 | S 8 |

**Book 2** (audience 65–80)
| Rival | Aud. | Personality | T1 | T2 | T3 | T4 | T5 | T6 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Playlist Bot | 70 | Copies your genre. **Passive:** after its Lock clears each rival phase, gains Lock equal to your genre streak (timing and streak count **Proposed**, see below). | S 10 | HR 9 | S 10 | S 11 | HR 9 | S 11 |
| Hot Hits 104 | 70 | Rides the trend, then cashes it. | T 2 | S 10 (shown 15) | T 2 | BS 20 (shown 30) | CB | S 11 |
| Late Night Love Lines | 75 | Call-in show. Floods you with calls. | I 2 Complaint Calls → draw | S 9 | I 2 Complaint Calls → draw | C 12 | S 12 | I 3 Complaint Calls → discard |
| College Radio WKRU | 65 | Erratic-looking but fixed. Big swings, long breaks. | CB | BS 2 hits × 11 | I 3 Static → discard | CB | BS 2 hits × 11 | T 2 |

**Book 3** (audience 90–105)
| Rival | Aud. | Personality | T1 | T2 | T3 | T4 | T5 | T6 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| MegaHits 102 | 100 | Corporate hit machine. Trend into a segment. | T 3 | BS 28 (shown 42) | S 14 (shown 21) | T 3 | BS 30 (shown 45) | S 15 (shown 22) |
| Rock Block 101 | 105 | Heavy rotation, few openings. | HR 12 | S 13 | HR 12 | S 14 | HR 12 | BS 2 hits × 13 |
| Farm & Market Report | 95 | Tanky, keeps the town loyal. | C 16 | S 12 | CB | C 16 | S 14 | C 16 |
| Drive at Five | 90 | Rush hour builds, then the road clears. | S 12 | S 14 | S 16 | BS 2 hits × 16 | CB | CB |

**Rival per Daypart node. Decided (brief §6):** the map allows at most 4 regular Daypart nodes on
any path, so a book's 4 rivals never repeat. Prime Time nodes fill any further fight slots, and the
boss is separate.

**Proposed:** a Daypart node's rival is drawn when you enter the node, from that book's rivals not yet
fought this book, on the `rng.rivals` stream (`save.md`). Rivals fought this book are saved. If the
open question on map visibility is answered "visible", the draw moves to map generation instead
(still on `rng.rivals`), with the constraint that no path holds the same rival twice.

**Playlist Bot timing. Proposed:** the Lock is gained in the rival phase right after step 1 (Lock
clears) and before its intent, so it protects through your next player phase like Heavy Rotation.
The streak count is defined in `flow.md` (a single on-genre song = 1).

### Prime Time elites (4, any book, 7 turns)
Audience 90 / 130 / 170. Values use the book's intent table; patterns below use book 1 numbers
(book 2 / 3 values in brackets). The passive is on a placard under its frequency from turn 1.

| Elite | Passive (placard) | T1 | T2 | T3 | T4 | T5 | T6 | T7 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Top 40 Countdown | **Countdown:** each Steal is +2 bigger than the last Steal this daypart, never above the book's Steal cap (9 / 13 / 16). | S 8 [11/14] | HR 6 [9/12] | S +2 → 9 [13/16] | T 2 [2/3] | S +2 → 9 [13/16] | CB | BS 2× next Steal → 18 [26/32] |
| Shock Jock Hour | **Outrage:** each time you play an Ad, the rival gains Buzz 1. | S 8 [11/14] | I 2 Complaint Calls → draw [3/3] | BS 2 hits × 8 [11/14] | CB | S 9 [12/15] | I 2 Complaint Calls → draw [3/3] | BS 2 hits × 9 [12/15] |
| Public Radio Pledge Drive | **Pledge Drive (Proposed):** while below half its starting audience, each Heavy Rotation it reveals becomes a Contest of the book value instead. | HR 6 [9/12] | S 7 [10/13] | C 8 [12/16] | S 8 [11/14] | C 8 [12/16] | HR 6 [9/12] | C 8 [12/16] |
| Traffic on the 8s | **Gridlock:** the first card you play each turn costs 1 more Airtime, never above 3 (**Proposed** cap). You always see its next 2 intents. | S 7 [10/13] | S 8 [11/14] | BS 2 hits × 8 [11/14] | CB | S 8 [11/14] | HR 6 [9/12] | BS 2 hits × 9 [12/15] |

**Countdown cap.** The brief caps Steal at 6–9 / 9–13 / 12–16 per book and Big Segment at 2× Steal.
Countdown's Steals never go above the top of the book's range (`countdown.stealCap` 9 / 13 / 16), and
its Big Segment never above 2× that cap (18 / 26 / 32). The +2 step counts from the last Steal's base
value (before Buzz); Buzz then applies to the shown number as usual. Base values over the fight:
book 1 S 8, 9, 9, BS 18; book 2 S 11, 13, 13, BS 26; book 3 S 14, 16, 16, BS 32. With T4's Trending,
T5's Steal shows 13 / 19 / 24.

**Pledge Drive. Proposed.** The old passive ("each Contest heals double") healed 16 / 24 / 32, past
the brief's Contest values (8 / 12 / 16). Instead, Contests always heal the book value, and they come
more often: while the rival is below half its starting audience, each Heavy Rotation it reveals
becomes a Contest of the book value (in practice T6; T1 is revealed at full audience). The check is
made when the intent is revealed; the shown intent never changes during your turn. Reasoning: the heal stays inside the
brief's range and the threat stays visible one turn ahead (pillar 1).

**Gridlock cap. Proposed.** The brief says card costs are 0 to 3 (§3, §4), so Gridlock never raises a
cost above 3: a 3-cost card played first still costs 3. Letting it cost 4 would conflict with the
brief and needs a brief change (see open question).

## Tunable values
Every audience and intent number in the tables above, plus:
| Key | Start |
| --- | --- |
| `rival.audience.regular` | 45–55 / 65–80 / 90–105 |
| `rival.audience.primeTime` | 90 / 130 / 170 |
| `countdown.stealStep` | 2 |
| `countdown.stealCap` | 9 / 13 / 16 (top of the brief's Steal range, a rule) |
| `gridlock.extraCost` | 1 |
| `gridlock.maxCost` | 3 (brief's card cost cap, a rule) |
| `pledgeDrive.threshold` | 0.5 of starting audience |
| `trafficOn8s.freeMonitor` | 2 |

## Edge cases
- Playlist Bot with no genre streak: gains 0 Lock. The extra Lock is shown on its Lock sticky note.
- Contest at full audience: shown with its number, heals 0.
- Interference into an empty draw pile: the cards become the draw pile.
- Commercial Break with Monitor: the next intent is still shown ghosted.
- Fight ends early (rival at 0): the remaining pattern is never shown again.
- Countdown's first Steal uses the base value; Big Segment on T7 is 2× what the next Steal would be,
  after the cap.
- Countdown at the cap: later Steals stay at the cap (book 1: 8 → 9 → 9); the T7 Big Segment is
  2× the cap (18), never 2× an uncapped value.
- Countdown with Buzz: the +2 step reads the last Steal's base, not its Buzz-boosted shown number.
- Gridlock on a 0-cost card: it costs 1. Gridlock with only 0-cost cards and 0 Airtime is impossible
  (you always start with 3).
- Gridlock on a 3-cost card played first: it stays at 3 (cap, proposed), so it is playable with the
  base 3 Airtime and uses the whole turn's Airtime. On a 2-cost card played first: it costs 3.
- Pledge Drive dropping below half during your turn: the intent already shown doesn't change; the next
  revealed Heavy Rotation becomes a Contest.
- Pledge Drive below half revealing a Steal or Contest: unchanged (only Heavy Rotation converts).
- Pledge Drive's converted Contest heals exactly the book value, however far below half the rival is
  (book 1: at 20 of 90, it heals to 28). It can't reach the starting-audience cap: from below half
  (under 45 / 65 / 85) plus 8 / 12 / 16 it always stays under the start (90 / 130 / 170).
- Monitor peek at a future Heavy Rotation in Pledge Drive's pattern: **Proposed:** the ghosted icon
  shows the conditional, "Heavy Rotation, or Contest if below half", and its tooltip states the rule,
  so a peek never shows something that later changes (pillar 1). Once that intent becomes the current
  one, it shows the resolved intent.
- Pledge Drive back above half when the next intent is revealed: Heavy Rotation stays Heavy Rotation.
- Gridlock and the cue slot: the first card played each turn costs 1 more whether it comes from hand
  or the cue slot.
- Outrage Buzz is gained during your turn, so it lasts through the coming rival phase, then −1.
- A 5th Prime Time in one run with all 4 elites already fought: **Proposed:** the elites-fought list
  resets and the draw continues on `rng.rivals`.

## Open questions (ask before building)
- Which elites can appear in which books, and can the same elite appear twice in a run? Proposed: any book,
  no repeats per run (until all 4 are fought; then the list resets). Elites fought this run are saved.
- Rival drawn at node entry from rivals not fought this book, on `rng.rivals`. Proposed above.
- Is a Daypart or Prime Time node's rival visible on the map before you pick it (pillar 1)? Proposed:
  no, the node shows its type only (the brief shows node types, not rivals); see `map.md`.
- Playlist Bot passive timing (after Lock clears, before its intent). Proposed above.
- Gridlock cap. Proposed: never raises a cost above 3 (brief-compliant). Alternative: a 3-cost card
  played first costs 4, which makes 3-cost cards unplayable all fight at base Airtime; that conflicts
  with brief §3/§4 (costs 0 to 3) and **needs a brief change**.
- Pledge Drive: Contests heal the book value; below half, revealed Heavy Rotations become Contests.
  Proposed above. Alternative: keep "heals double" but cap each heal at the book value (which makes
  the passive do nothing, so not recommended).
- Patterns loop from turn 1 when a fight outlasts them. Proposed above.
- Monitor peeks at Pledge Drive's future Heavy Rotations show the conditional icon ("Heavy Rotation,
  or Contest if below half") with the rule in its tooltip. Proposed above.

## Done checklist
- [ ] Rival data schema + ScriptableObject + exporter, patterns as data
- [ ] All 12 regulars and 4 elites authored with the exact values above
- [ ] Each intent type tested, including multi-hit and Buzz rounding
- [ ] Shown intent number equals the resolved number in every test
- [ ] Each passive tested, including Countdown's cap, Gridlock on 0- and 3-cost cards, and Pledge
      Drive's conversion at reveal
- [ ] No rival repeats within a book; elite no-repeat and reset; draws use only `rng.rivals`
- [ ] Every edge case above has a test
- [ ] Pattern loop and Monitor peeks tested
- [ ] Sim run of every rival against every starter deck logged in `tools/sim`
- [ ] Reviewer sign-off
