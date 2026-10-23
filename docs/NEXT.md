# Next

## Current system
Single-fight prototype (Proto_Combat), built as one system per the user's 2026-10-23 decisions:
Classic Rock starter deck + Ribbon Mic vs Morning Zoo, every Proposed rule the fight touches built
as its Proposed default and marked `// Proposed (<doc>.md)` in code.
- Core: `unity/Assets/TurnTheDial/Core/` (fight loop, Reach/Drop/Flow/statuses, seeded RNG streams,
  combat log, validator, rules text, sim runner).
- Data: ScriptableObjects + exporter; content in `unity/Assets/TurnTheDial/Content/`.
- Game: `unity/Assets/TurnTheDial/Scenes/Proto_Combat.unity` (plain uGUI, mouse/keyboard/gamepad).
- Tests: 176 EditMode tests pass (`TurnTheDial.Core.Tests`, `TurnTheDial.Data.Tests`).
- Sim: Unity menu Turn the Dial > Sim, report in `tools/sim/reports/`.

## Next step
1. Done: the reviewer returned APPROVED in round 2 (round 1: 3 majors, 11 minors, all fixed or
   queued below). Round 2 minors: README test name and cue-slot hint fixed; the doc keys for
   findings 8, 11 and the ui.md keys wait on the questions below (LESSONS tunable rule not yet met
   for `onAirWarningFlickerHz` and `logLinesShown`).
2. The user answers the prototype's doc questions (below, "From the prototype").
3. Then the next system. Done-checklist items still open from this prototype: card rewards and
   rerolls, Production Room remaster, elites and bosses (passives, Monitor UI ghosts), the other
   11 rivals and 2 formats as content, sim of every rival vs every deck, saves and fight snapshots,
   gear and staff beyond Ribbon Mic, markets beyond tier 1, the booth art and desk-view layout.

### From the prototype (unclear in the docs; builder choice pinned by a test, pending decision)
- Cue timing: brief §2 lists Cue (step 4) after Program (step 3). Built: once you cue, you can't
  play more cards that turn (marked `Proposed (combat.md, player phase order; pending user decision)`;
  tooltip and controls text say "Cueing ends your plays this turn"). Confirm, or allow plays after
  cueing.
- Gear "card played" triggers: cards.md's effect order has no step for them. Built: after the card
  reaches its pile (step 9), before the win/loss check (step 10). Test:
  `PendingDecisionTests.CardPlayedGear_*`.
- Simultaneous 0 (combat.md): a card that drops the rival to 0 and your listeners to 0 in one
  resolution. Built: the listeners-0 loss is checked first. Test:
  `PendingDecisionTests.SimultaneousZero_*`.
- Monitor draw peek: how long the peek stays visible isn't stated. Built: a snapshot of the peeked
  cards when the effect resolves, kept until end of turn. Test: `PendingDecisionTests.MonitorDraw_*`.
- Loop + Air Horn (formats.md covers only an Air-Horned T5). Built: (a) an Air-Horned T1 after a T5
  still wraps (the wrap checks the card's own tempo 1); (b) an Air-Horned T4 counts as 5, so a T1
  after it wraps. Tests: `PendingDecisionTests.Loop_*`.
- cards.md Fields: add `endOfTurnInHand` (an effect list that resolves at end-turn step 2 while the
  card is in hand; Complaint Call's "lose 3") as Proposed. Brief §4 card anatomy has no such field.
- save.md RNG streams: add the tooling-only `seed.next` stream (`SeedChain`: derives the prototype's
  restart seed and the sim's seed list from the current seed) as Proposed. It's not a gameplay roll.
- ui.md Tunable values: add `ui.onAirFlickerHz` (prototype uses 2, under the 3 flashes/second
  photosensitivity limit) and a combat-log length key, e.g. `ui.logLinesShown` (prototype uses 200).
  Both are prototype-only values in `UiConfig.asset` until the doc owns them.
- ui.md: "C cues the hovered card when nothing is focused" can't occur, because focus is never
  empty (focus is always visible). Built: C cues the card under the mouse if there is one, else the
  focused card. Right-click also cues.

## Blocked on
- The user's answers to the open questions below. Each doc's full list is under its own
  **Open questions** heading.
- CLAUDE.md's Determinism stream list doesn't include the proposed `rng.staff` stream (`save.md`).
  If the user accepts it, only the user can add it to CLAUDE.md.

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
