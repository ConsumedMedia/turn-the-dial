# Gear

Source: GAME_BRIEF.md sections 5, 6, 8, 9, 11. The brief wins if this doc disagrees.

## Purpose
Gear is the relic system: unlimited passive effects, each a physical object on the desk or shelf of the
booth. Gear changes how a run plays without adding per-card code.

## Rules
- No limit on gear held.
- Each piece is data: id, name, rarity, trigger, conditions, effects, prop model id, tooltip text.
- Triggers (v1): daypart start, turn start, card played (filtered by type/genre/tempo/keyword),
  first X each turn, Flow reaches N, end of turn, daypart won, node entered, Cash gained, shop opened.
- Effects reuse the card effect library (`cards.md`) plus passive modifiers (Airtime, draw, Flow cap,
  conversion rate, prices).
- Multiple gear on one trigger resolve in acquisition order.
- Selecting a gear prop in the booth shows its tooltip.

### Sources
| Source | Count per run |
| --- | --- |
| Starting gear | 1 (per format) |
| Remote Broadcast | 3 |
| Prime Time | 1–2 |
| Boss reward | 2 (pick 1 of 3 boss gear, books 1 and 2) |
| Sponsor Meeting | 0–2 |
| Station Events | some |

Total gear + staff per run: about 8 to 10.

### Rarities
Starter, Common, Uncommon, Rare, Boss. Shop prices 140 / 190 / 260 for Common / Uncommon / Rare.
Boss gear only comes from boss rewards.

### Gear defined so far
| Gear | Rarity | Effect |
| --- | --- | --- |
| Ribbon Mic | Starter (Classic Rock) | Your first DJ Break each turn gives +3 Loyalty. |
| Sampler | Starter (Hip-Hop Heat) | The first time Flow reaches 5 in a daypart, draw 2. |
| Mail Sack | Starter (Country Roads) | After winning a daypart, recover 4 listeners. |

The rest of the v1 pool (about 22 gear in total) is designed in a later session and tracked in
`docs/CONTENT.md`.

## Tunable values
| Key | Start |
| --- | --- |
| `gear.poolSize` | ~22 |
| `gear.shopPrices` | 140 / 190 / 260 |
| `gear.remoteChoices` | 2 |
| `gear.bossChoices` | 3 |
| `gear.primeTimeRewards` | 1 |

## Edge cases
- Duplicate gear: never offered twice in a run.
- Gear that raises the Flow cap while Flow is at the old cap: Flow stays, can now grow.
- Gear that triggers on "first X each turn" with X played from the cue slot: counts.
- Gear effect that would win the fight at daypart start: proposed allowed (win check after each step).
- Pool exhausted: see `nodes.md`.

## Open questions (ask before building)
- Rarity odds for Remote Broadcast and Prime Time gear. Proposed 60/30/10 common/uncommon/rare.

## Done checklist
- [ ] Gear schema + ScriptableObject + exporter, triggers and effects as data
- [ ] The three starting gear tested
- [ ] Trigger order tested with two gear on one trigger
- [ ] No duplicates, pool exhaustion tested
- [ ] Each gear has a prop on the desk/shelf using the shared palette texture
- [ ] Every gear listed in `docs/CONTENT.md`
- [ ] Reviewer sign-off
