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
- Multiple gear on one trigger resolve in acquisition order. **Proposed.**
- On any trigger shared by gear and staff (daypart start, turn start, card played, first X each turn,
  Flow reaches N, end of turn, daypart won, and so on), all gear resolves first in acquisition order,
  then staff in slot order, left to right. **Proposed** (same rule in `combat.md` and `staff.md`).
- Gear offers roll on `rng.rewards` (Remote Broadcast, Prime Time, boss), `rng.shop` (Sponsor
  Meeting) or `rng.events` (Station Events). See `save.md`, RNG streams. **Proposed.**
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
Starter, Common, Uncommon, Rare, Boss. Shop prices 140 / 190 / 260 for Common / Uncommon / Rare
(`shop.gearPrices`, owned by `nodes.md`).
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
| `shop.gearPrices` | 140 / 190 / 260 (defined in `nodes.md`) |
| `remote.gearChoices` | 2 (defined in `nodes.md`) |
| `gear.bossChoices` | 3 |
| `gear.primeTimeRewards` | 1 |
| `gear.rarityOdds` | 60 / 30 / 10 common/uncommon/rare *(proposed)*, Remote Broadcast and Prime Time offers |

## Edge cases
- Duplicate gear: never offered twice in a run. **Proposed.**
- Gear that raises the Flow cap while Flow is at the old cap: Flow stays, can now grow.
- Gear that triggers on "first X each turn" with X played from the cue slot: counts. **Proposed.**
- Gear effect that would win the fight at daypart start: proposed allowed (win check after each step).
- Pool exhausted: see `nodes.md`.

## Open questions (ask before building)
- Rarity odds for Remote Broadcast and Prime Time gear. Proposed 60/30/10 common/uncommon/rare.
- A gear piece is never offered twice in a run. Proposed above.
- A card played from the cue slot counts for "first X each turn" triggers. Proposed above.
- Multiple gear on one trigger resolve in acquisition order. Proposed above.
- On every trigger shared by gear and staff, gear resolves before staff. Proposed above.
- Gear offer streams (`rng.rewards` / `rng.shop` / `rng.events`). Proposed above.
- A gear effect that wins the fight at daypart start. Proposed: allowed.

## Done checklist
- [ ] Gear schema + ScriptableObject + exporter, triggers and effects as data
- [ ] The three starting gear tested
- [ ] Trigger order tested with two gear on one trigger, and with gear and staff on one trigger
- [ ] No duplicates, pool exhaustion tested
- [ ] Each gear has a prop on the desk/shelf using the shared palette texture
- [ ] Every gear listed in `docs/CONTENT.md`
- [ ] Reviewer sign-off
