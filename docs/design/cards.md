# Cards

Source: GAME_BRIEF.md sections 2, 3, 4, 5, 6, 9. The brief wins if this doc disagrees.

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
| `pool` | Classic Rock, Hip-Hop Heat, Country Roads, Shared, Rival (Interference cards). |
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
| Drop N | See `flow.md`. One hit, no Flow multiplier, Flow 0 afterwards. |
| Anthem | Classic Rock signature (`formats.md`). |
| Hold | Stays in hand at end of turn. |
| Monitor N | See the rival's next N intents this daypart, or the top N cards of your draw pile, as stated. |
| Pulled | Removed for the rest of the daypart once played (back in the deck next daypart). |
| Sticky | Interference that stays after the daypart. |
| Unplayable | Can't be played. |

Every keyword has a tooltip.

### Effect order on play
1. Pay Airtime (after cost modifiers such as Gridlock, `rivals.md`).
2. Move the card to "resolving".
3. **Reach.** Compute the card's Reach as one number: the Song or non-song formula in `combat.md`,
   or, for a card with Drop, the Drop formula in `flow.md` (base + Hype + N × Flow, no Flow
   multiplier). Flow here is the value before this card's own Flow update.
4. **Apply that Reach as one hit** (`combat.md`, Applying Reach): Lock absorbs once, one floor, one
   listener conversion. A Drop card's base Reach and its Drop part are never two hits.
5. Other base numbers in this order: Loyalty, Hype, Cash, listenerCost. **Proposed.**
6. `effects` list in order. **Proposed** (after the card's Reach).
7. Songs: Flow update (ramp / streak, `flow.md`). An Ad instead sets Flow to 0 and clears last song.
8. Drop cards: set Flow to 0 (this wipes a Drop Song's own update from step 7).
9. Card goes to discard (or is removed if Pulled / Complaint Call).
10. Win/loss check.

Steps 3, 4, 7 and 8 follow the brief (§2, §3: Reach before the song's Flow update; the Drop resolves
last; one hit). The placement of steps 5 and 6 after the Reach is **Proposed**: a card's own Hype or
effects never boost its own Reach, matching "a song never benefits from the Flow it earns itself".

### Effect library (v1 minimum)
`DrawCards(n)`, `GainFlow(n)`, `GainHype(n)`, `GainLoyalty(n)`, `GainCash(n)`, `LoseListeners(n)`,
`MonitorIntents(n)`, `MonitorDraw(n)`, `NextSongTempoBonus(n)`, `RaiseFlowCap(n)`,
`AddCardToPile(cardId, pile, n)`. New effects are added to the library, never one-off per card.

**Drop is a keyword, not an entry in `effects`.** Its N × Flow is part of the card's single Reach
(step 3) and its Flow reset is step 8, so it can never resolve as a separate hit.

### Starting decks
10 cards each (4 songs / 3 DJ breaks / 1 liner / 1 ad / 1 signature). Exact lists in `formats.md`.

### Rewards and remaster
- Card reward: pick 1 of 3 or skip. Reroll costs 1 Request token. Reward Cash is in `nodes.md`.
- Rarity odds (common/uncommon/rare): Daypart 60/35/5, Prime Time 45/45/10 with at least 1 uncommon,
  boss 0/0/100.
- Offers are drawn from your format's pool plus Shared, on the `rng.rewards` stream (`save.md`). The
  Rival pool (Interference) is never offered.
- No duplicates within one offer. **Proposed.**
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
- Rare offer with no rares left in the pool: fall back to uncommon, then common. **Proposed.**
- Drop card into Lock: Lock absorbs the single combined number once (e.g. Hype Man, 3 Reach + Drop 2
  at Flow 4 = 11 into Lock 6: 5 gets through, 0 listeners at 10%).
- Drop card listener conversion: one rounding on the combined post-Lock number, not one per part.
- Drop Song (Banger, T5 Hip-Hop, after Rooftop Jam, T4 Hip-Hop, at Flow 2, Hype 0): Reach uses
  Flow 2, so 7 + 3 × 2 = 13; then the song's ramp (+1) and genre streak (+1) make Flow 4; then the
  Drop sets Flow 0.
- A card whose own base Hype would apply to a Song: it never boosts that card's own Reach (step 5
  after step 3).

## Open questions (ask before building)
- Format pool vs Shared ratio for offers (proposed 75/25).
- Pop genre: which format's streak does a Pop song count for? Proposed: none in v1 (shared filler).
- Effect order steps 5–6 (other base numbers and the effects list resolve after the card's Reach).
  Proposed above.
- No duplicates within one card offer. Proposed: yes.
- Rarity fallback when a rarity is empty (rare → uncommon → common). Proposed above.

## Done checklist
- [ ] Card data schema in Core + ScriptableObject + exporter
- [ ] Validator: every card has an id, cost 0–3, tempo 1–5 for Songs, an upgrade unless Interference,
      Interference cards in the Rival pool, no `Drop` entry in any `effects` list
- [ ] Every keyword and effect tested
- [ ] Effect order on play tested, including a Drop card resolving as one hit and Flow 0 afterwards
- [ ] Reward odds, rerolls and fallbacks tested with a seeded `rng.rewards`
- [ ] Rules text generated from data for every starter card
- [ ] Every card listed in `docs/CONTENT.md`
- [ ] Every edge case above has a test
- [ ] Reviewer sign-off
