# Non-fight nodes: shop, rest, events, treasure

Source: GAME_BRIEF.md sections 6, 8, 9. The brief wins if this doc disagrees.

## Purpose
The familiar roguelike stops (pillar 4) in radio clothes: Sponsor Meeting (shop), Production Room
(rest), Station Event (mystery), Remote Broadcast (treasure). Every cost is exact, every chance shows its odds.

## Rules

### Sponsor Meeting (shop)
- Stock: 5 format cards, 2 shared cards, 3 gear, 1 card-cut service. Sometimes a staff member (see open question).
- One card or gear item per visit is 40% off (sale tag).
- Prices vary ±10% (rounded to the nearest whole Cash).

| Item | Common | Uncommon | Rare |
| --- | --- | --- | --- |
| Card | 45 | 70 | 140 |
| Gear | 140 | 190 | 260 |

- Card cut: 60, plus 20 for each earlier cut this run. Once per visit.
- A Request token rerolls the stock (cards and gear; the sale is re-rolled too). The cut price is unchanged.
- Bought items are replaced by an empty slot.

### Production Room (rest)
Choose one:
- **Recover:** 30% of maximum listeners (20% in Retiree Coast and up), shown as an exact number.
- **Remaster:** upgrade one card that isn't remastered, shown side by side.

### Station Event
- A short text scene in a radio-filtered panel with 2–3 choices.
- Every cost is stated exactly. Any chance outcome shows its odds.
- Outcomes can give or take cards, gear, staff, Cash, Request tokens, listeners, or Interference.
- A Request token rerolls one chance outcome after it's rolled (once per event).
- Choices you can't afford are shown but disabled with the reason.
- Event pool: about 20 in v1, listed in `docs/CONTENT.md`. No event repeats in a run.
- Events are data: text, choices, requirements, outcomes (with weights), using the `events` RNG stream.

### Remote Broadcast (treasure)
- Pick 1 of 2 gear. No skip cost; you may skip (see open question).
- Row 4 of every book is all Remote Broadcasts (3 per run).

## Tunable values
| Key | Start |
| --- | --- |
| `shop.formatCards / sharedCards / gear` | 5 / 2 / 3 |
| `shop.saleDiscount` | 0.40 |
| `shop.priceVariance` | 0.10 |
| `shop.cardPrices` | 45 / 70 / 140 |
| `shop.gearPrices` | 140 / 190 / 260 |
| `shop.cutBase / cutStep` | 60 / 20 |
| `rest.recover` | 0.30 |
| `remote.gearChoices` | 2 |
| `events.count` | ~20 |

## Edge cases
- Not enough Cash for anything: you can still leave; the cut is disabled.
- Card cut with a 1-card deck: not allowed (proposed minimum deck size 5).
- Remaster with every card already remastered: Remaster is disabled; Recover only.
- Recover at max listeners: allowed but shown as +0.
- Recover rounding: proposed round down; shown exact.
- Event that gives a 4th staff member: the staff swap prompt (`staff.md`).
- Gear pool exhausted for a Remote Broadcast: offer what's left; if none, give Cash (proposed 50).

## Open questions (ask before building)
- Staff in Sponsor Meetings ("sometimes"): how often and at what price? Proposed: 25% chance of 1 staff at 120.
- Can you skip a Remote Broadcast pick? Proposed: yes.
- Minimum deck size for cuts (proposed 5).
- Recover rounding (proposed down).

## Done checklist
- [ ] Shop generation, prices, sale, variance, cut cost, reroll tested with seeds
- [ ] Production Room both options tested, market modifier applied
- [ ] Event data schema, odds display, affordability, token reroll tested
- [ ] Remote Broadcast pick tested
- [ ] All screens built per `ui.md` and controller-complete
- [ ] Reviewer sign-off
