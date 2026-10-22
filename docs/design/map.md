# Map (town routes)

Source: GAME_BRIEF.md sections 6, 11, 13. The brief wins if this doc disagrees.

## Purpose
One run = one ratings season = 3 books. Each book is a branching route through an isometric town,
fully visible from the start (pillar 1), ending in a Ratings Book boss. Target run: 35–45 minutes.

## Rules

### Layout per book
- 8 rows: 7 rows of nodes, then the boss.
- 3 lanes. Each row has 2 to 4 nodes (see open question).
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

In row 2 Prime Time is removed and the other weights are renormalized.

### Generator constraints
- Every path has at least one reachable Sponsor Meeting.
- No two Sponsor Meetings in a row on the same path; no two Production Rooms in a row on the same path
  (row 6 → row 7 counts, so row 6 has no Production Room).
- About 4 to 5 fights (Daypart, Prime Time, boss) on any path.
- Generation uses the `map` RNG stream. If constraints fail, reroll the book with the next sub-seed
  from that stream (deterministic), up to a cap; then fall back to a hand-made template.

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
| `map.nodesPerRow` | 2–4 |
| `map.weights` | 45 / 25 / 12 / 10 / 8 |
| `map.fightsPerPath` | 4–5 |
| `map.maxRerolls` | 50 |

## Edge cases
- A row with 2 nodes and a next row with 4: each node needs 2 links, so all 4 are reachable.
- A node with no incoming link is not allowed (every node is reachable).
- A path that would have 3 fights: fails the constraint, reroll.
- Two paths sharing a node: allowed; only line crossings are forbidden.

## Open questions (ask before building)
- **3 lanes but up to 4 nodes per row** conflict. Proposed: "lanes" are the visual columns paths
  drift between, and up to 4 nodes fit across them. Or: cap at 3 nodes per row. Need your call.
- Should the boss row count toward "about 4 to 5 fights"? Proposed: yes.

## Done checklist
- [ ] Map generator in Core, seeded, deterministic
- [ ] Property tests over 10,000 seeds: fixed rows, no crossings, every node reachable,
      Sponsor on every path, no back-to-back Sponsor/Production, 4–5 fights per path
- [ ] Wash per row exposed to the Game layer
- [ ] Town map diorama shows nodes, labels, paths and current position (`ui.md`)
- [ ] Reviewer sign-off
