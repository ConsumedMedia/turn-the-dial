using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Sim;
using TurnTheDial.Data.Editor;
using UnityEditor;

namespace TurnTheDial.Data.Tests
{
    /// <summary>The authored assets match GAME_BRIEF.md §5, formats.md, rivals.md and every Tunable values table.</summary>
    public class ContentAssetTests
    {
        private static FightContent Content()
        {
            FightContent c = AssetDatabase.LoadAssetAtPath<FightContent>(SimRunner.ContentPath);
            Assert.IsNotNull(c, SimRunner.ContentPath);
            return c;
        }

        private static CardCatalog Catalog() => ContentExporter.Catalog(Content().cards);

        [Test]
        public void Content_PassesValidator()
        {
            CollectionAssert.IsEmpty(ContentExporter.Validate(Content()));
        }

        [Test]
        public void UiConfig_MatchesUiMdTunables()
        {
            const string path = "Assets/TurnTheDial/Content/UI/UiConfig.asset";
            UiConfigAsset ui = AssetDatabase.LoadAssetAtPath<UiConfigAsset>(path);
            Assert.IsNotNull(ui, path);
            Assert.AreEqual(2f, ui.onAirFlickerHz, "ui.onAirFlickerHz");
            Assert.Less(ui.onAirFlickerHz, 3f, "under 3 flashes per second");
            Assert.AreEqual(50, ui.logLines, "ui.logLines");
            Assert.AreEqual(1f, ui.rivalPulseHz, "ui.rivalPulseHz");
            Assert.AreEqual(1f, ui.rivalBlinkHz, "ui.rivalBlinkHz");
            Assert.AreEqual(12, ui.minTextPx, "ui.minTextPx");
            Assert.IsFalse(ui.reduceMotion, "reduce motion defaults off");
        }

        [Test]
        public void Balance_MatchesDesignDocStartingValues()
        {
            BalanceConfig b = Content().balance.ToCore();
            Assert.AreEqual(6, b.TurnsDaypart);
            Assert.AreEqual(7, b.TurnsPrimeTime);
            Assert.AreEqual(8, b.TurnsBoss);
            Assert.AreEqual(3, b.AirtimePerTurn);
            Assert.AreEqual(5, b.DrawPerTurn);
            Assert.AreEqual(10, b.HandLimit);
            Assert.AreEqual(10, b.ListenerConversionPercent);
            Assert.AreEqual(150, b.ExposedPercent);
            CollectionAssert.AreEqual(new[] { 5, 10, 15 }, b.DeadAirSteps);
            Assert.AreEqual(10, b.RatingsLossPenaltyPercent);
            Assert.AreEqual(1, b.CueSlots);
            Assert.AreEqual(10, b.FlowCap);
            Assert.AreEqual(15, b.FlowCapLoop);
            Assert.AreEqual(10, b.FlowReachPerStackPercent);
            Assert.AreEqual(1, b.FlowRampGain);
            Assert.AreEqual(1, b.FlowStreakGain);
            Assert.AreEqual(1, b.HypeDecayPerTurn);
            Assert.AreEqual(50, b.CashStart);
            Assert.AreEqual(2, b.RequestTokensStart);
            Assert.AreEqual(50, b.BuzzStealBonusPercent);
            Assert.AreEqual(25, b.BurnoutPerStackPercent);
            Assert.AreEqual(25, b.BurnoutFloorPercent);
            Assert.AreEqual(150, b.AnthemReachPercent);
            Assert.AreEqual(2, b.AnthemLoyaltyPerTempo);
            Assert.AreEqual(20, b.MailbagConversionPercent);
        }

