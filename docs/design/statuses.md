# Statuses and resources

Source: GAME_BRIEF.md section 3. The brief wins if this doc disagrees.

## Purpose
Every number on the board besides cards: what it does, who owns it, when it changes. Flow, Drop and
Hype have their own doc (`flow.md`).

## Rules

| Status | Owner | Gain | Clear / decay | Effect |
| --- | --- | --- | --- | --- |
| Listeners | You | Reach conversion, Production Room, effects | Steals, Dead Air, Ads, effects | Health for the run; 0 ends the run. Never above maximum. |
| Max listeners | You | Format start, effects | Effects only | Ratings-loss penalty never changes it. |
| Rival audience | Rival | Contest (not above start) | Your Reach past Lock | Rival health; 0 = you win. |
| Airtime | You | Set to 3 (+mods) at turn start | Lost at end of turn | Pays card costs 0–3. |
| Cash | You | Starts 50; Ads, rewards, events | Shop, events | Kept for the run. Never negative (**Proposed**). |
| Loyalty | You | Cards, gear, staff | Clears to 0 at your turn start | Absorbs rival steals first, and nothing else. Doesn't block Dead Air, Ad costs, or other listener loss. |
| Lock | Rival | Heavy Rotation, Playlist Bot | Clears at start of rival phase | Absorbs your Reach first, once per card (a Drop card is one hit). |
| Buzz | Rival | Trending, True Crime Suburbia, Outrage (Shock Jock Hour, `rivals.md`), Network Promo (Big Metro, `bosses.md`) | −1 at end of rival phase | Steals take 50% more (rounded down). Included in shown number. |
| Burnout | Song (by name) | +1 per repeat play this daypart | Resets after the daypart | −25% Reach per stack, floor 25%. |
| Exposed | Rival | Shown intent is Commercial Break | When the intent changes | +50% to all your Reach that turn. |
| Static | Your deck | Interference cards added | Count updates live | Number of Interference cards in your piles. |
| Dead Air | You | End turn with no On Air card | Count resets per daypart | −5/−10/−15 listeners, ignores Loyalty, Flow 0. |
| Request tokens | You | Start 2; ~3–5 found per run | Spent | Reroll a card reward, shop stock, or one event outcome. |
| Ratings Points | Profile | End of run | Spent never (thresholds) | Meta unlocks (`meta.md`). |

### Buzz timing
Buzz drops by 1 at the end of each rival phase. So Trending 2 gained in a rival phase leaves Buzz 1
for the next rival phase's intent; Buzz 1 from True Crime Suburbia affects turn 1's intent only.
Steal with Buzz: `floor(X × 1.5)`. For multi-hit Big Segments, apply per hit.

### Burnout
- The first play of a name in a daypart has 0 stacks. Each later play of that name adds 1 before
  resolving: 2nd play 0.75, 3rd 0.5, 4th+ 0.25.
- Remastered versions share the name and the count.

### Interference cards (v1)
| Card | Rule |
| --- | --- |
| Static | Unplayable. |
| Complaint Call | Costs 1 Airtime; playing it removes it for the rest of the daypart. In hand at end of turn: lose 3 listeners. |

- Interference leaves your deck when the daypart ends, except Sticky cards.
- Interference is not On Air.
- Interference cards are in the `Rival` pool (brief §4) and are never offered as rewards or in shops.
- Interference added to the draw pile is shuffled in at a random position on `rng.shuffle`
  (`save.md`). **Proposed.** Added to the discard pile: no roll.

### Request tokens
- One token = one reroll of one thing: a card reward's 3 choices, a Sponsor Meeting's stock, or one
  Station Event outcome roll.

## Tunable values
| Key | Start |
| --- | --- |
| `cash.start` | 50 |
| `requestTokens.start` | 2 |
| `buzz.stealBonus` | 0.5 |
| `burnout.perStack` | 0.25 |
| `burnout.floor` | 0.25 |
| `complaintCall.listenerLoss` | 3 |
| `complaintCall.cost` | 1 |

## Edge cases
- Complaint Call in hand and Dead Air on the same turn: both apply (Dead Air first, per end-turn order).
- Complaint Call cued: it isn't in hand, so it doesn't trigger. **Decided** (user, 2026-10-23): its trigger is "end of turn,
  while in hand" (brief §4 `triggers`).
- Complaint Call held by Hold effects: still triggers each end of turn while in hand.
- Buzz 0: no bonus, no decrement below 0.
- Contest when audience is at start: no gain.
- Cash cost greater than Cash: action unavailable.
- Static added to draw pile vs discard pile: as the intent states.
- Outrage Buzz and Network Promo Buzz follow the same −1 timing as Trending.
- A cost that would take Cash below 0: unavailable, Cash never goes negative (proposed).

## Open questions (ask before building)
- Cash never goes negative (an effect that would take more is unavailable). Proposed above.
- Interference shuffled into the draw pile at a random position on `rng.shuffle`. Proposed above.
  Alternative: placed on top (fully visible, pillar 1).

## Done checklist
- [ ] Every row above has gain/clear tests
- [ ] Buzz timing and per-hit rounding tested
- [ ] Burnout stacks by name, including RM versions
- [ ] Interference removal at daypart end, Sticky kept
- [ ] Complaint Call play and end-of-turn loss
- [ ] Every status is shown where `ui.md` puts it (sticky notes, VU meters, readouts, crate count)
- [ ] Every edge case above has a test
- [ ] Reviewer sign-off
