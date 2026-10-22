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
Seed and every RNG stream's state; format; market; book; map layout and current node; visited nodes;
deck (card ids + remaster flags); gear ids; staff ids by slot; listeners and max; Cash; Request tokens;
cut count; seen events; offered gear; per-run counters for Ratings Points; save version.

### When to save
- Autosave on entering every node and after every node's outcome is resolved.
- On quit from the map: save and return to title.
- Mid-fight: the run save holds the state from when you entered the fight node (see open question).
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

## Open questions (ask before building)
- **Quit mid-fight:** resume restarts the fight from daypart start with the same seed (proposed — deterministic,
  so no reroll abuse), or save every action and resume mid-turn?

## Done checklist
- [ ] Save data is plain Core types, serialized with no Unity dependency
- [ ] Round-trip test: save → load → identical state and identical future rolls
- [ ] Atomic write and `.bak` recovery tested
- [ ] Version migration framework with a v0→v1 test
- [ ] Continue / abandon flow on the title screen
- [ ] Steam Cloud paths configured (`steam.md`)
- [ ] Reviewer sign-off
