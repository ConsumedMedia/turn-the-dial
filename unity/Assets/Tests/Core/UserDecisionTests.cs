using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>
    /// Rules the user decided on 2026-10-23 after the prototype review (brief §2, §4, §5; combat.md,
    /// cards.md, flow.md, formats.md). Cue and Air Horn are covered in CombatLoopTests / FlowTests.
    /// </summary>
    public class UserDecisionTests
    {
        private static int IndexOf(Fight f, string text)
        {
            for (int i = 0; i < f.Log.Lines.Count; i++)
            {
                if (f.Log.Lines[i].Contains(text)) return i;
            }

            return -1;
        }

        // Decided (user, 2026-10-23), brief §2: both hit 0 from the same card = you win, listeners set to 1.
        [Test]
        public void BothHitZeroFromSameCard_YouWin_ListenersSetTo1()
        {
            Fight f = Started(listeners: 4);
            CardData both = NonSongReach(60);
            both.ListenerCost = 10; // 4 listeners + 6 from the Reach (10% of 60), then lose 10
            Play(f, both);
            Assert.AreEqual(0, f.RivalAudience);
            Assert.AreEqual(FightOutcome.WonAudienceZero, f.Outcome);
            Assert.AreEqual(1, f.Listeners);
            Assert.IsFalse(f.RunEnded);
            Assert.IsTrue(LogContains(f, "Both hit 0 from the same card: you win the daypart, and your listeners are set to 1."));
        }

        [Test]
        public void ListenersZeroAlone_StillEndsRun()
        {
            Fight f = Started(listeners: 4);
            Play(f, CarDealerSpot());
            Assert.AreEqual(FightOutcome.LostListenersZero, f.Outcome);
            Assert.AreEqual(0, f.Listeners);
            Assert.IsTrue(f.RunEnded);
        }

        // Decided (user, 2026-10-23), cards.md effect order steps 9-11.
        [Test]
        public void CardPlayedGear_FiresAfterCardReachesDiscard()
        {
            var gear = new GearData
            {
                Id = "test.gear.draw", Name = "Test Draw Gear", Trigger = GearTrigger.CardPlayed, TriggerCardType = CardType.DjBreak,
                Effects = new List<EffectData> { new EffectData(EffectKind.DrawCards, 1) },
            };
            Fight f = Started(gear: new List<GearData> { gear });
            CardInstance played = f.TestPutInHand(RequestLine());
            Assert.IsTrue(f.PlayCard(played.Uid).Ok);
            // Draw pile was empty: the gear's draw reshuffled the discard, which already held the played card.
            Assert.AreEqual(1, f.Hand.Count);
            Assert.AreSame(played, f.Hand[0]);
        }

        [Test]
        public void CardPlayedGear_FiresAfterFlowUpdateAndDrop()
        {
            var gear = new GearData
            {
                Id = "test.gear.flow", Name = "Test Flow Gear", Trigger = GearTrigger.CardPlayed, TriggerCardType = CardType.Song,
                Effects = new List<EffectData> { new EffectData(EffectKind.GainFlow, 1) },
            };
            Fight f = Started(format: HipHopHeat(), gear: new List<GearData> { gear });
            f.TestSetLastSong(RooftopJam());
            Play(f, Banger()); // ramp + streak to 2, Drop to 0, then the gear's +1
            Assert.AreEqual(1, f.Flow);
        }

        [Test]
        public void CardPlayedGear_FiresBeforeWinLossCheck()
        {
            var gear = new GearData
            {
                Id = "test.gear.loyalty", Name = "Test Loyalty Gear", Trigger = GearTrigger.CardPlayed, TriggerCardType = CardType.DjBreak,
                Effects = new List<EffectData> { new EffectData(EffectKind.GainLoyalty, 3) },
            };
            Fight f = Started(gear: new List<GearData> { gear });
            Play(f, NonSongReach(60));
            Assert.AreEqual(FightOutcome.WonAudienceZero, f.Outcome);
            Assert.AreEqual(3, f.Loyalty);
            Assert.Less(IndexOf(f, "Test Loyalty Gear: +3 Loyalty"), IndexOf(f, "audience hit 0"));
        }

        // Decided (user, 2026-10-23), brief §4: Monitor on intents rolls forward for the rest of the daypart.
        [Test]
        public void MonitorIntents_RollsForward_AlwaysNAhead()
        {
            Fight f = Started(format: BigFormat(), rival: MorningZoo());
            Play(f, TestEffectCard("Peek", CardType.DjBreak, 0, new EffectData(EffectKind.MonitorIntents, 2)));
            List<IntentPreview> t1 = f.PreviewUpcomingIntents(f.MonitorIntentCount);
            Assert.AreEqual(new[] { 2, 3 }, new[] { t1[0].TurnNumber, t1[1].TurnNumber });
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(2, f.MonitorIntentCount, "still active next turn");
            List<IntentPreview> t2 = f.PreviewUpcomingIntents(f.MonitorIntentCount);
            Assert.AreEqual(new[] { 3, 4 }, new[] { t2[0].TurnNumber, t2[1].TurnNumber });
            Assert.AreEqual(IntentKind.Interference, t2[0].Kind);
            Assert.AreEqual(IntentKind.Steal, t2[1].Kind);
        }

        // Proposed (cards.md): two Monitor-intent effects use the larger N; they don't add.
        [Test]
        public void MonitorIntents_TwoEffects_LargerNApplies()
        {
            Fight f = Started(format: BigFormat(), rival: MorningZoo());
            Play(f, TestEffectCard("Peek Two", CardType.DjBreak, 0, new EffectData(EffectKind.MonitorIntents, 2)));
            Play(f, TestEffectCard("Peek One", CardType.DjBreak, 0, new EffectData(EffectKind.MonitorIntents, 1)));
            Assert.AreEqual(2, f.MonitorIntentCount);
            Play(f, TestEffectCard("Peek Three", CardType.DjBreak, 0, new EffectData(EffectKind.MonitorIntents, 3)));
            Assert.AreEqual(3, f.MonitorIntentCount);
        }

        // Decided (user, 2026-10-23), brief §4: Monitor on the draw pile is a one-time look, not kept.
        [Test]
        public void MonitorDraw_OneTimeLook_ShownWhenResolvesAndInLog()
        {
            Fight f = Started();
            CardInstance second = f.TestPutOnDraw(AlbumCut());
            CardInstance top = f.TestPutOnDraw(PowerBallad());
            var shown = new List<IReadOnlyList<CardInstance>>();
            f.DrawMonitored += cards => shown.Add(cards);
            Play(f, TestEffectCard("Peek Draw", CardType.Liner, 0, new EffectData(EffectKind.MonitorDraw, 2)));
            Assert.AreEqual(1, shown.Count, "shown once, when it resolves");
            Assert.AreSame(top, shown[0][0]);
            Assert.AreSame(second, shown[0][1]);
            Assert.IsTrue(LogContains(f, "Peek Draw: Monitor 2, top of your draw pile: Power Ballad, Album Cut."));

            f.DebugDraw(); // the pile changes; nothing re-shows the peek
            f.EndTurn();
            Assert.AreEqual(1, shown.Count, "not refreshed");
        }

        // Decided (user, 2026-10-23), formats.md: Loop + Air Horn read the real tempos.
        [Test]
        public void Loop_AirHornOnT1AfterT5_OneRampOnly()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(Banger()); // T5
            Play(f, AirHorn());
            Play(f, CornerCypher()); // ramp (Air Horn; the Loop wrap would also give it) counts once
            Assert.AreEqual(2, f.Flow); // one ramp + genre streak
        }

        [Test]
        public void Loop_AirHornedT4ThenT1_IsNotARamp()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(BlockParty()); // T2
            Play(f, AirHorn());
            Play(f, RooftopJam()); // T4: one ramp + streak
            Assert.AreEqual(2, f.Flow);
            Play(f, CornerCypher()); // T1 after a real T4: no wrap, streak only
            Assert.AreEqual(3, f.Flow);
        }

        [Test]
        public void Glossary_FlowTooltip_UsesFightsActualCap()
        {
            Fight f = Started(format: HipHopHeat());
            StringAssert.Contains("cap 15", Glossary.TermText(Term.Flow, f.Balance, f.FlowCap));
            string cue = Glossary.TermText(Term.CueSlot, f.Balance);
            StringAssert.Contains("next turn", cue);
            StringAssert.DoesNotContain("ends your plays", cue);
        }
    }
}
