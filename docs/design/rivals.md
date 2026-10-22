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
  the pattern, it loops from turn 1.
- Monitor reveals the next intents as smaller ghosted icons after the current one.
- Market modifiers (College Town +10% audience, True Crime Suburbia Buzz 1) apply on top.

### Regular rivals (12, 4 per book, 6 turns)
**Decided:** Playlist Bot is in book 2. Oldies Gold 1490 moved to book 1 in its place: it is the
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
| Playlist Bot | 70 | Copies your genre. **Passive:** after its Lock clears each rival phase, gains Lock equal to your genre streak. | S 10 | HR 9 | S 10 | S 11 | HR 9 | S 11 |
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

Each book draws its 4–5 regular fights from its 4 rivals; no rival repeats until all 4 have been
fought that book.

### Prime Time elites (4, any book, 7 turns)
Audience 90 / 130 / 170. Values use the book's intent table; patterns below use book 1 numbers
(book 2 / 3 values in brackets). The passive is on a placard under its frequency from turn 1.

| Elite | Passive (placard) | T1 | T2 | T3 | T4 | T5 | T6 | T7 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Top 40 Countdown | **Countdown:** each Steal is +2 bigger than the last Steal this daypart. | S 8 [11/14] | HR 6 [9/12] | S +2 | T 2 [2/3] | S +2 | CB | BS 2× next Steal |
| Shock Jock Hour | **Outrage:** each time you play an Ad, the rival gains Buzz 1. | S 8 [11/14] | I 2 Complaint Calls → draw [3/3] | BS 2 hits × 8 [11/14] | CB | S 9 [12/15] | I 2 Complaint Calls → draw [3/3] | BS 2 hits × 9 [12/15] |
| Public Radio Pledge Drive | **Pledge Drive:** while below half its starting audience, each Contest heals double. | HR 6 [9/12] | S 7 [10/13] | C 8 [12/16] | S 8 [11/14] | C 8 [12/16] | HR 6 [9/12] | C 8 [12/16] |
| Traffic on the 8s | **Gridlock:** the first card you play each turn costs 1 more Airtime. You always see its next 2 intents. | S 7 [10/13] | S 8 [11/14] | BS 2 hits × 8 [11/14] | CB | S 8 [11/14] | HR 6 [9/12] | BS 2 hits × 9 [12/15] |

## Tunable values
Every audience and intent number in the tables above, plus:
| Key | Start |
| --- | --- |
| `rival.audience.regular` | 45–55 / 65–80 / 90–105 |
| `rival.audience.primeTime` | 90 / 130 / 170 |
| `countdown.stealStep` | 2 |
| `gridlock.extraCost` | 1 |
| `trafficOn8s.freeMonitor` | 2 |

## Edge cases
- Playlist Bot with no genre streak: gains 0 Lock. The extra Lock is shown on its Lock sticky note.
- Contest at full audience: shown with its number, heals 0.
- Interference into an empty draw pile: the cards become the draw pile.
- Commercial Break with Monitor: the next intent is still shown ghosted.
- Fight ends early (rival at 0): the remaining pattern is never shown again.
- Countdown's first Steal uses the base value; Big Segment on T7 is 2× what the next Steal would be.
- Gridlock on a 0-cost card: it costs 1. Gridlock with only 0-cost cards and 0 Airtime is impossible
  (you always start with 3).
- Outrage Buzz is gained during your turn, so it lasts through the coming rival phase, then −1.

## Open questions (ask before building)
- Which elites can appear in which books, and can the same elite appear twice in a run? Proposed: any book,
  no repeats per run.

## Done checklist
- [ ] Rival data schema + ScriptableObject + exporter, patterns as data
- [ ] All 12 regulars and 4 elites authored with the exact values above
- [ ] Each intent type tested, including multi-hit and Buzz rounding
- [ ] Shown intent number equals the resolved number in every test
- [ ] Each passive tested
- [ ] Pattern loop and Monitor peeks tested
- [ ] Sim run of every rival against every starter deck logged in `tools/sim`
- [ ] Reviewer sign-off
