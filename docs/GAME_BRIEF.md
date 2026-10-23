# Turn the Dial: Game Brief

Single source of truth. Consumed Media · Unity 6 (URP) · Steam, Steam Deck verified.
Built from `docs/Turn the Dial Game Concept.md` (approved). If this brief and the concept disagree, this brief wins. If this brief is silent, the design pillars (section 13) settle it.

All numbers marked *(tune)* are starting values for `tools/sim`. Everything else is a rule.

---

## 1. Pitch and player fantasy

**A roguelike deckbuilder where your deck is the broadcast.** You run a scrappy local radio station. Every card is something you put on air: songs, DJ breaks, ads, promos, station IDs. Program the hour well and listeners turn the dial to you. Program it badly and they turn it to the other guy.

**Fantasy.** Minute one: a program director with a cheap board, three crates of records, and a rival across town eating your lunch. Hour five: you are stringing tempo ramps into a power hit while your rival cuts to a six-minute stopset, and the whole town lights up on your frequency.

**Hook.** Radio is familiar to everyone, nobody has made this game, and Consumed Media knows the industry from the inside.

---

## 2. Turn structure

A **daypart** is one fight against one rival. It lasts a fixed number of turns: **6** for a regular Daypart, **7** for Prime Time, **8** for a Ratings Book boss. One turn is one player phase followed by one rival phase.

### Daypart start
1. Shuffle the deck into the draw pile. Flow, Hype, Burnout, Loyalty and the Dead Air count all start at 0. The cue slot is empty.
2. Apply market modifiers (e.g. True Crime Suburbia: the rival gains Buzz 1).
3. Apply daypart-start gear and staff effects.
4. Reveal the rival's turn-1 intent.

### Player phase
1. **Turn start.** Your Loyalty clears to 0. Airtime is set to 3 (plus modifiers). Start-of-turn gear and staff effects trigger.
2. **Draw 5.** If the draw pile runs out, shuffle the discard pile into it. Hand limit is 10; any extra draws go to the discard pile. A card in the cue slot stays there and can be played from it.
3. **Program.** Play cards in any order by paying their Airtime. Each card resolves fully before the next one. Win and loss are checked after every card (see Win/Loss).
4. **Cue (optional).** If the cue slot is empty, move 1 card from your hand into it.
5. **End turn**, in this order:
   1. **Dead Air check.** If you played no card with On Air this turn, take Dead Air (section 3).
   2. End-of-turn card effects resolve (e.g. Complaint Call).
   3. Discard your hand, except cards with Hold and the cued card. Unspent Airtime is lost.
   4. Hype −1 (Mailbag exception, section 5).

### Rival phase
1. The rival's Lock clears to 0.
2. The rival performs the intent you saw.
3. The rival's Buzz −1 (if above 0).
4. If this was the daypart's last turn, run the **ratings check**. Otherwise reveal the next intent.

### Win/Loss
- **Rival audience hits 0:** you win immediately.
- **Your listeners hit 0:** the run ends immediately, in any fight.
- **Ratings check** (after the final rival phase): your share = listeners ÷ maximum listeners. The rival's share = audience ÷ its starting audience. The higher share wins, and a tie goes to you.
- **Ratings loss:** in a regular Daypart or Prime Time you get no rewards and lose 10% of your current listeners (rounded up), and the run continues. Your maximum listeners don't change. Losing a boss on ratings ends the run.

### Reach resolution (order of operations)
For a song: `Reach = floor((base + Hype) × (1 + 0.1 × Flow) × Burnout factor × Exposed factor)`
1. Lock absorbs the Reach first; whatever is left lowers the rival's audience.
2. You gain listeners equal to **10%** of the Reach that got past Lock (rounded down), up to your maximum.
3. Then update Flow from this song (section 3). A song never benefits from the Flow it earns itself.

Non-song cards that deal Reach skip Hype and the Flow multiplier. Drop cards, Song or not, follow their own formula (section 3): one hit, no Flow multiplier.

---

## 3. Resources and status effects

