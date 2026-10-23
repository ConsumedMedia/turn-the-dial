# Content tracker

Every card, format, market, rival, gear piece, staff member, event and achievement in the game, with
its status.
Ids are dotted. Prefixes: cards `card.cr.*` (Classic Rock), `card.hh.*` (Hip-Hop Heat), `card.cc.*`
(Country Roads), `card.sh.*` (Shared), `card.int.*` (Interference, Rival pool); `format.*`,
`market.*`, `rival.*`, `elite.*`, `boss.*`, `gear.*`, `staff.*`, `event.*` (none designed yet),
`ach.*` (achievements, `steam.md`).
Status: **designed** (in a design doc) → **built** (data asset exists) → **tested** (Core tests pass).
Add a row the moment something is designed. Ids are stable once built (CLAUDE.md).

v1 targets: 3 formats, 6 markets, ~120 cards (~30 per format + ~30 shared), ~22 gear, ~8 staff,
12 rivals, 4 elites, 3 bosses, ~20 events.

## Cards

| Id | Name | Pool | Type | Rarity | Source doc | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `card.cr.album_cut` | Album Cut | Classic Rock | Song · Rock · T3 | Starter | formats.md | designed |
| `card.cr.request_line` | Request Line | Classic Rock | DJ Break | Starter | formats.md | designed |
| `card.cr.car_dealer_spot` | Car Dealer Spot | Classic Rock | Ad | Starter | formats.md | designed |
| `card.cr.power_ballad` | Power Ballad | Classic Rock | Song · Rock · T2 · Anthem | Starter | formats.md | designed |
| `card.sh.station_id` | Station ID | Shared *(Proposed)* | Liner | Starter | formats.md | designed |
| `card.hh.corner_cypher` | Corner Cypher | Hip-Hop Heat | Song · Hip-Hop · T1 | Starter | formats.md | designed |
| `card.hh.block_party` | Block Party | Hip-Hop Heat | Song · Hip-Hop · T2 | Starter | formats.md | designed |
| `card.hh.basement_session` | Basement Session | Hip-Hop Heat | Song · Hip-Hop · T3 | Starter | formats.md | designed |
| `card.hh.rooftop_jam` | Rooftop Jam | Hip-Hop Heat | Song · Hip-Hop · T4 | Starter | formats.md | designed |
| `card.hh.shout_out` | Shout-Out | Hip-Hop Heat | DJ Break | Starter | formats.md | designed |
| `card.hh.hype_man` | Hype Man | Hip-Hop Heat | DJ Break · Drop 2 | Starter | formats.md | designed |
| `card.hh.air_horn` | Air Horn | Hip-Hop Heat | Liner | Starter | formats.md | designed |
| `card.hh.sneaker_drop_spot` | Sneaker Drop Spot | Hip-Hop Heat | Ad | Starter | formats.md | designed |
| `card.hh.banger` | Banger | Hip-Hop Heat | Song · Hip-Hop · T5 · Drop 3 | Starter | formats.md | designed |
| `card.cc.back_porch_tune` | Back Porch Tune | Country Roads | Song · Country · T2 | Starter | formats.md | designed |
| `card.cc.listener_mail` | Listener Mail | Country Roads | DJ Break | Starter | formats.md | designed |
| `card.cc.feed_store_spot` | Feed Store Spot | Country Roads | Ad | Starter | formats.md | designed |
| `card.cc.caller_nine_wins` | Caller Nine Wins | Country Roads | Promo | Starter | formats.md | designed |
| `card.int.static` | Static | Rival | Interference · Unplayable | Special | statuses.md | designed |
| `card.int.complaint_call` | Complaint Call | Rival | Interference | Special | statuses.md | designed |

**Proposed:** Station ID is one card in the Shared pool, used by both the Classic Rock and Country
Roads starter decks, rather than one copy per format pool. Its rarity is Starter, and rewards and shops
only roll Common, Uncommon and Rare, so it never appears in any format's rewards or shops either way.
The choice only affects data layout: one shared id (`card.sh.station_id`), or two format ids
(`card.cr.station_id`, `card.cc.station_id`) split before it is built. Open question: keep one
Shared id (proposed, less duplicated data)?

## Formats

| Id | Name | Signature | Starting gear | Source doc | Status |
| --- | --- | --- | --- | --- | --- |
| `format.classic_rock` | Classic Rock | Anthem | `gear.ribbon_mic` | formats.md | designed |
| `format.hip_hop_heat` | Hip-Hop Heat | Loop | `gear.sampler` | formats.md | designed |
| `format.country_roads` | Country Roads | Mailbag | `gear.mail_sack` | formats.md | designed |

