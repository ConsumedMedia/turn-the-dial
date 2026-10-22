# Meta progression

Source: GAME_BRIEF.md sections 9, 13. The brief wins if this doc disagrees.

## Purpose
Meta progression adds options, not power (pillar 3). Runs earn Ratings Points that unlock content
bundles, formats and markets. A first-time player can win with only the starting content.

## Rules
- **Ratings Points** at the end of every run, win or lose:
  - 10 per daypart won (regular and Prime Time)
  - 25 per boss beaten
  - 5 per book reached (book 1 counts)
- **Thresholds** on lifetime Ratings Points unlock about 15 fixed bundles of 3–5 cards, gear or events
  into the pools. Bundles unlock in a fixed order.
- **Formats:** Classic Rock from the start. Hip-Hop Heat: first time you reach book 2. Country Roads:
  first time you beat a boss.
- **Markets:** beating market N with any format unlocks market N+1 (`markets.md`).
- No permanent stat upgrades, ever.
- The run-end screen shows Ratings Points earned and any unlocks.

## Tunable values
| Key | Start |
| --- | --- |
| `rp.perDaypartWon` | 10 |
| `rp.perBoss` | 25 |
| `rp.perBookReached` | 5 |
| `unlock.bundleCount` | ~15 |
| `unlock.thresholds` | 40, 100, 170, 250, 340, 440, 550, 670, 800, 940, 1090, 1250, 1420, 1600, 1800 *(proposed)* |

## Edge cases
- A run that unlocks several bundles at once: all shown on the run-end screen.
- Abandoning a run: counts as a loss and still awards points (proposed).
- Content in locked bundles never appears in rewards, shops or events.
- Starting content alone must be enough to win (sim check).

## Open questions (ask before building)
- Threshold curve: the numbers above are placeholders. Tune in the sim so everything unlocks after
  roughly 8+ hours of play (pillar 7) without feeling like a grind wall (pillar 3).
- Does abandoning a run award points (proposed yes)?

## Done checklist
- [ ] RP calculation tested for wins, losses, and mid-book losses
- [ ] Threshold unlocks, format unlocks, market unlocks tested
- [ ] Locked content excluded from every pool
- [ ] Profile save stores unlocks (`save.md`)
- [ ] Reviewer sign-off
