using System.Collections.Generic;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;

namespace TurnTheDial.Core.Tests
{
    /// <summary>
    /// Test fixtures with the exact numbers from GAME_BRIEF.md and the design docs. Test-only
    /// cards and rivals live here, never as content.
    /// </summary>
    public static class TestContent
    {
        /// <summary>Every tunable at its design-doc starting value.</summary>
        public static BalanceConfig Balance()
        {
            return new BalanceConfig
            {
                TurnsDaypart = 6,
                TurnsPrimeTime = 7,
                TurnsBoss = 8,
                AirtimePerTurn = 3,
                DrawPerTurn = 5,
                HandLimit = 10,
                ListenerConversionPercent = 10,
                ExposedPercent = 150,
                DeadAirSteps = new[] { 5, 10, 15 },
                RatingsLossPenaltyPercent = 10,
                CueSlots = 1,
                FlowCap = 10,
                FlowCapLoop = 15,
                FlowReachPerStackPercent = 10,
                FlowRampGain = 1,
                FlowStreakGain = 1,
                HypeDecayPerTurn = 1,
                CashStart = 50,
                RequestTokensStart = 2,
                BuzzStealBonusPercent = 50,
                BurnoutPerStackPercent = 25,
                BurnoutFloorPercent = 25,
                AnthemReachPercent = 150,
                AnthemLoyaltyPerTempo = 2,
                MailbagConversionPercent = 20,
            };
        }

        public static CardData Song(string id, string name, Genre genre, int tempo, int cost, int reach, int drop = 0, bool anthem = false, Rarity rarity = Rarity.Starter, CardPool pool = CardPool.ClassicRock)
        {
            return new CardData
            {
                Id = id,
                Name = name,
                Type = CardType.Song,
                Genre = genre,
                Tempo = tempo,
                Cost = cost,
                Reach = reach,
                Rarity = rarity,
                Pool = pool,
                Keywords = new CardKeywords { OnAir = true, Drop = drop, Anthem = anthem },
            };
        }

        public static CardData WithUpgrade(CardData card, System.Action<CardData> change)
        {
            var up = new CardData
            {
                Id = card.Id,
                Name = card.Name,
                Type = card.Type,
                Genre = card.Genre,
                Tempo = card.Tempo,
                Cost = card.Cost,
                Reach = card.Reach,
                Loyalty = card.Loyalty,
                Hype = card.Hype,
                Cash = card.Cash,
                ListenerCost = card.ListenerCost,
                Rarity = card.Rarity,
                Pool = card.Pool,
                Keywords = card.Keywords.Clone(),
                Effects = new List<EffectData>(card.Effects),
                Triggers = new List<CardTrigger>(card.Triggers),
                IsRemastered = true,
            };
            change(up);
            card.Upgrade = up;
            return card;
        }

        // Classic Rock starter (brief §5).
        public static CardData AlbumCut() => WithUpgrade(Song("card.cr.album_cut", "Album Cut", Genre.Rock, 3, 1, 6), u => u.Reach = 9);

        public static CardData PowerBallad() => WithUpgrade(Song("card.cr.power_ballad", "Power Ballad", Genre.Rock, 2, 2, 9, anthem: true), u => u.Reach = 13);

        public static CardData RequestLine() => WithUpgrade(new CardData
        {
            Id = "card.cr.request_line", Name = "Request Line", Type = CardType.DjBreak, Cost = 1, Loyalty = 5,
            Rarity = Rarity.Starter, Pool = CardPool.ClassicRock, Keywords = new CardKeywords { OnAir = true },
        }, u => u.Loyalty = 8);

        public static CardData StationId() => WithUpgrade(new CardData
        {
            Id = "card.sh.station_id", Name = "Station ID", Type = CardType.Liner, Cost = 0,
            Rarity = Rarity.Starter, Pool = CardPool.Shared, Keywords = new CardKeywords(),
            Effects = new List<EffectData> { new EffectData(EffectKind.DrawCards, 1) },
        }, u => u.Effects = new List<EffectData> { new EffectData(EffectKind.DrawCards, 2) });

        public static CardData CarDealerSpot() => WithUpgrade(new CardData
        {
            Id = "card.cr.car_dealer_spot", Name = "Car Dealer Spot", Type = CardType.Ad, Cost = 0, Cash = 25, ListenerCost = 4,
            Rarity = Rarity.Starter, Pool = CardPool.ClassicRock, Keywords = new CardKeywords(),
        }, u => u.Cash = 35);