## Markets

| Id | Name | Tier | Source doc | Status |
| --- | --- | --- | --- | --- |
| `market.chill_coastal_village` | Chill Coastal Village | 1 | markets.md | designed |
| `market.college_town` | College Town | 2 | markets.md | designed |
| `market.blue_collar_heartland` | Blue-Collar Heartland | 3 | markets.md | designed |
| `market.retiree_coast` | Retiree Coast | 4 | markets.md | designed |
| `market.true_crime_suburbia` | True Crime Suburbia | 5 | markets.md | designed |
| `market.big_metro` | Big Metro | 6 | markets.md | designed |

## Rivals

| Id | Name | Kind | Book | Source doc | Status |
| --- | --- | --- | --- | --- | --- |
| `rival.morning_zoo` | Morning Zoo | Regular | 1 | rivals.md | designed |
| `rival.smooth_jazz` | Smooth Jazz 98.3 | Regular | 1 | rivals.md | designed |
| `rival.sports_talk` | Sports Talk Guys | Regular | 1 | rivals.md | designed |
| `rival.oldies_gold` | Oldies Gold 1490 | Regular | 1 | rivals.md | designed |
| `rival.playlist_bot` | Playlist Bot | Regular | 2 | rivals.md | designed |
| `rival.hot_hits` | Hot Hits 104 | Regular | 2 | rivals.md | designed |
| `rival.love_lines` | Late Night Love Lines | Regular | 2 | rivals.md | designed |
| `rival.wkru` | College Radio WKRU | Regular | 2 | rivals.md | designed |
| `rival.megahits` | MegaHits 102 | Regular | 3 | rivals.md | designed |
| `rival.rock_block` | Rock Block 101 | Regular | 3 | rivals.md | designed |
| `rival.farm_report` | Farm & Market Report | Regular | 3 | rivals.md | designed |
| `rival.drive_at_five` | Drive at Five | Regular | 3 | rivals.md | designed |
| `elite.top40` | Top 40 Countdown | Prime Time | Any | rivals.md | designed |
| `elite.shock_jock` | Shock Jock Hour | Prime Time | Any | rivals.md | designed |
| `elite.pledge_drive` | Public Radio Pledge Drive | Prime Time | Any | rivals.md | designed |
| `elite.traffic_8s` | Traffic on the 8s | Prime Time | Any | rivals.md | designed |
| `boss.big_wakeup` | The Big Wake-Up | Boss | 1 | bosses.md | designed |
| `boss.endless_stream` | Endless Stream | Boss | 2 | bosses.md | designed |
| `boss.the_flip` | The Flip | Boss | 3 | bosses.md | designed |

## Gear

| Id | Name | Rarity | Source doc | Status |
| --- | --- | --- | --- | --- |
| `gear.ribbon_mic` | Ribbon Mic | Starter (Classic Rock) | gear.md | designed |
| `gear.sampler` | Sampler | Starter (Hip-Hop Heat) | gear.md | designed |
| `gear.mail_sack` | Mail Sack | Starter (Country Roads) | gear.md | designed |

## Staff

| Id | Name | Source doc | Status |
| --- | --- | --- | --- |
| `staff.old_reliable` | Old Reliable | staff.md | designed |
| `staff.loose_cannon` | Loose Cannon | staff.md | designed |

## Station Events

| Id | Name | Source doc | Status |
| --- | --- | --- | --- |
| — | None designed yet (~20 planned) | nodes.md | — |

## Achievements

The list itself is a proposal (`steam.md` open questions).

| Id | Name | Source doc | Status |
| --- | --- | --- | --- |
| `ach.first_win` | On the Air | steam.md | designed |
| `ach.book1` | First Book | steam.md | designed |
| `ach.season` | Ratings Champ | steam.md | designed |
| `ach.each_format` | Full Spectrum | steam.md | designed |
| `ach.big_metro` | Big Metro | steam.md | designed |
| `ach.flow_max` | In the Pocket | steam.md | designed |
| `ach.no_dead_air` | Never Silent | steam.md | designed |
| `ach.drop` | Drop It | steam.md | designed |
| `ach.ads` | Sold Out | steam.md | designed |
| `ach.all_rivals` | Know the Market | steam.md | designed |