| Name | Owner | Exact rule |
| --- | --- | --- |
| **Listeners** | You | Your health, kept for the whole run. Starting maximum depends on format (section 5). Reach that gets past Lock converts 10% (rounded down) into listeners, up to the maximum. Hitting 0 ends the run. |
| **Rival audience** | Rival | The rival's health. Its starting value is used for the ratings check. Values in section 7. |
| **Airtime** | You | 3 per turn. Unspent Airtime is lost at end of turn. Card costs are 0 to 3. |
| **Cash** | You | Start the run with 50. Earned mostly from Ads. Spent at Sponsor Meetings. Kept for the whole run. |
| **Loyalty** | You | Block. Absorbs rival steals before your listeners take any, and nothing else. Clears at the start of your turn. Does **not** block Dead Air, Ad listener costs, or any other listener loss. |
| **Lock** | Rival | Rival block, from the Heavy Rotation intent. Absorbs your Reach first. Clears at the start of the rival phase. |
| **Flow** | You | Combo counter, 0 to 10 (cap can be raised by Loop, gear and cards). Each song adds **+1** if its tempo is higher than the last song's (tempo ramp) and **+1** if both it and the last song are your format's genre (genre streak). Each stack gives songs **+10% Reach**. Flow lasts the whole daypart. **Breaks to 0:** playing an Ad, taking Dead Air, or certain rival effects. A song with lower tempo only misses the ramp bonus. "Last song" = the most recent Song played this daypart; non-song cards don't change it, except an Ad, which clears it. |
| **Drop** | Card keyword | `Drop N`: the card deals one number, `floor((base Reach + Hype + N × Flow) × Anthem × Burnout × Exposed × boss modifiers)`, as a single hit (Lock absorbs it once and it converts to listeners once). The Flow multiplier never applies to a Drop card. Hype and Burnout apply only if the card is a Song; Anthem only if it is an Anthem Song that gets the bonus. Then set Flow to 0. The Drop resolves last: a Drop Song's own Flow update happens first and is then wiped, so Flow is always 0 after a Drop card. |
| **Hype** | You | Each stack gives every Song you play +1 Reach (added to base, before the Flow multiplier). −1 at the end of your turn. Built by Promos. |
| **Buzz** | Rival | While Buzz is above 0, the rival's steals take **50% more** listeners (rounded down). −1 at the end of the rival phase. Already included in the intent number shown. |
| **Burnout** | Song card | Each repeat play of a song with the same name (remastered versions count) in a daypart adds 1 Burnout. **−25% Reach per stack**, never below 25% of base. Resets after the daypart. |
| **Exposed** | Rival | While the rival's shown intent is Commercial Break, your Reach against it gets **+50%** this turn. |
| **Static** | Your deck | The count of Interference cards in your piles, shown on the board. Interference cards leave your deck when the daypart ends, except cards with Sticky. |
| **Dead Air** | You | Triggers when you end a turn without playing an On Air card. Lose **5** listeners the first time, **10** the second and **15** for each time after that in the same daypart. Ignores Loyalty. Flow drops to 0. |
| **Request tokens** | You | Start each run with 2. Spend 1 to reroll a card reward, a Sponsor Meeting's stock, or one Station Event's outcome. About 3 to 5 more are found per run. |
| **Ratings Points** | Profile | Meta currency, earned at the end of each run (section 9). |

**Interference cards (v1):**
- **Static:** Unplayable.
- **Complaint Call:** 1 Airtime to play, which removes it for the rest of the daypart. If it's still in your hand at end of turn, lose 3 listeners.

---

## 4. Card anatomy

Every card is a data asset; no card logic is hard-coded per card (pillar 7).

