# Next

## Current system
Design docs review: done. The reviewer returned APPROVED in round 4, after 4 rounds of fixes
(18, 13, 5 findings). Brief §2, §3, §4 and §6 changed in round 1 with the user's approval; later
rounds changed only `docs/design/`, `CONTENT.md`, `LESSONS.md` and this file.

## Next step
1. The user answers the open questions below (or accepts the proposals as written).
2. If any answer changes a doc, the reviewer re-checks the design docs against the brief.
3. Then pick the first build system (proposed: combat core, `docs/design/combat.md`).

## Blocked on
- The user's answers to the open questions below. Each doc's full list is under its own
  **Open questions** heading.
- CLAUDE.md's Determinism stream list doesn't include the proposed `rng.staff` stream (`save.md`).
  If the user accepts it, only the user can add it to CLAUDE.md.

## Needs user confirmation
- The brief keeps Hype and Burnout for Drop Songs only (Hype Man ignores Hype); the user hasn't
  confirmed this yet.

## Open questions
Most important first. Full lists: each doc's Open questions section.

**cards.md / flow.md**
- Effect order: a card's other base numbers and effects list resolve after its own Reach (Proposed).
- Genre streak count for Playlist Bot: a single on-genre song = 1 (Proposed).

**map.md / rivals.md**
- Is a fight node's rival visible on the map before you pick it (pillar 1)? Proposed: no, drawn on
  entry from rivals not fought this book on `rng.rivals`.
- Hard check of 3 to 5 non-boss fights per path, with 4 to 5 as the target (Proposed).
- Elites: no repeats per run, reset after all 4 (Proposed).
- Gridlock never raises a cost above 3 (Proposed, brief-compliant). Letting it reach 4 needs a brief
  change.
- Pledge Drive: Contests heal the book value; below half, revealed Heavy Rotations become Contests
  (Proposed; replaces "heals double", which exceeded the brief's Contest values).
- Pledge Drive under Monitor: future Heavy Rotations show a conditional ghosted icon (Proposed).

**combat.md / gear.md / staff.md**
- Daypart-start Loyalty is wiped by the turn-1 clear. Proposed: daypart-start effects grant no
  Loyalty. The alternative (grant it after the clear) needs a brief change.
- On every trigger shared by gear and staff, gear first (acquisition order), then staff (slot order).

**bosses.md**
- Satellite Feed 24 exceeds the brief's book 1 Big Segment range (max 18). Proposed: allowed as a
  boss rule ("giant"); otherwise a brief change or drop it to 18.
- Algorithm ties: Proposed default is "both tied genres get −50%". Alternative: no penalty on a tie,
  consistent with Recommended for You's Decided tie rule; recommended. The user decides.
- Loop and Air Horn under The Flip; boss names.

**save.md**
- RNG stream assignments not set by the user: gear offers (`rng.rewards`), shop staff (`rng.shop`),
  event offers (`rng.events`), Interference placement (`rng.shuffle`), Loose Cannon (`rng.staff`).
- Autosave after every in-node action (Proposed).

**nodes.md / gear.md / staff.md / statuses.md / meta.md / markets.md / formats.md**
- Shop rounding, one cut per visit, reroll keeps cut price, one event reroll, no event repeats,
  reward Cash roll distribution, skipping a Remote Broadcast, 50 Cash when the gear pool is empty.
- Gear never offered twice; cue-slot cards count for "first X each turn".
- Declining a hire; let-go staff never return.
- Cash never negative; cued Complaint Call doesn't trigger; Interference position in the draw pile.
- Prime Time wins and book 1 count for Ratings Points.
- College Town +10% includes Prime Time and bosses.
- Remaster carry-overs (unmentioned fields keep base values); Mail Sack on any fight win.

**ui.md / audio.md / CONTENT.md**
- Gamepad map, game speeds, text scales, blink rate, needle time, default audio mix (all Proposed).
- Title, map and boss music variants (scope change to brief §11).
- Station ID as one Shared id vs two format ids (data layout only).
