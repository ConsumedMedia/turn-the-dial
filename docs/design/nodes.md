# Non-fight nodes: shop, rest, events, treasure

Source: GAME_BRIEF.md sections 6, 8, 9. The brief wins if this doc disagrees.

## Purpose
The familiar roguelike stops (pillar 4) in radio clothes: Sponsor Meeting (shop), Production Room
(rest), Station Event (mystery), Remote Broadcast (treasure). Every cost is exact, every chance shows its odds.
This doc also owns the Cash paid for winning fights (the card pick is in `cards.md`).

## Rules

### Fight rewards (Cash)
Won fights pay Cash before the card pick (brief §6). A ratings loss pays nothing (`combat.md`).

| Fight | Reward Cash *(tune)* | Fields |
| --- | --- | --- |
| Daypart | 8 to 14 | `reward.cash.daypart.min` 8, `reward.cash.daypart.max` 14 |
| Prime Time | 20 to 28 | `reward.cash.primeTime.min` 20, `reward.cash.primeTime.max` 28 |
| Boss (books 1 and 2) | 50 | `reward.cash.boss` 50 |

- The Daypart and Prime Time amounts are rolled on the `rng.rewards` stream (`save.md`).
- The roll is a uniform whole number, min and max included. **Proposed.**
- Winning book 3 wins the run; there is no reward screen.

### Sponsor Meeting (shop)
- Stock: 5 format cards, 2 shared cards, 3 gear, 1 card-cut service. Sometimes a staff member (see open question).
- Stock, prices and the sale tag roll on the `rng.shop` stream (`save.md`).
- One card or gear item per visit is 40% off (sale tag).
- Prices vary ±10%, rounded to the nearest whole Cash. **Proposed** (the brief gives ±10%, not the
  rounding).

| Item | Common | Uncommon | Rare |
| --- | --- | --- | --- |
| Card | 45 | 70 | 140 |
| Gear | 140 | 190 | 260 |

- Card cut: 60, plus 20 for each earlier cut this run. Once per visit. **Proposed** (once per visit).
- A Request token rerolls the stock (cards and gear). The sale is re-rolled too and the cut price is
  unchanged. **Proposed** (sale and cut handling).
- Bought items are replaced by an empty slot.

### Production Room (rest)
Choose one:
- **Recover:** 30% of maximum listeners (20% in Retiree Coast and up), shown as an exact number.
- **Remaster:** upgrade one card that isn't remastered, shown side by side.

### Station Event
- A short text scene in a radio-filtered panel with 2–3 choices.
- Every cost is stated exactly. Any chance outcome shows its odds.
- Outcomes can give or take cards, gear, staff, Cash, Request tokens, listeners, or Interference.
- A Request token rerolls one chance outcome after it's rolled. Once per event. **Proposed.**
- Choices you can't afford are shown but disabled with the reason.
- Event pool: about 20 in v1, listed in `docs/CONTENT.md`. No event repeats in a run. **Proposed.**
- Events are data: text, choices, requirements, outcomes (with weights), using the `rng.events` stream.
  Which event a node shows, and any gear, staff or cards an event offers, also roll on `rng.events`.
  **Proposed** (stream for event offers).

### Remote Broadcast (treasure)
- Pick 1 of 2 gear. You may skip at no cost. **Proposed** (see open question).
- The 2 gear roll on the `rng.rewards` stream. **Proposed** (`save.md`).
- Row 4 of every book is all Remote Broadcasts (3 per run).

## Tunable values
| Key | Start |
| --- | --- |
| `reward.cash.daypart.min / max` | 8 / 14 |
| `reward.cash.primeTime.min / max` | 20 / 28 |
| `reward.cash.boss` | 50 |
| `shop.formatCards / sharedCards / gear` | 5 / 2 / 3 |
| `shop.saleDiscount` | 0.40 |
| `shop.priceVariance` | 0.10 |
| `shop.cardPrices` | 45 / 70 / 140 |
| `shop.gearPrices` | 140 / 190 / 260 |
| `shop.cutBase / cutStep` | 60 / 20 |
| `shop.minDeckSize` | 5 *(proposed)*: no cut below this deck size |
| `shop.staffChance` | 0.25 *(proposed)* |
| `shop.staffPrice` | 120 *(proposed)* |
| `rest.recover` | 0.30 |
| `remote.gearChoices` | 2 |
| `remote.exhaustedCash` | 50 *(proposed)* |
| `events.count` | ~20 |

This doc owns the shop prices (cards and gear) and the Remote Broadcast choice count; `gear.md`
points here.

## Edge cases
- Not enough Cash for anything: you can still leave; the cut is disabled.
- Card cut with a 1-card deck: not allowed (proposed minimum deck size 5).
- Remaster with every card already remastered: Remaster is disabled; Recover only.
- Recover at max listeners: allowed but shown as +0.
- Recover rounding: proposed round down; shown exact.
- Event that gives a 4th staff member: the staff swap prompt (`staff.md`).
- Gear pool exhausted for a Remote Broadcast: offer what's left; if none, give
  `remote.exhaustedCash` 50 Cash instead. **Proposed.**
- Reward Cash roll at the bounds: 8 and 14 (Daypart), 20 and 28 (Prime Time) can all be rolled.
- Ratings loss in a Daypart or Prime Time: no reward Cash, no `rng.rewards` roll.
- Rerolling a card reward with a token doesn't reroll the Cash already gained.
- Second cut in one visit: disabled (proposed once per visit).

## Open questions (ask before building)
- Reward Cash roll: uniform whole number, bounds included. Proposed above.
- Shop price rounding to the nearest whole Cash. Proposed above.
- One card cut per visit. Proposed above.
- A stock reroll re-rolls the sale and keeps the cut price. Proposed above.
- One token reroll per event; no event repeats in a run. Proposed above.
- Remote Broadcast gear on `rng.rewards`; event offers on `rng.events`. Proposed above.
- Staff in Sponsor Meetings ("sometimes"): how often and at what price? Proposed: 25% chance of 1 staff at 120.
- Can you skip a Remote Broadcast pick? Proposed: yes, at no cost.
- Remote Broadcast with the gear pool exhausted gives 50 Cash. Proposed above.
- Minimum deck size for cuts (proposed 5).
- Recover rounding (proposed down).

## Done checklist
- [ ] Reward Cash ranges and boss 50 tested with a seeded `rng.rewards`; no Cash on a ratings loss
- [ ] Shop generation, prices, sale, variance, cut cost, reroll tested with a seeded `rng.shop`
- [ ] Every edge case above has a test
- [ ] Production Room both options tested, market modifier applied
- [ ] Event data schema, odds display, affordability, token reroll tested
- [ ] Remote Broadcast pick tested
- [ ] All screens built per `ui.md` and controller-complete
- [ ] Reviewer sign-off
