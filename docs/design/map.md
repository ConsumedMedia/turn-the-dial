# Map (town routes)

Source: GAME_BRIEF.md sections 6, 11, 13. The brief wins if this doc disagrees.

## Purpose
One run = one ratings season = 3 books. Each book is a branching route through an isometric town,
fully visible from the start (pillar 1), ending in a Ratings Book boss. Target run: 35–45 minutes.

## Rules

### Layout per book
- 8 rows: 7 rows of nodes, then the boss.
- 3 lanes. Each row has 2 to 3 nodes. **Decided** (brief §6; LESSONS map-rows line).
- Every node connects to 1 or 2 nodes in the next row. Paths may cross lanes but never cross each other.
- Every node in row 7 connects to the boss.
- The whole book and every node type are visible from the start.

### Fixed rows
| Row | Nodes |
| --- | --- |
| 1 | All Daypart |
| 4 | All Remote Broadcast |
| 7 | All Production Room |
| 8 | Ratings Book boss |

### Weighted rows (2, 3, 5, 6) *(tune)*
| Node | Weight |
| --- | --- |
| Daypart | 45% |
| Station Event | 25% |
| Sponsor Meeting | 12% |
| Prime Time | 10% (rows 3 and up only) |
| Production Room | 8% |

In row 2 Prime Time is removed and the other weights are renormalized. **Proposed.**

### Generator constraints
- Every path has at least one reachable Sponsor Meeting.
- No two Sponsor Meetings in a row on the same path; no two Production Rooms in a row on the same path
  (row 6 → row 7 counts, so row 6 has no Production Room).
- **At most 4 regular Daypart nodes on any path. Decided (brief §6).** A book has 4 rivals, so they
  never repeat on a path (`rivals.md`). Prime Time fills any extra fight slot: a path can hold at most
  5 non-boss fights (row 1 plus rows 2, 3, 5, 6), and if it holds 5, at least one is Prime Time.
- **Fights per path, not counting the boss. Decided (brief §6):** the boss is counted separately;
  the target is about 4 to 5 non-boss fights per path.
- **Hard check. Proposed:** every path has 3 to 5 non-boss fights; 4 to 5 is the target the weights
  are tuned toward, not a rejection rule. Reasoning: a path has only 5 rows that can hold a fight and
  must also fit a Sponsor Meeting, so a hard minimum of 4 would leave almost no room for Station Events.
- Generation uses the `rng.map` stream (`save.md`, RNG streams). If constraints fail, reroll the book
  with the next sub-seed from that stream (deterministic), up to `map.maxRerolls` attempts; then fall
  back to a hand-made template that meets every constraint. **Proposed** (the reroll cap of 50 and the
  template fallback).
- Rivals are not placed by the generator; each fight node's rival is drawn on `rng.rivals`
  (`rivals.md`). **Proposed**, tied to the map-visibility open question below.

### Daypart wash by row
| Rows | Wash |
| --- | --- |
| 1–2 | Morning drive |
| 3–4 | Midday |
| 5–6 | Afternoon drive |
| 7–8 | Overnight |

### Movement
- Start below row 1, pick any row-1 node. After each node, pick a connected node in the next row.
- Save and quit at any node; resume at that node (`save.md`).

## Tunable values
| Key | Start |
| --- | --- |
| `map.rows` | 8 |
| `map.lanes` | 3 |
| `map.nodesPerRow` | 2–3 |
| `map.weights` | 45 / 25 / 12 / 10 / 8 |
| `map.maxDaypartsPerPath` | 4 (rule: rivals per book, brief §6) |
| `map.fightsPerPathTarget` | 4–5, boss not counted |
| `map.fightsPerPathMin / Max` | 3 / 5 *(proposed hard check)* |
| `map.maxRerolls` | 50 *(proposed)* |

## Edge cases
- A row with 2 nodes and a next row with 3: at least one node needs 2 links so all 3 are reachable.
- A row with 3 nodes and a next row with 2: links converge; no node is left without an outgoing link.
- A node with no incoming link is not allowed (every node is reachable).
- A path with Daypart in rows 1, 2, 3, 5 and 6 (5 Dayparts): fails, reroll.
- A path with 5 fights, one of them Prime Time: passes.
- A path with 4 Dayparts plus 1 Prime Time: passes (5 non-boss fights).
- A path with 2 non-boss fights: fails the proposed hard check, reroll.
- A path with 3 non-boss fights: passes the hard check (below the 4–5 target).
- Two paths sharing a node: allowed; only line crossings are forbidden.
- All 50 rerolls fail: the hand-made template is used, and it passes every constraint test (proposed).

## Open questions (ask before building)
- Hard check of 3 to 5 non-boss fights per path, with 4 to 5 as the target. Proposed above.
- Reroll cap (50) and the hand-made template fallback when every reroll fails (one template per book,
  checked by the same constraint tests). Proposed above.
- Is a fight node's rival visible on the map before you pick it (pillar 1: "map nodes are labeled")?
  Proposed: no; the node shows its type (Daypart or Prime Time) and the rival is drawn when you enter.
  If yes, rivals are assigned at map generation on `rng.rivals` with no repeat on any path.
- Row 2 renormalizes the weights without Prime Time. Proposed above.

## Done checklist
- [ ] Map generator in Core, seeded, deterministic
- [ ] Property tests over 10,000 seeds: fixed rows, no crossings, every node reachable,
      Sponsor on every path, no back-to-back Sponsor/Production, at most 4 Dayparts per path,
      3–5 non-boss fights per path (boss not counted); report the share of paths with 4–5
- [ ] Every edge case above has a test
- [ ] Wash per row exposed to the Game layer
- [ ] Town map diorama shows nodes, labels, paths and current position (`ui.md`)
- [ ] Reviewer sign-off
