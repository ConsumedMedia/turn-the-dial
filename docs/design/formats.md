# Formats

Source: GAME_BRIEF.md sections 5, 9. The brief wins if this doc disagrees.

## Purpose
Formats are the characters: each has a max listener count, a signature rule, a starting gear piece
and a 10-card starting deck. Talk Radio is out of scope for v1.

## Rules

| | Classic Rock | Hip-Hop Heat | Country Roads |
| --- | --- | --- | --- |
| Max listeners | 85 | 70 | 78 |
| Genre | Rock | Hip-Hop | Country |
| Signature | Anthem | Loop | Mailbag |
| Starting gear | Ribbon Mic | Sampler | Mail Sack |
| Unlock | From the start | First time you reach book 2 | First time you beat a boss |

You start a run at full listeners (= max).

### Signatures
- **Anthem (Classic Rock):** the first Song you play each turn gets +50% Reach (multiplied after the
  Flow multiplier) and gives Loyalty equal to its tempo × 2. Only applies to cards with the Anthem keyword.
- **Loop (Hip-Hop Heat):** Flow cap 15. A tempo 1 song right after a tempo 5 song counts as a ramp.
- **Mailbag (Country Roads):** Reach converts to listeners at 20% instead of 10%. Hype doesn't drop at
  end of turn if you played a DJ Break that turn.

### Starting gear
- **Ribbon Mic:** your first DJ Break each turn gives +3 Loyalty.
- **Sampler:** the first time Flow reaches 5 in a daypart, draw 2.
- **Mail Sack:** after winning a daypart, recover 4 listeners.

### Starting decks *(tune)* — RM values in brackets

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
| 4 | Block Party | Song · Hip-Hop · T1/T2/T3/T4 (one each) | 1 | 5 Reach [7] |
| 2 | Shout-Out | DJ Break | 1 | 4 Loyalty [7] |
| 1 | Hype Man | DJ Break · Drop 2 | 1 | 3 Reach + Drop 2 [Drop 3] |
| 1 | Air Horn | Liner | 0 | Next Song +1 tempo for the ramp [and draw 1] |
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

Remastered Ads keep their listener cost; remastered Listener Mail keeps +1 Hype; remastered Hype Man
keeps 3 Reach.

## Tunable values
All numbers in the tables above, plus:
| Key | Start |
| --- | --- |
| `anthem.reachFactor` | 1.5 |
| `anthem.loyaltyPerTempo` | 2 |
| `loop.flowCap` | 15 |
| `mailbag.conversion` | 0.20 |
| `ribbonMic.loyalty` | 3 |
| `sampler.flowThreshold / draw` | 5 / 2 |
| `mailSack.recover` | 4 |

## Edge cases
- Anthem: if the first Song of the turn has no Anthem keyword, no Anthem bonus that turn (see open question).
- Sampler: Flow reaching 5 via a jump (4 → 6) counts as reaching 5. Triggers once per daypart.
- Sampler draw 2 with a full hand: extras go to discard.
- Mail Sack at max listeners: no effect.
- Block Party: four cards share a name, so playing two in a daypart adds Burnout.
- Loop + Air Horn: tempo 5 + 1 = 6 for the ramp check; a T1 after it still counts as a ramp.

## Open questions (ask before building)
- **Anthem:** "the first Song you play each turn" + "only applies to cards with Anthem". Does a
  non-Anthem first song use up the bonus (proposed), or does the first *Anthem* song get it?
- **Mail Sack:** "after winning a daypart" — does it include Prime Time and boss wins? Proposed: yes, any fight.
- **Remastered Ads:** the brief lists only the Cash change; confirm listener cost is unchanged.
- **Block Party Burnout:** intended, or should each tempo have its own name?

## Done checklist
- [ ] Format data assets for all three, exported to Core
- [ ] Each signature tested, including the edge cases above
- [ ] Each starting gear tested
- [ ] Starting decks match these tables exactly (data validation test)
- [ ] Unlock rules wired to `meta.md`
- [ ] Reviewer sign-off
