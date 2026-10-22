# Cards

Source: GAME_BRIEF.md sections 2, 4, 5, 6, 9. The brief wins if this doc disagrees.

## Purpose
Your deck is the broadcast. Cards are pure data (pillar 7): fields, keywords and an ordered effect
list interpreted by Core. No per-card code. The full card list lives in `docs/CONTENT.md`.

## Rules

### Fields
| Field | Rule |
| --- | --- |
| `id` | Stable string, e.g. `card.cr.album_cut`. Never renamed once shipped. |
| `name` | Display name. Burnout matches on name. |
| `cost` | Airtime 0–3. |
| `type` | Song, DJ Break, Ad, Promo, Liner, Interference. |
| `genre` | Songs only: Rock, Hip-Hop, Country, Pop. Others: none. |
| `tempo` | Songs only: 1–5 (five pips on the label). |
| `rarity` | Starter, Common, Uncommon, Rare, Special. |
| `pool` | Classic Rock, Hip-Hop Heat, Country Roads, Shared. |
| `reach`, `loyalty`, `hype`, `cash`, `listenerCost` | Base numbers, any can be 0. |
| `keywords` | From the list below. |
| `effects` | Ordered list of data effects. |
| `upgrade` | Full remastered card (same fields). One remaster per card; gold RM stamp. |
| `text`, `flavor`, `artId` | Rules text generated from data where possible. |

### Type defaults
| Type | On Air | Notes |
| --- | --- | --- |
| Song | Yes | Builds Flow. Main Reach source. Sets "last song". |
| DJ Break | Yes | Setup, Loyalty, Drops. |
| Ad | No | Gives Cash, costs listeners or Loyalty. Breaks Flow, clears last song. |
| Promo | No | Builds Hype. |
| Liner | No | Costs 0–1. Doesn't change last song. |
| Interference | No | Rival junk. |

On Air comes from the keyword; the type sets the default value.

### Keywords (v1)
| Keyword | Rule |
| --- | --- |
| On Air | Counts against Dead Air. |
| Drop N | See `flow.md`. |
| Anthem | Classic Rock signature (`formats.md`). |
| Hold | Stays in hand at end of turn. |
| Monitor N | See the rival's next N intents this daypart, or the top N cards of your draw pile, as stated. |
| Pulled | Removed for the rest of the daypart once played (back in the deck next daypart). |
| Sticky | Interference that stays after the daypart. |
| Unplayable | Can't be played. |

Every keyword has a tooltip.

### Effect order on play
1. Pay Airtime. 2. Move the card to "resolving". 3. Base numbers in this order: Reach, Loyalty,
Hype, Cash, listenerCost. 4. `effects` list in order. 5. Flow update (Songs). 6. Card goes to discard
(or removed if Pulled / Complaint Call). 7. Win/loss check.

### Effect library (v1 minimum)
`DrawCards(n)`, `GainFlow(n)`, `GainHype(n)`, `GainLoyalty(n)`, `GainCash(n)`, `LoseListeners(n)`,
`MonitorIntents(n)`, `MonitorDraw(n)`, `NextSongTempoBonus(n)`, `Drop(n)`, `RaiseFlowCap(n)`,
`AddCardToPile(cardId, pile, n)`. New effects are added to the library, never one-off per card.

### Starting decks
10 cards each (4 songs / 3 DJ breaks / 1 liner / 1 ad / 1 signature). Exact lists in `formats.md`.

### Rewards and remaster
- Card reward: pick 1 of 3 or skip. Reroll costs 1 Request token.
- Rarity odds (common/uncommon/rare): Daypart 60/35/5, Prime Time 45/45/10 with at least 1 uncommon,
  boss 0/0/100.
- Offers are drawn from your format's pool plus Shared, no duplicates within one offer.
- Remaster at a Production Room (or by effects). Each card once.

## Tunable values
| Key | Start |
| --- | --- |
| `reward.rarity.daypart` | 60/35/5 |
| `reward.rarity.primeTime` | 45/45/10 |
| `reward.rarity.boss` | 0/0/100 |
| `reward.choices` | 3 |
| `pool.formatShare` | 0.75 *(proposed)* |
| `content.cardsPerFormat` | ~30 |
| `content.sharedCards` | ~30 |

## Edge cases
- A card with cost above current Airtime can't be played (UI shows it dimmed).
- Playing from the cue slot empties the cue slot.
- Hold card with Unplayable (Static): stays in hand — avoid in content.
- Monitor N beyond the remaining turns shows only the remaining intents.
- Monitor on draw pile with fewer than N cards shows all of them; no reshuffle.
- Remastering a card whose upgrade is missing: not allowed (data validation error).
- Rare offer with no rares left in the pool: fall back to uncommon, then common.

## Open questions (ask before building)
- Format pool vs Shared ratio for offers (proposed 75/25).
- Pop genre: which format's streak does a Pop song count for? Proposed: none in v1 (shared filler).

## Done checklist
- [ ] Card data schema in Core + ScriptableObject + exporter
- [ ] Validator: every card has an id, cost 0–3, tempo 1–5 for Songs, an upgrade unless Interference
- [ ] Every keyword and effect tested
- [ ] Effect order on play tested
- [ ] Reward odds, rerolls and fallbacks tested with a seeded RNG
- [ ] Rules text generated from data for every starter card
- [ ] Every card listed in `docs/CONTENT.md`
- [ ] Reviewer sign-off