| Field | Values / rule |
| --- | --- |
| `id`, `name` | Unique id and display name. Burnout matches on `name`. |
| `cost` | Airtime, 0 to 3. |
| `type` | Song, DJ Break, Ad, Promo, Liner, Interference. |
| `genre` | Songs only: Rock, Hip-Hop, Country, Pop (shared pool). Other types: none. |
| `tempo` | Songs only: 1 to 5. Shown as five pips on the record label. |
| `rarity` | Starter, Common, Uncommon, Rare, Special (Interference, event cards). |
| `pool` | Classic Rock, Hip-Hop Heat, Country Roads, Shared, or Rival (Interference cards). |
| `reach`, `loyalty`, `hype`, `cash`, `listenerCost` | Base numbers; any can be 0. |
| `keywords` | See list below. Every keyword has a tooltip (pillar 5). |
| `effects` | An ordered list of data-driven effects (draw N, gain Flow, peek N, etc.). |
| `upgrade` | The full remastered card (same fields). Each card can be remastered once; remastered cards show a gold **RM** stamp. |
| `text`, `flavor`, `artId` | Rules text is generated from data where possible. |

**Type defaults:**
- **Song:** On Air. Builds Flow. The main source of Reach.
- **DJ Break:** On Air. Setup, Loyalty, Drops.
- **Ad:** Gives Cash and costs listeners or Loyalty. Breaks Flow.
- **Promo:** Builds Hype. Not On Air.
- **Liner:** Costs 0 to 1. Not On Air. Doesn't change "last song". The glue cards.
- **Interference:** Rival junk (section 3).

**Keywords (v1):**
- **On Air:** counts against Dead Air.
- **Drop N:** see section 3.
- **Anthem:** Classic Rock signature (section 5).
- **Hold:** stays in hand at end of turn.
- **Monitor N:** see the rival's next N intents this daypart, or the top N cards of your draw pile, as the card states.
- **Pulled:** removed for the rest of the daypart once played.
- **Sticky:** Interference that doesn't leave after the daypart.
- **Unplayable**

---

## 5. The three v1 formats

| | Classic Rock | Hip-Hop Heat | Country Roads |
| --- | --- | --- | --- |
| **Max listeners** | 85 | 70 | 78 |
| **Playstyle** | Steady and durable. Big single hits, strong Loyalty. Forgiving: the format for new players. | Combo engine. Long tempo ramps and Flow chains. High risk, high ceiling. | Community and sustain. Listener mail, call-ins, slow-building Hype. |
| **Signature** | **Anthem:** the first Anthem-keyword Song you play each turn gets +50% Reach (multiplied after the Flow multiplier) and gives Loyalty equal to its tempo × 2, regardless of what was played before it. Songs without Anthem never use up the bonus. | **Loop:** Flow cap is 15. A tempo 1 song played right after a tempo 5 song counts as a rising ramp. | **Mailbag:** Reach converts to listeners at 20% instead of 10%. Hype doesn't drop at end of turn if you played a DJ Break that turn. |
| **Starting gear** | Ribbon Mic: your first DJ Break each turn gives +3 Loyalty. | Sampler: the first time Flow reaches 5 in a daypart, draw 2. | Mail Sack: after winning a daypart, recover 4 listeners. |

**Starting decks (10 cards each, template 4 songs / 3 DJ breaks / 1 liner / 1 ad / 1 signature).** Values *(tune)*; remastered (RM) values in brackets.

**Classic Rock**
| Qty | Card | Type | Cost | Effect |
| --- | --- | --- | --- | --- |
| 4 | Album Cut | Song · Rock · T3 | 1 | 6 Reach [9] |
| 3 | Request Line | DJ Break | 1 | 5 Loyalty [8] |
| 1 | Station ID | Liner | 0 | Draw 1 [Draw 2] |
| 1 | Car Dealer Spot | Ad | 0 | +25 Cash, lose 4 listeners [+35 Cash] |
| 1 | Power Ballad | Song · Rock · T2 · Anthem | 2 | 9 Reach [13] |

