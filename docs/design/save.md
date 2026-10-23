# Save system

Source: GAME_BRIEF.md sections 6, 9, 12. The brief wins if this doc disagrees.

## Purpose
Save and quit at any node, resume at that node. Keep the profile (unlocks, Ratings Points, settings)
safe across runs and Steam Cloud. Saves are plain data from Core; Unity only writes the bytes.

## Rules

### Files
| File | Contents |
| --- | --- |
| `profile.json` | Lifetime Ratings Points, unlocked bundles, formats, markets, first-run flag, stats, achievement progress. |
| `run.json` | The current run (absent when no run is active). |
| `settings.json` | Settings (`ui.md`). Local only, not synced. |

### Run save contents
Seed and the state of every RNG stream below (by stream name); format; market; book; map layout and
current node; visited nodes; deck (card ids + remaster flags); gear ids **in acquisition order**
(trigger order, `gear.md`); staff ids by slot; listeners and max; Cash; Request tokens; cut count;
seen events; offered gear; per-run counters for Ratings Points; save version. Plus the state the
no-repeat rules need:
- **Rivals fought this book** (rival ids, in order; cleared at the start of each book) for
  `rivals.md`'s no-repeat draw.
- **Elites fought this run** (elite ids, in order) for the elite no-repeat rule (`rivals.md`).
- **Staff let go this run** (staff ids) so they are never offered again (`staff.md`).
- **Dead Air taken this run** (a flag, set the first time Dead Air triggers in any fight) for
  `ach.no_dead_air`. The per-daypart Dead Air count resets each daypart, so it can't do this job.

### Achievement progress
**Rule:** the fight snapshot stores the in-progress counter of every daypart-scoped achievement, the
run save stores it for every run-scoped one, and `profile.json` stores it for every lifetime one.
Checked against all 10 achievements in `steam.md`:

| Achievement | Scope | Stored state |
| --- | --- | --- |
| `ach.first_win` | Instant (fight won) | None; fires on the win event. |
| `ach.book1` | Instant (boss 1 beaten) | None. |
| `ach.season` | Instant (run won) | None. |
| `ach.each_format` | Lifetime | Profile: format ids you have won a run with. |
| `ach.big_metro` | Instant (run won) | None; the market is already in the run save. |
| `ach.flow_max` | Instant (Flow hits cap) | None. |
| `ach.no_dead_air` | Run | Run save: Dead Air taken this run (flag, above). |
| `ach.drop` | Instant (one Drop card's Reach) | None. |
| `ach.ads` | Daypart | Fight snapshot: Ads played this daypart. |
| `ach.all_rivals` | Lifetime | Profile: regular rival ids you have ever beaten. |

A new achievement isn't done until its row is added here.

**In-node state** (saved while you are inside a node, so quitting and resuming never re-rolls or
repeats anything):
- **Sponsor Meeting:** the stock (each slot's item id and rolled price), which slots are bought, which
  slot has the sale tag, and whether the card cut was used this visit (`nodes.md`).
- **Card reward (after a fight):** whether the reward Cash was paid, the 3 offered card ids, how many
  rerolls were used on this offer, and any pending gear pick (Prime Time gear, boss gear choices).
- **Remote Broadcast:** the 2 offered gear ids.
- **Station Event:** event id, current choice step, any rolled outcome not yet applied, and whether
  the token reroll was used (`nodes.md`).
- **Staff swap prompt:** the offered staff id while you choose who to let go (`staff.md`).
- **Production Room:** nothing rolled; the node is saved as entered, not resolved.

### RNG streams
Every random roll goes through one seeded RNG service in Core (`IRng`). Each stream below is a
separate generator derived from the run seed and its stream name, so rolls in one system never shift
another's. The derivation must use a fixed, version-stable hash (never `string.GetHashCode`, which
changes between processes). **Proposed** (derivation method). Every stream's state is saved in
`run.json` and in each fight snapshot.

| Stream | Rolls that use it |
| --- | --- |
| `rng.shuffle` | Deck shuffle at daypart start; reshuffling the discard pile into the draw pile; Interference shuffled into the draw pile (**Proposed**, `statuses.md`). |
| `rng.rewards` | Card reward offers and their rarity rolls; reward rerolls; reward Cash for Daypart and Prime Time (`nodes.md`); gear offers from Remote Broadcasts, Prime Time and boss rewards (**Proposed**). |
| `rng.shop` | Sponsor Meeting stock, price variance, the sale tag, stock rerolls; staff offered in a shop (**Proposed**). |
| `rng.map` | Map generation for each book, including constraint rerolls (`map.md`). |
| `rng.rivals` | Which regular rival a Daypart node gets and which elite a Prime Time node gets (`rivals.md`). |
| `rng.events` | Which event a Station Event node shows; event chance outcomes and their token rerolls; cards, gear and staff an event offers (**Proposed** for offers). |
| `rng.staff` | In-fight chance traits such as Loose Cannon's turn-start roll (`staff.md`). **Proposed** (keeps fight rolls apart from shuffles). |

- A reroll uses the same stream as the roll it replaces.
- Bosses are fixed per book; no roll. Rival intents are fixed patterns; no roll.
- No other source of randomness is allowed (`System.Random`, `UnityEngine.Random`, `Guid`, clock time).

### When to save
- Autosave on entering every node and after every node's outcome is resolved.
- Autosave after every in-node action that changes in-node state (buy, cut, reroll, event choice,
  card pick). **Proposed.**
- On quit from the map: save and return to title.
- **Mid-fight. Decided** (brief §6; LESSONS quitting line). At the start of every player turn (before Loyalty clears and before the
  draw), Core takes a fight snapshot: piles in order, hand, cue slot, all statuses, Flow, last song,
  Burnout, Dead Air count, rival state and pattern position, and every RNG stream's state. Quitting
  mid-fight saves that snapshot; resuming restarts the current turn from it. Same seed, same draws,
  same intents. Actions taken earlier in the quit turn are lost.
- **Cross-turn fight state in the snapshot.** Everything that lasts longer than one turn is stored too:
  - turn number and fight kind (sets fight length, The Flip and Satellite Feed timing);
  - current Flow cap (after any `RaiseFlowCap` this daypart);
  - genre streak count (Playlist Bot, `flow.md`);
  - a pending Air Horn ramp bonus not yet used by a Song, with its stack count (`flow.md`);
  - cards Pulled or removed for the rest of the daypart (Pulled cards, played Complaint Calls;
    `cards.md`, `statuses.md`), and which cards in the piles are Interference to remove at daypart end;
  - once-per-daypart gear and staff flags (e.g. Sampler has fired, `formats.md`);
  - Monitor reveals still showing this daypart (how many intents are revealed);
  - the rival's starting audience (ratings check), Lock, Buzz and its currently shown intent (some
    passives change an intent when it is revealed, e.g. Pledge Drive, `rivals.md`);
  - Countdown's last Steal base value (`rivals.md`);
  - Algorithm's last-turn genre and Recommended for You's per-genre counts from finished turns
    (`bosses.md`);
  - Ads played this daypart (for `ach.ads`, `steam.md`).
