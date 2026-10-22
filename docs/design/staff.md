# Staff

Source: GAME_BRIEF.md section 8. The brief wins if this doc disagrees.

## Purpose
Staff are people with personality traits. Most have a visible upside and a visible downside. Three
slots force choices: hiring a 4th means letting one go.

## Rules
- **3 slots**, shown on the staff board by the booth door.
- Each staff member is data: id, name, trait text, triggers, effects (same system as gear).
- Hiring when all 3 slots are full: choose one to let go, or decline the hire. Let-go staff are gone for the run.
- Sources: mainly Station Events, sometimes Sponsor Meetings.
- Any chance-based trait rolls through the `staff` RNG stream and shows its result **before you act**.

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
- Two staff with the same trigger: slot order (left to right).

## Open questions (ask before building)
- Loose Cannon's −3: applied at turn start when rolled (proposed), or at end of turn?
- Loose Cannon's +6: base or flat (proposal above)?
- Can a let-go staff member be rehired later in the same run? Proposed: no.

## Done checklist
- [ ] Staff schema + ScriptableObject + exporter
- [ ] Slots and swap prompt tested
- [ ] Old Reliable and Loose Cannon tested, Loose Cannon seeded and shown before acting
- [ ] Staff board shows 3 slots with portraits and tooltips, controller-selectable
- [ ] Every staff member listed in `docs/CONTENT.md`
- [ ] Reviewer sign-off