**Hip-Hop Heat**
| Qty | Card | Type | Cost | Effect |
| --- | --- | --- | --- | --- |
| 1 | Corner Cypher | Song · Hip-Hop · T1 | 1 | 5 Reach [7] |
| 1 | Block Party | Song · Hip-Hop · T2 | 1 | 5 Reach [7] |
| 1 | Basement Session | Song · Hip-Hop · T3 | 1 | 5 Reach [7] |
| 1 | Rooftop Jam | Song · Hip-Hop · T4 | 1 | 5 Reach [7] |
| 2 | Shout-Out | DJ Break | 1 | 4 Loyalty [7] |
| 1 | Hype Man | DJ Break · Drop 2 | 1 | 3 Reach + Drop 2 [Drop 3] |
| 1 | Air Horn | Liner | 0 | Your next Song counts as +1 tempo for the ramp [and draw 1] |
| 1 | Sneaker Drop Spot | Ad | 0 | +20 Cash, lose 3 listeners [+30 Cash] |
| 1 | Banger | Song · Hip-Hop · T5 · Drop 3 | 2 | 7 Reach + Drop 3 [10 Reach] |

**Country Roads**
| Qty | Card | Type | Cost | Effect |
| --- | --- | --- | --- | --- |
| 4 | Back Porch Tune | Song · Country · T2 | 1 | 6 Reach [9] |
| 3 | Listener Mail | DJ Break | 1 | 4 Loyalty, +1 Hype [6 Loyalty] |
| 1 | Station ID | Liner | 0 | Draw 1 [Draw 2] |
| 1 | Feed Store Spot | Ad | 0 | +20 Cash, lose 3 listeners [+30 Cash] |
| 1 | Caller Nine Wins | Promo | 1 | +3 Hype [+5] |

---

## 6. Run structure

**One run = one ratings season = 3 books.** Target 35 to 45 minutes. You can save and quit at any node; the run resumes at that node. Quitting mid-fight resumes at the start of the current turn, from a snapshot taken at turn start (same seed, same intents).

**Map rules (per book):**
- 8 rows: 7 rows of nodes, then the boss. 3 lanes; each row has 2 to 3 nodes. Every node connects to 1 or 2 nodes in the next row. Paths may cross lanes but never cross each other.
- The whole book and every node type are visible from the start (pillar 1).
- **Row 1:** all Daypart. **Row 4:** all Remote Broadcast. **Row 7:** all Production Room. **Row 8:** Ratings Book boss.
- **Rows 2, 3, 5, 6** are weighted *(tune)*: Daypart 45%, Station Event 25%, Sponsor Meeting 12%, Prime Time 10% (rows 3 and up only), Production Room 8%.
- **Generator constraints:** every path has at least one reachable Sponsor Meeting. No two Sponsor Meetings or Production Rooms in a row on the same path. At most 4 regular Daypart nodes on any path, so a book's 4 rivals never repeat; Prime Time fills any extra fight slot. About 4 to 5 fights per book on any path, not counting the boss.
- Each daypart's wash comes from its row: rows 1 to 2 Morning drive, rows 3 to 4 Midday, rows 5 to 6 Afternoon drive, rows 7 to 8 Overnight.

| Node | What it does |
| --- | --- |
| **Daypart** | Regular fight (6 turns). Win: Cash, then pick 1 of 3 cards or skip. |
| **Prime Time** | Elite fight (7 turns). Win: Cash, a 1-of-3 card pick (at least 1 uncommon), and 1 gear. |
| **Sponsor Meeting** | Shop (section 9). |
| **Production Room** | Choose one: recover 30% of your maximum listeners, or remaster one card. |
| **Station Event** | A short text scene with 2 to 3 choices. Every cost is stated exactly; any chance outcome shows its odds. Can give cards, gear, staff, Cash, Request tokens, listeners or Interference. |
| **Remote Broadcast** | Pick 1 of 2 gear. |
| **Ratings Book** | Boss fight (8 turns). Win books 1 and 2: Cash, a 1-of-3 rare card pick, and 1 of 3 boss gear. Winning book 3 wins the run. |

**Reward Cash** *(tune)*: Daypart 8 to 14, Prime Time 20 to 28, boss 50.
**Card reward rarity:** Daypart 60/35/5, Prime Time 45/45/10, boss 0/0/100 (common/uncommon/rare).

---

## 7. Rivals, elites, and bosses

**Intents.** The rival's next move is always shown above its frequency on the tuner (right side), as an icon plus its exact final number (Buzz and Exposed already included). A tooltip shows how the number was calculated. Monitor effects show further intents as smaller, ghosted icons after the current one. Each rival follows a fixed, readable pattern; no hidden randomness.

