using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>
    /// Pins builder choices the docs don't settle (queued in NEXT.md as pending user decisions), so a
    /// later decision changes a test on purpose rather than by accident.
    /// </summary>
    public class PendingDecisionTests
    {
        private static int IndexOf(Fight f, string text)
        {
            for (int i = 0; i < f.Log.Lines.Count; i++)
            {
                if (f.Log.Lines[i].Contains(text)) return i;
            }

            return -1;
        }

        // Pending user decision (combat.md: simultaneous 0).
        [Test]
        public void SimultaneousZero_ListenersAndRivalFromOneCard_LossCheckedFirst()
        {
            Fight f = Started(listeners: 4);
            CardData both = NonSongReach(60);
            both.ListenerCost = 10; // 4 listeners + 6 from the Reach (10% of 60), then lose 10
            Play(f, both);
            Assert.AreEqual(0, f.RivalAudience);
            Assert.AreEqual(0, f.Listeners);
            Assert.AreEqual(FightOutcome.LostListenersZero, f.Outcome);
            Assert.IsTrue(f.RunEnded);
        }

        // Pending user decision (cards.md: effect order has no gear step).
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

        // Pending user decision (cards.md: how long a draw peek stays visible).
        [Test]
        public void MonitorDraw_SnapshotTakenWhenEffectResolves_ClearedAtEndOfTurn()
        {
            Fight f = Started();
            CardInstance second = f.TestPutOnDraw(AlbumCut());
            CardInstance top = f.TestPutOnDraw(PowerBallad());
            Play(f, TestEffectCard("Peek Draw", CardType.Liner, 0, new EffectData(EffectKind.MonitorDraw, 2)));
            Assert.AreEqual(2, f.MonitoredDraw.Count);
            Assert.AreSame(top, f.MonitoredDraw[0]);
            Assert.AreSame(second, f.MonitoredDraw[1]);

            f.DebugDraw(); // the live pile changes; the snapshot doesn't
            Assert.AreEqual(1, f.DrawPile.Count);
            Assert.AreEqual(2, f.MonitoredDraw.Count);
            Assert.AreSame(top, f.MonitoredDraw[0]);

            f.EndTurn();
            Assert.AreEqual(0, f.MonitoredDraw.Count);
        }

        // Pending user decision (formats.md: Loop + Air Horn).
        [Test]
        public void Loop_AirHornOnT1AfterT5_StillWraps()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(Banger()); // T5
            Play(f, AirHorn());
            Play(f, CornerCypher()); // T1 counted as T2: not above 5, but its own tempo 1 wraps
            Assert.AreEqual(2, f.Flow); // Loop ramp + genre streak
        }

        [Test]
        public void Loop_AirHornedT4CountedAs5_ThenT1Wraps()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(BlockParty()); // T2
            Play(f, AirHorn());
            Play(f, RooftopJam()); // T4 counted as 5: ramp + streak
            Assert.AreEqual(2, f.Flow);
            Play(f, CornerCypher()); // T1 after a song counted as T5: Loop ramp + streak
            Assert.AreEqual(4, f.Flow);
        }

        [Test]
        public void Glossary_FlowTooltip_UsesFightsActualCap()
        {
            Fight f = Started(format: HipHopHeat());
            StringAssert.Contains("cap 15", Glossary.TermText(Term.Flow, f.Balance, f.FlowCap));
            StringAssert.Contains("Cueing ends your plays this turn", Glossary.TermText(Term.CueSlot, f.Balance));
        }
    }
}
