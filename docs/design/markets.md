# Markets (difficulty ladder)

Source: GAME_BRIEF.md sections 3, 6, 7, 9. The brief wins if this doc disagrees.

## Purpose
Markets are the ascension ladder. Each tier keeps every rule before it and adds one. Each market also
dresses the town diorama differently.

## Rules
| Tier | Market | Added rule |
| --- | --- | --- |
| 1 | Chill Coastal Village | Base rules. The first run has a guided first fight. |
| 2 | College Town | Rival audience +10%. |
| 3 | Blue-Collar Heartland | Dead Air starts at 8 (8 / 13 / 18). |
| 4 | Retiree Coast | Production Room recovers 20% instead of 30%. |
| 5 | True Crime Suburbia | Rivals start each daypart with Buzz 1. |
| 6 | Big Metro | Bosses gain a second telegraphed rule (`bosses.md`). |

- Rules are cumulative: tier N applies tiers 1..N.
- Beating market N with any format unlocks market N+1.
- Market modifiers apply at daypart start step 2 (`combat.md`).
- "Rival audience" for College Town includes Prime Time and bosses, and changes the starting value
  used for the ratings check.
- The guided first fight: only on the profile's first run, in Chill Coastal Village (see `ui.md`).

## Tunable values
| Key | Start |
| --- | --- |
| `market.collegeTown.audienceBonus` | 0.10 |
| `market.heartland.deadAir` | 8 / 13 / 18 |
| `market.retiree.recover` | 0.20 |
| `market.trueCrime.startBuzz` | 1 |

## Edge cases
- College Town rounding: proposed round up (55 → 61).
- True Crime Buzz 1: affects the turn-1 intent's shown number, then decays to 0.
- True Crime + Trending on turn 1: Buzz 1 + N, then −1.
- Selecting a locked market: not possible; it shows the unlock condition.

## Open questions (ask before building)
- College Town rounding (proposed up).

## Done checklist
- [ ] Market data assets, cumulative rule stacking tested for every tier
- [ ] Each modifier tested in isolation and at tier 6
- [ ] Unlock chain tested (`meta.md`)
- [ ] New-run screen shows each market's full rule list
- [ ] Each market has its town dressing variant
- [ ] Reviewer sign-off