- Per-turn flags (first DJ Break for Ribbon Mic, first Anthem song, Gridlock's first card, Mailbag's
  DJ Break played, cards played this turn for Dead Air) reset at turn start, so the snapshot taken at
  turn start holds them already cleared; they are not stored separately.
- Writes are atomic: write to a temp file, then replace.

### Load
- Title shows Continue when `run.json` exists.
- A save with an unknown content id or newer version: show an error, keep the file, offer to abandon.
- Older save versions run through migrations, one step per version.
- Run end (win or loss) deletes `run.json` after the profile is updated.

## Tunable values
| Key | Start |
| --- | --- |
| `save.version` | 1 |
| `save.backupCount` | 1 (previous good save kept as `.bak`) |

## Edge cases
- Crash during write: the `.bak` or previous file is still valid.
- Corrupt `run.json`: offer to abandon; never block the profile.
- Corrupt `profile.json`: restore from `.bak`; if none, warn before creating a new one.
- Steam Cloud conflict: Steam's newer-file prompt; profile files are small and whole-file.
- Content id removed in an update: never remove shipped ids (CLAUDE.md); migration maps renamed ones.
- Quit mid-turn after drawing: resume replays the turn start and draws the same 5 cards.
- Loose Cannon roll at turn start: the snapshot is taken before the roll, so the resumed turn rolls
  the same result.
- Extra rolls on one stream (e.g. a reward reroll on `rng.rewards`) don't change the next shuffle,
  shop, map or rival draw.
- New book: rivals fought this book clears; elites fought this run and staff let go don't.
- Quit in a Sponsor Meeting after buying one item: resume shows the same stock, prices and sale, with
  that slot empty and the Cash already spent.
- Quit on a card reward after one reroll: resume shows the rerolled 3 cards; the Cash is not paid again.
- Quit mid-event after a chance roll: resume shows the same rolled outcome; the token reroll can't be
  used twice.
- Air Horn played on an earlier turn with no Song since, then quit: the resumed turn starts with the
  bonus still pending. (Anything done earlier in the quit turn itself is lost, as above.)
- Sampler fired on an earlier turn, then quit: it doesn't fire again this daypart.
- A Pulled card played on an earlier turn, then quit: it stays out for the rest of the daypart.
- 3 Ads played on earlier turns this daypart, then quit: the resumed turn still counts 3 toward
  `ach.ads`; Ads played earlier in the quit turn are lost with the rest of that turn.
- Dead Air taken in book 1, then quit and resume in book 2: `ach.no_dead_air` stays failed.
- Save from an older version without a stream added later: the migration derives that stream fresh
  from the seed and its name.

## Open questions (ask before building)
- Stream derivation from the run seed with a fixed, version-stable hash. Proposed above.
- Autosave after every in-node action. Proposed above.
- Stream assignments for gear offers (`rng.rewards`), shop staff (`rng.shop`), event offers
  (`rng.events`), Interference placement (`rng.shuffle`) and Loose Cannon (`rng.staff`). Proposed
  above.

## Done checklist
- [ ] Save data is plain Core types, serialized with no Unity dependency
- [ ] Round-trip test: save → load → identical state and identical future rolls on every stream
- [ ] Stream independence test: extra rolls on one stream leave every other stream's sequence unchanged
- [ ] Rivals fought this book, elites fought this run and staff let go round-trip and gate the
      no-repeat rules
- [ ] Every cross-turn fight field and every in-node field above round-trips
- [ ] Every achievement counter in the Achievement progress table round-trips at its scope
- [ ] Every edge case above has a test
- [ ] Mid-fight test: play part of a turn, quit, resume → same hand, same intent, same outcome for the same inputs
- [ ] Atomic write and `.bak` recovery tested
- [ ] Version migration framework with a v0→v1 test
- [ ] Continue / abandon flow on the title screen
- [ ] Steam Cloud paths configured (`steam.md`)
- [ ] Reviewer sign-off