| Intent | Effect | Book 1 / 2 / 3 values *(tune)* |
| --- | --- | --- |
| **Steal** | Take X listeners. Loyalty absorbs it first. | 6–9 / 9–13 / 12–16 |
| **Big Segment** | A large steal, sometimes shown as N hits. | 2× Steal |
| **Commercial Break** | Skips its move. The rival is Exposed during your turn (+50% Reach). | n/a |
| **Contest** | Rival regains X audience (not above its starting value). | 8 / 12 / 16 |
| **Heavy Rotation** | Rival gains X Lock. | 6 / 9 / 12 |
| **Interference** | Adds N Static or Complaint Calls to your draw or discard pile. | 1–2 / 2–3 / 3 |
| **Trending** | Rival gains Buzz N. | 2 / 2 / 3 |

**Rival audience** *(tune)*: regular 45–55 / 65–80 / 90–105. Prime Time 90 / 130 / 170. Boss 200 / 280 / 360.

**Regular rivals (12, 4 per book).** Each is a show with one clear personality, expressed through its intent pattern. Named so far (the rest go in `docs/design/rivals.md`):
- **Morning Zoo:** chaotic. Heavy on Interference, with mixed small Steals.
- **Smooth Jazz 98.3:** quietly heals. Mostly Contest and Heavy Rotation; a ratings grind.
- **Sports Talk Guys:** hit hard, then burn out. Big Segments followed by Commercial Breaks.
- **Playlist Bot:** copies your genre. Gains Lock equal to your genre streak.

**Prime Time elites (4).** Tougher shows that can appear in any book, scaled by book. Each has one passive rule shown on a placard under its frequency.

**Bosses (one per book).** Each boss's rule is telegraphed from turn 1 on a placard and the wall clock, and each has an answer you could reasonably build (pillar 2). Big Metro adds a second rule to every boss (defined in `docs/design/bosses.md`).

| Book | Boss | Rule | Intended answer |
| --- | --- | --- | --- |
| 1 | Syndicated national morning show | **Satellite Feed:** every 3rd turn is a giant Big Segment, with a countdown on the clock. | Save Loyalty for it, or cash a Drop right before it. |
| 2 | Streaming giant | **Algorithm:** the genre you played most last turn gets −50% Reach this turn (shown on the placard). | Mix genres, use DJ breaks, use Hype. |
| 3 | Corporate format flip | **The Flip:** at turn 5, falling tempo builds Flow instead of rising tempo, and your genre streak stops counting. The countdown shows from turn 1. | Decks that don't only depend on Flow; cue a card for the flip. |

---

## 8. Gear and staff

**Gear.** Unlimited passive effects. Each piece is a physical object on the desk or shelf. Sources: starting gear (1 per format), Remote Broadcasts (3 per run), Prime Time (1 to 2), boss rewards (2), Sponsor Meetings (0 to 2), Station Events.

**Staff.** **3 slots**, shown on the staff board by the booth door. Each staff member has a personality trait; most have a visible upside and a visible downside (e.g. **Old Reliable:** +3 Loyalty every turn. **Loose Cannon:** each turn, 50% chance of +6 Reach on your first song or −3 listeners, shown before you act). Hiring a 4th means letting one go. Sources: mainly Station Events, sometimes Sponsor Meetings.

**Per run:** about 8 to 10 gear and staff in total. v1 pool: about 30 (roughly 22 gear, 8 staff).

---

## 9. Markets and meta progression

**Sponsor Meeting (shop).** Stock: 5 format cards, 2 shared cards, 3 gear, 1 card-cut service. One card or gear item each visit is 40% off. Prices vary ±10%. *(tune)*

| Item | Common | Uncommon | Rare |
| --- | --- | --- | --- |
| Card | 45 | 70 | 140 |
| Gear | 140 | 190 | 260 |

Cutting a card costs 60, plus 20 for each earlier cut this run. A Request token rerolls the stock.