        [Test]
        public void ClassicRock_FormatAndStartingDeck_MatchBriefTable()
        {
            FormatData f = ContentExporter.ToCore(Content().format);
            Assert.AreEqual("format.classic_rock", f.Id);
            Assert.AreEqual(85, f.MaxListeners);
            Assert.AreEqual(Genre.Rock, f.Genre);
            Assert.AreEqual(Signature.Anthem, f.Signature);
            Assert.AreEqual("gear.ribbon_mic", f.StartingGearId);
            List<CardData> deck = Catalog().BuildDeck(f);
            Assert.AreEqual(10, deck.Count);
            Assert.AreEqual(4, deck.FindAll(c => c.Id == "card.cr.album_cut").Count);
            Assert.AreEqual(3, deck.FindAll(c => c.Id == "card.cr.request_line").Count);
            Assert.AreEqual(1, deck.FindAll(c => c.Id == "card.sh.station_id").Count);
            Assert.AreEqual(1, deck.FindAll(c => c.Id == "card.cr.car_dealer_spot").Count);
            Assert.AreEqual(1, deck.FindAll(c => c.Id == "card.cr.power_ballad").Count);
        }

        [Test]
        public void StarterCards_ExactNumbers_AndRemasters()
        {
            CardCatalog cat = Catalog();
            CardData album = cat.Get("card.cr.album_cut");
            Assert.AreEqual(("Album Cut", CardType.Song, Genre.Rock, 3, 1, 6), (album.Name, album.Type, album.Genre, album.Tempo, album.Cost, album.Reach));
            Assert.IsTrue(album.Keywords.OnAir);
            Assert.AreEqual(9, album.Upgrade.Reach);
            Assert.AreEqual(3, album.Upgrade.Tempo);

            CardData req = cat.Get("card.cr.request_line");
            Assert.AreEqual((CardType.DjBreak, 1, 5), (req.Type, req.Cost, req.Loyalty));
            Assert.AreEqual(8, req.Upgrade.Loyalty);

            CardData sid = cat.Get("card.sh.station_id");
            Assert.AreEqual((CardType.Liner, 0, CardPool.Shared), (sid.Type, sid.Cost, sid.Pool));
            Assert.AreEqual(EffectKind.DrawCards, sid.Effects[0].Kind);
            Assert.AreEqual(1, sid.Effects[0].Amount);
            Assert.AreEqual(2, sid.Upgrade.Effects[0].Amount);

            CardData car = cat.Get("card.cr.car_dealer_spot");
            Assert.AreEqual((CardType.Ad, 0, 25, 4), (car.Type, car.Cost, car.Cash, car.ListenerCost));
            Assert.AreEqual(35, car.Upgrade.Cash);
            Assert.AreEqual(4, car.Upgrade.ListenerCost, "Proposed (formats.md): RM keeps fields its bracket doesn't mention");

            CardData pb = cat.Get("card.cr.power_ballad");
            Assert.AreEqual((CardType.Song, Genre.Rock, 2, 2, 9), (pb.Type, pb.Genre, pb.Tempo, pb.Cost, pb.Reach));
            Assert.IsTrue(pb.Keywords.Anthem);
            Assert.AreEqual(13, pb.Upgrade.Reach);
            Assert.IsTrue(pb.Upgrade.Keywords.Anthem);

            foreach (CardData c in new[] { album, req, sid, car, pb })
            {
                Assert.AreEqual(Rarity.Starter, c.Rarity, c.Id);
                Assert.IsTrue(c.Upgrade.IsRemastered, c.Id);
                Assert.AreEqual(c.Name, c.Upgrade.Name, c.Id);
            }
        }

        [Test]
        public void Interference_StaticAndComplaintCall()
        {
            CardCatalog cat = Catalog();
            CardData s = cat.Get("card.int.static");
            Assert.AreEqual((CardType.Interference, CardPool.Rival, Rarity.Special), (s.Type, s.Pool, s.Rarity));
            Assert.IsTrue(s.Keywords.Unplayable);
            Assert.IsNull(s.Upgrade);

            CardData cc = cat.Get("card.int.complaint_call");
            Assert.AreEqual((CardType.Interference, 1, CardPool.Rival), (cc.Type, cc.Cost, cc.Pool));
            Assert.IsTrue(cc.Keywords.Pulled);
            Assert.AreEqual(1, cc.Triggers.Count);
            Assert.AreEqual(CardTriggerKind.EndOfTurnInHand, cc.Triggers[0].Kind);
            Assert.AreEqual(EffectKind.LoseListeners, cc.Triggers[0].Effects[0].Kind);
            Assert.AreEqual(3, cc.Triggers[0].Effects[0].Amount);
            Assert.IsFalse(cc.Keywords.OnAir);
        }

