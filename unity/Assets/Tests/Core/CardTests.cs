using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>cards.md: validator, rules text, effect library, keywords, effect order.</summary>
    public class CardTests
    {
        // ------------------------------------------------------------ validator

        [Test]
        public void Validator_AllStarterAndInterferenceCardsValid()
        {
            foreach (CardData c in Catalog().Cards)
            {
                if (c.Id.StartsWith("test.")) continue;
                if (c.Pool == CardPool.HipHopHeat || c.Pool == CardPool.CountryRoads) continue; // fixtures without upgrades
                CollectionAssert.IsEmpty(ContentValidator.ValidateCard(c), c.Id);
            }
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void Validator_CostOutside0To3_Fails(int cost)
        {
            CardData c = AlbumCut();
            c.Cost = cost;
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
        }

        [TestCase(0)]
        [TestCase(6)]
        public void Validator_SongTempoOutside1To5_Fails(int tempo)
        {
            CardData c = AlbumCut();
            c.Tempo = tempo;
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
        }

        [Test]
        public void Validator_MissingUpgrade_FailsUnlessInterference()
        {
            CardData c = AlbumCut();
            c.Upgrade = null;
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
            CollectionAssert.IsEmpty(ContentValidator.ValidateCard(Static()));
        }

        [Test]
        public void Validator_InterferenceMustBeRivalPool()
        {
            CardData s = Static();
            s.Pool = CardPool.Shared;
            Assert.IsNotEmpty(ContentValidator.ValidateCard(s));
        }

        [Test]
        public void Validator_MissingId_Fails()
        {
            CardData c = AlbumCut();
            c.Id = "";
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
        }

        [Test]
        public void Validator_UnknownEffect_Fails_NoDropEffectExists()
        {
            Assert.IsFalse(System.Enum.IsDefined(typeof(EffectKind), "Drop"), "Drop is a keyword, never an effect");
            CardData c = StationId();
            c.Effects.Add(new EffectData((EffectKind)99, 1));
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
        }

        [Test]
        public void Validator_UnknownTrigger_Fails()
        {
            CardData c = ComplaintCall();
            c.Triggers.Add(new CardTrigger((CardTriggerKind)99, new List<EffectData>()));
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
            CollectionAssert.IsEmpty(ContentValidator.ValidateCard(ComplaintCall()));
        }

        [Test]
        public void Validator_UpgradeMustKeepName()
        {
            CardData c = AlbumCut();
            c.Upgrade.Name = "Album Cut Deluxe";
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
        }

        [Test]
        public void Validator_LinerCost2_Fails()
        {
            CardData c = StationId();
            c.Cost = 2;
            Assert.IsNotEmpty(ContentValidator.ValidateCard(c));
        }

        [Test]
        public void Validator_GearGainLoyaltyOnDaypartStart_Rejected()
        {
            var g = new GearData
            {
                Id = "test.gear", Name = "Bad Gear", Trigger = GearTrigger.DaypartStart,
                Effects = new List<EffectData> { new EffectData(EffectKind.GainLoyalty, 3) },
            };
            Assert.IsNotEmpty(ContentValidator.ValidateGear(g));
            CollectionAssert.IsEmpty(ContentValidator.ValidateGear(RibbonMic()));
        }

        [Test]
        public void Validator_RivalInterferenceCardMustExist()
        {
            CollectionAssert.IsEmpty(ContentValidator.ValidateRival(MorningZoo(), Catalog()));
            RivalData bad = TestRival(50, new IntentData(IntentKind.Interference, 1, cardId: "card.int.nope"));
            Assert.IsNotEmpty(ContentValidator.ValidateRival(bad, Catalog()));
        }

        // ------------------------------------------------------------ rules text

        [Test]
        public void RulesText_GeneratedForEveryStarterCard()
        {
            Assert.AreEqual("On Air. 6 Reach.", CardTextGenerator.RulesText(AlbumCut()));
            Assert.AreEqual("On Air. 9 Reach.", CardTextGenerator.RulesText(AlbumCut().Upgrade));
            Assert.AreEqual("On Air. 5 Loyalty.", CardTextGenerator.RulesText(RequestLine()));
            Assert.AreEqual("On Air. 8 Loyalty.", CardTextGenerator.RulesText(RequestLine().Upgrade));
            Assert.AreEqual("Draw 1.", CardTextGenerator.RulesText(StationId()));
            Assert.AreEqual("Draw 2.", CardTextGenerator.RulesText(StationId().Upgrade));
            Assert.AreEqual("+25 Cash. Lose 4 listeners.", CardTextGenerator.RulesText(CarDealerSpot()));
            Assert.AreEqual("+35 Cash. Lose 4 listeners.", CardTextGenerator.RulesText(CarDealerSpot().Upgrade));
            Assert.AreEqual("On Air. Anthem. 9 Reach.", CardTextGenerator.RulesText(PowerBallad()));
            Assert.AreEqual("On Air. Anthem. 13 Reach.", CardTextGenerator.RulesText(PowerBallad().Upgrade));
            Assert.AreEqual("Unplayable.", CardTextGenerator.RulesText(Static()));
            Assert.AreEqual("Pulled. In hand at end of turn: lose 3 listeners.", CardTextGenerator.RulesText(ComplaintCall()));
            Assert.AreEqual("On Air. 3 Reach + Drop 2.", CardTextGenerator.RulesText(HypeMan()));
            Assert.AreEqual("Your next Song counts as a rising ramp.", CardTextGenerator.RulesText(AirHorn()));
            Assert.AreEqual("Song · Rock · T3", CardTextGenerator.TypeLine(AlbumCut()));
        }

        // ------------------------------------------------------------ effect library

        [Test]
        public void Effect_DrawCards()
        {
            Fight f = Started();
            f.TestPutOnDraw(AlbumCut());
            f.TestPutOnDraw(AlbumCut());
            Play(f, StationId().Upgrade);
            Assert.AreEqual(2, f.Hand.Count);
        }

        [Test]
        public void Effect_GainFlow_ClampedToCap()
        {
            Fight f = Started();
            Play(f, TestEffectCard("Flow Card", CardType.Liner, 0, new EffectData(EffectKind.GainFlow, 3)));
            Assert.AreEqual(3, f.Flow);
            f.TestSetFlow(9);
            Play(f, TestEffectCard("Flow Card", CardType.Liner, 0, new EffectData(EffectKind.GainFlow, 3)));
            Assert.AreEqual(10, f.Flow);
        }

        [Test]
        public void Effect_GainHype_GainLoyalty_GainCash_LoseListeners()
        {
            Fight f = Started();
            Play(f, TestEffectCard("Multi", CardType.Liner, 0,
                new EffectData(EffectKind.GainHype, 2), new EffectData(EffectKind.GainLoyalty, 4),
                new EffectData(EffectKind.GainCash, 10), new EffectData(EffectKind.LoseListeners, 3)));
            Assert.AreEqual(2, f.Hype);
            Assert.AreEqual(4, f.Loyalty);
            Assert.AreEqual(60, f.Cash);
            Assert.AreEqual(82, f.Listeners);
        }

        [Test]
        public void Effect_MonitorDraw_FewerThanN_ShowsAll_NoReshuffle()
        {
            Fight f = Started();
            f.TestPutInDiscard(AlbumCut());
            f.TestPutOnDraw(PowerBallad());
            IReadOnlyList<CardInstance> shown = null;
            f.DrawMonitored += top => shown = top;
            Play(f, TestEffectCard("Peek Draw", CardType.Liner, 0, new EffectData(EffectKind.MonitorDraw, 3)));
            Assert.AreEqual(1, shown.Count);
            Assert.AreEqual("Power Ballad", shown[0].Card.Name);
            Assert.AreEqual(1, f.DiscardPile.Count - 1, "discard untouched (plus the played liner)");
            Assert.AreEqual(1, f.DrawPile.Count);
        }

        [Test]
        public void Effect_NextSongCountsAsRamp_SetsFlag()
        {
            Fight f = Started();
            Play(f, TestEffectCard("Horn", CardType.Liner, 0, new EffectData(EffectKind.NextSongCountsAsRamp, 0)));
            Assert.IsTrue(f.PendingRamp);
        }

        [Test]
        public void Effect_RaiseFlowCap_AtOldCap_FlowCanGrow()
        {
            Fight f = Started();
            f.TestSetFlow(10);
            Play(f, TestEffectCard("Cap", CardType.Liner, 0, new EffectData(EffectKind.RaiseFlowCap, 2)));
            Assert.AreEqual(12, f.FlowCap);
            Assert.AreEqual(10, f.Flow);
            Play(f, TestEffectCard("Flow Card", CardType.Liner, 0, new EffectData(EffectKind.GainFlow, 5)));
            Assert.AreEqual(12, f.Flow);
        }

        [Test]
        public void Effect_AddCardToPile_DrawDiscardHand()
        {
            Fight f = Started();
            Play(f, TestEffectCard("Adder", CardType.Liner, 0,
                new EffectData(EffectKind.AddCardToPile, 1, "card.int.static", PileKind.Draw),
                new EffectData(EffectKind.AddCardToPile, 2, "card.int.static", PileKind.Discard),
                new EffectData(EffectKind.AddCardToPile, 1, "card.cr.album_cut", PileKind.Hand)));
            Assert.AreEqual(1, f.DrawPile.Count);
            Assert.AreEqual(3, f.DiscardPile.Count); // 2 Static + the played Adder
            Assert.AreEqual(1, f.Hand.Count);
            Assert.AreEqual(3, f.StaticCount);
        }

        // ------------------------------------------------------------ keywords

        [Test]
        public void Keyword_OnAir_PreventsDeadAir()
        {
            Fight f = Started();
            Play(f, RequestLine());
            f.EndTurn();
            Assert.AreEqual(0, f.DeadAirCount);
        }

        [Test]
        public void Keyword_Hold_StaysInHand()
        {
            Fight f = Started();
            CardData hold = TestLiner("test.hold");
            hold.Keywords.Hold = true;
            CardInstance h = f.TestPutInHand(hold);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(h, f.Hand[0]);
        }

        [Test]
        public void Keyword_Pulled_RemovedForDaypart_BackInDeckAfter()
        {
            Fight f = Started();
            CardData pulled = TestLiner("test.pulled");
            pulled.Keywords.Pulled = true;
            Play(f, pulled);
            Assert.AreEqual(1, f.RemovedPile.Count);
            Assert.AreEqual(0, f.DiscardPile.Count);
            Assert.IsTrue(f.DeckAfterDaypart().Exists(c => c.Id == "test.pulled"));
        }

        [Test]
        public void Keyword_Unplayable_HasReason()
        {
            Fight f = Started();
            CardInstance s = f.TestPutInHand(Static());
            Assert.IsFalse(f.CanPlay(s.Uid, out string reason));
            StringAssert.Contains("Unplayable", reason);
        }

        [Test]
        public void Keyword_EveryKeywordHasTooltip()
        {
            foreach (Keyword k in System.Enum.GetValues(typeof(Keyword)))
            {
                Assert.IsFalse(string.IsNullOrEmpty(Glossary.KeywordText(k, Balance())), k.ToString());
            }

            StringAssert.Contains("x1.5", Glossary.KeywordText(Keyword.Anthem, Balance()));
            StringAssert.Contains("5 / 10 / 15", Glossary.TermText(Term.DeadAir, Balance()));
        }

        // ------------------------------------------------------------ effect order

        [Test]
        public void EffectOrder_ReachThenBaseNumbersThenEffectsThenFlow()
        {
            Fight f = Started();
            f.TestSetLastSong(PowerBallad());
            CardData song = Song("test.order", "Order Song", Genre.Rock, 3, 1, 6);
            song.Loyalty = 2;
            song.Hype = 1;
            song.Effects.Add(new EffectData(EffectKind.GainCash, 5));
            Play(f, song);
            string[] order = { "Order Song: 6 = 6 Reach", "Order Song: +2 Loyalty", "Order Song: +1 Hype", "Order Song: +5 Cash", "Flow 0 -> 2" };
            int at = 0;
            foreach (string line in f.Log.Lines)
            {
                if (at < order.Length && line.Contains(order[at])) at++;
            }

            Assert.AreEqual(order.Length, at, at < order.Length ? order[at] : "");
        }
    }
}
