# Staff

Source: GAME_BRIEF.md section 8. The brief wins if this doc disagrees.

## Purpose
Staff are people with personality traits. Most have a visible upside and a visible downside. Three
slots force choices: hiring a 4th means letting one go.

## Rules
- **3 slots**, shown on the staff board by the booth door.
- Each staff member is data: id, name, trait text, triggers, effects (same system as gear).
- Hiring when all 3 slots are full: choose one to let go (brief §8), or decline the hire
  (**Proposed**). Let-go staff are gone for the run (**Proposed**; saved as "staff let go this run",
  `save.md`).
- Sources: mainly Station Events, sometimes Sponsor Meetings. Staff offers roll on `rng.events` or
  `rng.shop` (**Proposed**).
- Any chance-based trait rolls through the `rng.staff` stream (**Proposed**, `save.md` RNG streams)
  and shows its result **before you act**.

### Staff defined so far
| Staff | Trait | Effect |
| --- | --- | --- |
| Old Reliable | Steady | +3 Loyalty every turn (at turn start, after Loyalty clears). |
| Loose Cannon | Swingy | Each turn, 50% chance of +6 Reach on your first Song, or −3 listeners. Rolled and shown at turn start. |

The rest of the v1 pool (about 8 staff in total) is designed in a later session and tracked in
`docs/CONTENT.md`.

## Tunable values
| Key | Start |
| --- | --- |
| `staff.slots` | 3 |
| `staff.poolSize` | ~8 |
| `oldReliable.loyalty` | 3 |
| `looseCannon.chance` | 0.5 |
| `looseCannon.reachBonus` | 6 |
| `looseCannon.listenerLoss` | 3 |

## Edge cases
- Loose Cannon good roll and no Song played that turn: the bonus is lost.
- Loose Cannon +6 Reach: added to base (before Hype/Flow) or flat after? Proposed: added to base like Hype.
- Loose Cannon bad roll at 3 listeners or fewer: the run ends.
- Declining a hire: the event or shop outcome is lost (no refund unless the event says so).
- Two staff with the same trigger: slot order (left to right). **Proposed.**
- Gear and staff on the same trigger (daypart start, turn start, card played, end of turn, daypart
  won, and so on): all gear first in acquisition order, then staff in slot order. **Proposed** (same
  rule in `combat.md` and `gear.md`). E.g. at turn start, Old Reliable's Loyalty comes after any
  turn-start gear.
- A let-go staff member is never offered again this run (proposed), even by an event.

## Open questions (ask before building)
- Loose Cannon's −3: applied at turn start when rolled (proposed), or at end of turn?
- Loose Cannon's +6: base or flat (proposal above)?
- Can a let-go staff member be rehired later in the same run? Proposed: no.
- Can you decline a hire when all 3 slots are full? Proposed: yes (the offer is lost).
- On every trigger shared by gear and staff, gear resolves before staff; staff in slot order.
  Proposed above.
- Loose Cannon and other chance traits on `rng.staff`; staff offers on `rng.events` / `rng.shop`.
  Proposed above.

## Done checklist
- [ ] Staff schema + ScriptableObject + exporter
- [ ] Slots and swap prompt tested
- [ ] Old Reliable and Loose Cannon tested, Loose Cannon seeded and shown before acting
- [ ] Staff board shows 3 slots with portraits and tooltips, controller-selectable
- [ ] Every staff member listed in `docs/CONTENT.md`
- [ ] Reviewer sign-off