        [Test]
        public void RibbonMic_FirstDjBreakEachTurn_Plus3Loyalty()
        {
            GearData g = ContentExporter.ToCore(Content().format.startingGear);
            Assert.AreEqual("gear.ribbon_mic", g.Id);
            Assert.AreEqual(GearTrigger.FirstCardOfTypeEachTurn, g.Trigger);
            Assert.AreEqual(CardType.DjBreak, g.TriggerCardType);
            Assert.AreEqual(EffectKind.GainLoyalty, g.Effects[0].Kind);
            Assert.AreEqual(3, g.Effects[0].Amount);
        }

        [Test]
        public void MorningZoo_PatternMatchesRivalsDoc()
        {
            RivalData r = ContentExporter.ToCore(Content().rival);
            Assert.AreEqual("rival.morning_zoo", r.Id);
            Assert.AreEqual(50, r.StartingAudience);
            Assert.AreEqual(6, r.Pattern.Count);
            Assert.AreEqual((IntentKind.Interference, 2, "card.int.static", PileKind.Draw), (r.Pattern[0].Kind, r.Pattern[0].Amount, r.Pattern[0].CardId, r.Pattern[0].Pile));
            Assert.AreEqual((IntentKind.Steal, 6), (r.Pattern[1].Kind, r.Pattern[1].Amount));
            Assert.AreEqual((IntentKind.Interference, 1, "card.int.complaint_call", PileKind.Discard), (r.Pattern[2].Kind, r.Pattern[2].Amount, r.Pattern[2].CardId, r.Pattern[2].Pile));
            Assert.AreEqual((IntentKind.Steal, 7), (r.Pattern[3].Kind, r.Pattern[3].Amount));
            Assert.AreEqual((IntentKind.Interference, 2, "card.int.static", PileKind.Draw), (r.Pattern[4].Kind, r.Pattern[4].Amount, r.Pattern[4].CardId, r.Pattern[4].Pile));
            Assert.AreEqual((IntentKind.BigSegment, 6, 2), (r.Pattern[5].Kind, r.Pattern[5].Amount, r.Pattern[5].Hits));
        }

        [Test]
        public void Ids_MatchContentTracker()
        {
            var ids = new List<string>();
            foreach (CardDefinition c in Content().cards) ids.Add(c.id);
            CollectionAssert.AreEquivalent(new[]
            {
                "card.cr.album_cut", "card.cr.request_line", "card.sh.station_id", "card.cr.car_dealer_spot",
                "card.cr.power_ballad", "card.int.static", "card.int.complaint_call",
            }, ids);
        }

        [Test]
        [Category("Sim")]
        public void Sim_ProtoCombat_FiveSeeds_BuildsReport()
        {
            // Never writes into the repo: tools/sim/reports/ is written only from the menu.
            string report = SimRunner.BuildReport(SimRunner.DefaultFirstSeed, SimRunner.DefaultSeedCount, out List<SimFightResult> results, out string fileName);
            StringAssert.StartsWith("# Proto combat sim: Classic Rock vs Morning Zoo", report);
            StringAssert.Contains("| 1 | 6 |", report);
            StringAssert.Contains("## Log, seed 1", report);
            Assert.AreEqual("proto_combat_rival_morning_zoo_seeds_1_x5.md", fileName);
            Assert.AreEqual(5, results.Count);
            foreach (SimFightResult r in results)
            {
                Assert.AreNotEqual(TurnTheDial.Core.Combat.FightOutcome.None, r.Outcome);
            }
        }
    }
}