**Markets (difficulty ladder).** Each tier keeps every rule from the tiers before it. Beating market N with any format unlocks market N+1. Each market also has its own town diorama dressing.

| Tier | Market | Added rule |
| --- | --- | --- |
| 1 | Chill Coastal Village | Base rules. The first run has a guided first fight. |
| 2 | College Town | Rival audience +10%. |
| 3 | Blue-Collar Heartland | Dead Air starts at 8 (8 / 13 / 18). |
| 4 | Retiree Coast | Production Room recovers 20% instead of 30%. |
| 5 | True Crime Suburbia | Rivals start each daypart with Buzz 1. |
| 6 | Big Metro | Bosses gain a second telegraphed rule. |

**Meta progression adds options, not power (pillar 3).**
- **Ratings Points** at the end of every run, win or lose *(tune)*: 10 per daypart won, 25 per boss beaten, 5 per book reached.
- **Thresholds** unlock about 15 fixed bundles of 3 to 5 cards, gear or events into the pools.
- **Formats:** Classic Rock from the start. Hip-Hop Heat unlocks the first time you reach book 2. Country Roads unlocks the first time you beat a boss.
- No permanent stat upgrades. A first-time player can win with only the starting content.

---

## 10. UI

Two kinds of view. **Fights use the desk view:** a fixed seat in the booth, where the booth's objects are the HUD. **Everything else is a diorama:** high three-quarter isometric scenes. Every screen works with a controller, with no exceptions. No text below 12 px at 1280×800.

**Desk view (fight).** Layout follows `docs/refs/booth/KEY_booth_*.png`. You are on the left, the rival on the right.

| Booth object | Shows |
| --- | --- |
| Tuner (across the top of the window) | Your frequency on the left (amber dot) and the rival's on the right (teal diamond). The needle drifts toward whoever is winning on share. The rival's intent and any Monitor peeks show above the rival's frequency. |
| Wall clock | Turns remaining in the hour (one tick per turn), and boss countdowns. |
| On-air light (red) | Glows steadily while you're on air. Flickers and dims as a warning when ending the turn now would cause Dead Air, or when the next steal would end the run. The warning state is the danger signal. |
| Corkboard (left wall) | Your statuses as sticky notes: Hype stacks, and Burnout on songs in play. Each note shows an icon and a stack count. |
| Sticky notes by the rival's frequency (right) | The rival's statuses: Buzz, Lock, Exposed. Each note shows an icon and a stack count. |
| Desk mat | Your hand. |
| Left turntable | Cue slot. |
| Right turntable | What the rival has on air (its current show, placard). |
| Mixer VU meters | Flow (left), Hype (center), Loyalty (right). Faders show Airtime remaining. |
| Record crate (left) | Draw pile (count; select to view). |
| Clipboard (right) | Discard pile and a log of this turn's plays. |
| Cash tin | Cash. |
| Shelf and desk props | Gear (select one for its tooltip). |
| Board by the door | Staff (3 slots). |
| Listener and audience readouts | Your listeners at the left end of the tuner and the rival's audience at the right end, as numbers. Static count next to the crate. |

**Other screens (dioramas):**

| Screen | What's on it |
| --- | --- |
| Title | The station on its patch of ground. The tuner is the menu (tune to Continue / New Run / Collection / Settings / Quit). |
| New run | Format choice (starter deck preview, max listeners, signature) and market choice (rule list). |
| Town map | Isometric town for the current book. Nodes are real places with icons and labels. Shows path lines, current position, listeners, Cash, Request tokens, and deck/gear/staff buttons. |
| Reward | Cash gained, 1-of-3 card pick with Skip and Reroll (token), gear if any. |
| Sponsor Meeting | Diner booth with the sponsor. Stock with prices, the sale tag, cut service, Reroll. |
| Production Room | Two choices: Recover (shows the exact amount) or Remaster (shows each card next to its remastered version). |
| Station Event | Diorama of the place, text in a radio-filtered panel, choices with exact costs and odds. |
| Remote Broadcast | Van at the location. Pick 1 of 2 gear. |
| Deck / Collection | Crate browser. Filter by type, genre and tempo; shows remaster previews. |
| Run end | The "ratings book" report: books reached, fights won, Ratings Points earned, unlocks. |
| Settings | Game speed toggle, text size, controller remap, screen shake, colorblind preview, audio mix. |

