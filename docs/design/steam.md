# Steam

Source: GAME_BRIEF.md sections 10, 12, 13. The brief wins if this doc disagrees.

## Purpose
Ship on Steam under Consumed Media: Steam Deck verified at launch, full controller support,
achievements, cloud saves, an early Coming Soon page and a Next Fest demo. Price target $14.99–$19.99.

## Rules
- **Steam Deck verified:** 1280×800 default, no text under 12 px, full controller support, no launcher,
  correct glyphs, Steam on-screen keyboard where text entry exists, suspend/resume safe.
- **Controller:** every screen; Steam Input with a default layout.
- **Cloud saves:** `profile.json` and `run.json` (`save.md`). Settings stay local.
- **Achievements:** unlocked from Core events via a Game-layer bridge; Core has no Steam reference.
- **Steamworks integration** sits behind an interface in TurnTheDial.Game so builds run without Steam.
- **Coming Soon page** early. **Demo** for Steam Next Fest.
- Subtitle and trademark search are still open (brief "Still open").

### Achievements (v1 proposal)
| Id | Name | Condition |
| --- | --- | --- |
| `ach.first_win` | On the Air | Win your first daypart |
| `ach.book1` | First Book | Beat the book 1 boss |
| `ach.season` | Ratings Champ | Win a run |
| `ach.each_format` | Full Spectrum | Win a run with each format |
| `ach.big_metro` | Big Metro | Win a run in Big Metro |
| `ach.flow_max` | In the Pocket | Reach the Flow cap |
| `ach.no_dead_air` | Never Silent | Win a run without taking Dead Air |
| `ach.drop` | Drop It | Deal 50+ Reach with one Drop card (one hit) |
| `ach.ads` | Sold Out | Play 5 Ads in one daypart |
| `ach.all_rivals` | Know the Market | Beat all 12 regular rivals |

The state each achievement needs across turns, runs and sessions is listed in `save.md`
(Achievement progress).

Ids follow the project's dotted pattern (`ach.*`). If Steamworks rejects dots in API names, the
Game-layer bridge maps each id to its Steam API name; Core and saves use the dotted id.

## Tunable values
| Key | Start |
| --- | --- |
| `steam.price` | $14.99–$19.99 |
| `demo.scope` | Book 1, Classic Rock, Chill Coastal Village *(proposed)* |

## Edge cases
- Steam not running: game runs, achievements queue locally and sync later (proposed).
- Cloud conflict: see `save.md`.
- Demo saves don't carry into the full game (proposed).

## Open questions (ask before building)
- Achievement list (proposal above).
- Demo scope (proposal above) and whether demo progress carries over.
- Steam subtitle and trademark search.

## Done checklist
- [ ] Steamworks wrapper behind an interface, game runs without Steam
- [ ] Achievements fire from Core events, tested with a fake bridge
- [ ] Cloud save paths configured and tested on two machines
- [ ] Steam Deck compatibility checklist passed on hardware
- [ ] Coming Soon page assets (capsule uses the tuner)
- [ ] Demo build config
- [ ] Reviewer sign-off