        // Hip-Hop Heat starter (brief §5).
        public static CardData CornerCypher() => Song("card.hh.corner_cypher", "Corner Cypher", Genre.HipHop, 1, 1, 5, pool: CardPool.HipHopHeat);
        public static CardData BlockParty() => Song("card.hh.block_party", "Block Party", Genre.HipHop, 2, 1, 5, pool: CardPool.HipHopHeat);
        public static CardData BasementSession() => Song("card.hh.basement_session", "Basement Session", Genre.HipHop, 3, 1, 5, pool: CardPool.HipHopHeat);
        public static CardData RooftopJam() => Song("card.hh.rooftop_jam", "Rooftop Jam", Genre.HipHop, 4, 1, 5, pool: CardPool.HipHopHeat);
        public static CardData Banger() => Song("card.hh.banger", "Banger", Genre.HipHop, 5, 2, 7, drop: 3, pool: CardPool.HipHopHeat);

        public static CardData HypeMan() => new CardData
        {
            Id = "card.hh.hype_man", Name = "Hype Man", Type = CardType.DjBreak, Cost = 1, Reach = 3,
            Rarity = Rarity.Starter, Pool = CardPool.HipHopHeat, Keywords = new CardKeywords { OnAir = true, Drop = 2 },
        };

        public static CardData AirHorn() => new CardData
        {
            Id = "card.hh.air_horn", Name = "Air Horn", Type = CardType.Liner, Cost = 0,
            Rarity = Rarity.Starter, Pool = CardPool.HipHopHeat, Keywords = new CardKeywords(),
            Effects = new List<EffectData> { new EffectData(EffectKind.NextSongCountsAsRamp, 0) },
        };

        public static CardData ListenerMail() => new CardData
        {
            Id = "card.cc.listener_mail", Name = "Listener Mail", Type = CardType.DjBreak, Cost = 1, Loyalty = 4, Hype = 1,
            Rarity = Rarity.Starter, Pool = CardPool.CountryRoads, Keywords = new CardKeywords { OnAir = true },
        };

        public static CardData BackPorchTune() => Song("card.cc.back_porch_tune", "Back Porch Tune", Genre.Country, 2, 1, 6, pool: CardPool.CountryRoads);

        // Interference (brief §3, statuses.md).
        public static CardData Static() => new CardData
        {
            Id = "card.int.static", Name = "Static", Type = CardType.Interference, Cost = 0,
            Rarity = Rarity.Special, Pool = CardPool.Rival, Keywords = new CardKeywords { Unplayable = true },
        };

        public static CardData ComplaintCall() => new CardData
        {
            Id = "card.int.complaint_call", Name = "Complaint Call", Type = CardType.Interference, Cost = 1,
            Rarity = Rarity.Special, Pool = CardPool.Rival, Keywords = new CardKeywords { Pulled = true },
            Triggers = new List<CardTrigger> { new CardTrigger(CardTriggerKind.EndOfTurnInHand, new List<EffectData> { new EffectData(EffectKind.LoseListeners, 3) }) },
        };

        // Test-only cards.
        public static CardData TestLiner(string id = "test.liner") => new CardData
        {
            Id = id, Name = "Test Liner", Type = CardType.Liner, Cost = 0, Rarity = Rarity.Common, Pool = CardPool.Shared, Keywords = new CardKeywords(),
        };

        public static CardData TestStickyStatic() => new CardData
        {
            Id = "test.sticky_static", Name = "Sticky Static", Type = CardType.Interference, Cost = 0,
            Rarity = Rarity.Special, Pool = CardPool.Rival, Keywords = new CardKeywords { Unplayable = true, Sticky = true },
        };

        public static CardData TestEffectCard(string name, CardType type, int cost, params EffectData[] effects) => new CardData
        {
            Id = "test." + name.ToLowerInvariant().Replace(' ', '_'), Name = name, Type = type, Cost = cost, Rarity = Rarity.Common, Pool = CardPool.Shared,
            Keywords = new CardKeywords { OnAir = type == CardType.DjBreak || type == CardType.Song },
            Effects = new List<EffectData>(effects),
        };

        public static GearData RibbonMic() => new GearData
        {
            Id = "gear.ribbon_mic", Name = "Ribbon Mic", Trigger = GearTrigger.FirstCardOfTypeEachTurn, TriggerCardType = CardType.DjBreak,
            Effects = new List<EffectData> { new EffectData(EffectKind.GainLoyalty, 3) },
        };