---

## 11. Look and sound

**Art.** Kind Words 2 spirit (see `docs/refs/style`): chunky, rounded, simplified low-poly with soft bevels and slightly exaggerated proportions. Flat colors with gentle gradients and a few hand-painted accents. Soft light, soft shadows, a little bloom on anything that glows. Take the principles, not the elements: every model, icon and panel is our own.

**Booth.** Warm base materials: walnut, tobacco, aged cream. Outside the window, a city-pop sky over the town. Keys: `docs/refs/booth`.

**One color wash per daypart:**

| Daypart | Wash |
| --- | --- |
| Morning drive | Pale, cool pink and gold. Hazy sky, low sun on the left. |
| Midday | Pale sky blue and lavender. |
| Afternoon drive | Deep warm orange and coral. Low sun on the right, long shadows across the desk. |
| Overnight | Deep blue with warm lamp glow. |

**Color rule (hard requirement).** **You are a steady round amber dot on the left. The rival is a blinking teal diamond on the right.** Every you-versus-rival signal uses all four cues: color, shape, side, and motion (your lights glow steadily, the rival's blink). The rival's teal is dimmer and deeper than your amber, so the two read as different grays. Red means danger only. The on-air light glows steadily while you're on air; its danger signal is the warning state, when it flickers and dims (section 10). Every wash must pass a grayscale check.

The booth keys are mood and layout targets. Where their tuner markers differ from this section, the color rule wins.

**Pipeline.** Every prop maps its faces to one shared palette texture, so Tripo exports match automatically and a recolor is a single image edit. Animation is mechanical only: platters, faders, needles, the tuner. Townsfolk are simple, chunky figures with idle loops at most.

**Sound.** Short original loops per format; no licensed music. DJ chatter is on-screen text with a radio filter; no voice acting. Foley is all hardware: needle drops, fader clicks, tuner static, the on-air relay. Where the music comes from is still open (see the end of this brief).

---

## 12. v1 content counts and out of scope

| Content | v1 target |
| --- | --- |
| Formats | 3 (Classic Rock, Hip-Hop Heat, Country Roads) |
| Cards | About 30 per format plus 30 shared, so roughly 120 |
| Gear and staff | About 30 |
| Rivals | 12 regular, 4 Prime Time elites |
| Bosses | 3 (one per book) |
| Station Events | About 20 |
| Markets | 6 difficulty tiers |

**Out of scope for v1:** multiplayer, voice acting, licensed music, character animation, the Talk Radio format, an explorable 3D world, and a story campaign.

Anything not in this brief is out of scope until it is added here.

**Steam:** Steam Deck verified, full controller support, achievements, cloud saves. Coming Soon page early; demo for Steam Next Fest. Price target $14.99 to $19.99.

---

## 13. Design pillars

1. **Randomness you can see and play around.** Rival moves are always shown. Map nodes are labeled. Tools to bend luck: rerolls, peeks, holding a card.
2. **Every boss has an answer.** Boss rules are telegraphed, and every one can be countered by a deck you could reasonably build.
3. **Unlocks add options, not power.** Fun from the first run, no grind walls.
4. **Familiar structure, new core.** Keep the map, shops, events, and relics players expect. The radio broadcast is what's new.
5. **Readable everywhere.** Big card text, a tooltip on every keyword, a speed toggle, full controller support, Steam Deck verified at launch.
6. **Broken combos stay broken.** Balance by adding counters and content, not by nerfing what players love after launch.
7. **8+ hours before you've seen it all.** All content is data, so it's cheap to add.

---

### Still open (not blocking a first playable)
- **Music source and budget.**
- **Steam subtitle and trademark search.**
- **Design docs to write:** `docs/design/rivals.md` (12 + 4 patterns), `docs/design/bosses.md` (Big Metro second rules), and the full card, gear, staff and event lists.