        public static FormatData ClassicRock() => new FormatData
        {
            Id = "format.classic_rock", Name = "Classic Rock", MaxListeners = 85, Genre = Genre.Rock, Signature = Signature.Anthem, StartingGearId = "gear.ribbon_mic",
            StartingDeck = new List<DeckEntry>
            {
                new DeckEntry("card.cr.album_cut", 4), new DeckEntry("card.cr.request_line", 3), new DeckEntry("card.sh.station_id", 1),
                new DeckEntry("card.cr.car_dealer_spot", 1), new DeckEntry("card.cr.power_ballad", 1),
            },
        };

        public static FormatData HipHopHeat() => new FormatData { Id = "format.hip_hop_heat", Name = "Hip-Hop Heat", MaxListeners = 70, Genre = Genre.HipHop, Signature = Signature.Loop };

        public static FormatData CountryRoads() => new FormatData { Id = "format.country_roads", Name = "Country Roads", MaxListeners = 78, Genre = Genre.Country, Signature = Signature.Mailbag };

        /// <summary>Morning Zoo (rivals.md, book 1, audience 50).</summary>
        public static RivalData MorningZoo() => new RivalData
        {
            Id = "rival.morning_zoo", Name = "Morning Zoo", StartingAudience = 50,
            Pattern = new List<IntentData>
            {
                new IntentData(IntentKind.Interference, 2, cardId: "card.int.static", pile: PileKind.Draw),
                new IntentData(IntentKind.Steal, 6),
                new IntentData(IntentKind.Interference, 1, cardId: "card.int.complaint_call", pile: PileKind.Discard),
                new IntentData(IntentKind.Steal, 7),
                new IntentData(IntentKind.Interference, 2, cardId: "card.int.static", pile: PileKind.Draw),
                new IntentData(IntentKind.BigSegment, 6, hits: 2),
            },
        };

        /// <summary>A test-only rival with a given pattern (e.g. Commercial Break for Exposed tests).</summary>
        public static RivalData TestRival(int audience, params IntentData[] pattern) => new RivalData
        {
            Id = "test.rival", Name = "Test Rival", StartingAudience = audience, Pattern = new List<IntentData>(pattern),
        };

        /// <summary>A rival that does nothing harmful: Contest 0 every turn.</summary>
        public static RivalData QuietRival(int audience = 50) => TestRival(audience, new IntentData(IntentKind.Contest, 0));

        public static CardCatalog Catalog()
        {
            var catalog = new CardCatalog();
            CardData[] all =
            {
                AlbumCut(), PowerBallad(), RequestLine(), StationId(), CarDealerSpot(), CornerCypher(), BlockParty(), BasementSession(),
                RooftopJam(), Banger(), HypeMan(), AirHorn(), ListenerMail(), BackPorchTune(), Static(), ComplaintCall(), TestLiner(), TestStickyStatic(),
            };
            for (int i = 0; i < all.Length; i++) catalog.Add(all[i]);
            return catalog;
        }

        public static List<CardData> ClassicRockDeck()
        {
            return Catalog().BuildDeck(ClassicRock());
        }

        public static Fight NewFight(FormatData format = null, RivalData rival = null, List<CardData> deck = null, ulong seed = 1,
            FightKind kind = FightKind.Daypart, List<GearData> gear = null, MarketRules market = null, int? listeners = null)
        {
            var setup = new FightSetup
            {
                Balance = Balance(),
                Format = format ?? ClassicRock(),
                Rival = rival ?? QuietRival(),
                Kind = kind,
                Catalog = Catalog(),
                Deck = deck ?? new List<CardData>(),
                Gear = gear ?? new List<GearData>(),
                Market = market,
                Rng = new RngStreams(seed),
                Listeners = listeners,
            };
            return new Fight(setup);
        }

        /// <summary>A started fight with an empty deck: the hand is empty, so tests place exact cards.</summary>
        public static Fight Started(FormatData format = null, RivalData rival = null, FightKind kind = FightKind.Daypart, List<GearData> gear = null, MarketRules market = null, int? listeners = null)
        {
            Fight f = NewFight(format, rival, null, 1, kind, gear, market, listeners);
            f.Start();
            return f;
        }

        public static CardInstance FindInHand(Fight f, string name)
        {
            for (int i = 0; i < f.Hand.Count; i++)
            {
                if (f.Hand[i].Card.Name == name) return f.Hand[i];
            }

            return null;
        }

        public static string LastLine(Fight f) => f.Log.Lines[f.Log.Lines.Count - 1];

        public static bool LogContains(Fight f, string text)
        {
            for (int i = 0; i < f.Log.Lines.Count; i++)
            {
                if (f.Log.Lines[i].Contains(text)) return true;
            }

            return false;
        }
    }
}
