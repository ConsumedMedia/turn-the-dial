using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>combat.md turn loop, draws, hand limit, cue, Dead Air, win/loss, ratings check.</summary>
    public class CombatLoopTests
    {
        private static int PlayOut(Fight f)
        {
            int turns = 0;
            while (!f.IsOver)
            {
                Play(f, FreeOnAir());
                f.EndTurn();
                turns++;
            }

            return turns;
        }

        [TestCase(FightKind.Daypart, 6)]
        [TestCase(FightKind.PrimeTime, 7)]
        [TestCase(FightKind.Boss, 8)]
        public void TurnLoop_RunsExactTurns_ThenRatingsCheck(FightKind kind, int turns)
        {
            Fight f = Started(format: BigFormat(), kind: kind);
            Assert.AreEqual(turns, f.TurnsTotal);
            Assert.AreEqual(turns, PlayOut(f));
            Assert.AreEqual(turns, f.Turn);
            Assert.AreEqual(FightOutcome.WonRatings, f.Outcome); // 1000/1000 vs 50/50: tie goes to you
        }

        [Test]
        public void PhaseOrder_EndTurnSteps_ThenRivalSteps()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Steal, 6), new IntentData(IntentKind.Steal, 6)));
            f.TestPutInHand(ComplaintCall());
            f.TestSetHype(1);
            f.TestSetLock(2);
            f.TestSetBuzz(1);
            f.EndTurn();
            string[] order =
            {
                "Dead Air: lose", "Complaint Call in hand at end of turn", "Discarded", "Hype decays",
                "Lock 2 clears", "Test Rival: Steal 9", "Buzz drops to 0", "reveals its turn 2 intent", "-- Turn 2 of 6 --", "Airtime set to 3",
            };
            int at = 0;
            for (int i = 0; i < f.Log.Lines.Count && at < order.Length; i++)
            {
                if (f.Log.Lines[i].Contains(order[at])) at++;
            }

            Assert.AreEqual(order.Length, at, "missing or out of order: " + (at < order.Length ? order[at] : ""));
        }

        [Test]
        public void Draw5_ReshufflesDiscardWhenDrawRunsOut()
        {
            var deck = new List<CardData>();
            for (int i = 0; i < 7; i++) deck.Add(AlbumCut());
            Fight f = NewFight(deck: deck);
            f.Start();
            Assert.AreEqual(5, f.Hand.Count);
            Assert.AreEqual(2, f.DrawPile.Count);
            f.EndTurn();
            Assert.AreEqual(5, f.Hand.Count);
            Assert.IsTrue(LogContains(f, "shuffled 5 discarded cards into the draw pile (rng.shuffle)"));
            Assert.AreEqual(2, f.DrawPile.Count);
        }

        [Test]
        public void DrawAndDiscardEmpty_StopDrawing_NoPenalty()
        {
            Fight f = Started();
            Assert.AreEqual(0, f.Hand.Count);
            Assert.AreEqual(85, f.Listeners);
            Assert.IsTrue(LogContains(f, "both empty, so drawing stops"));
        }

        [Test]
        public void HandLimit10_ExtraDrawsGoToDiscard()
        {
            CardData hold = TestLiner("test.hold");
            hold.Keywords.Hold = true;
            var deck = new List<CardData>();
            for (int i = 0; i < 5; i++) deck.Add(AlbumCut());
            Fight f = NewFight(deck: deck);
            f.Start();
            for (int i = 0; i < 8; i++) f.TestPutInHand(hold);
            Play(f, FreeOnAir());
            f.EndTurn(); // 5 Album Cuts discarded, 8 Hold cards stay
            Assert.AreEqual(10, f.Hand.Count); // 8 + 2 drawn
            Assert.AreEqual(3, f.DiscardPile.Count);
        }

        [Test]
        public void HandFull_WithCuedCard_CuedDoesNotCount()
        {
            Fight f = Started();
            for (int i = 0; i < 11; i++) f.TestPutInHand(TestLiner());
            Assert.IsTrue(f.Cue(f.Hand[0].Uid).Ok);
            Assert.AreEqual(10, f.Hand.Count);
            f.TestPutOnDraw(AlbumCut());
            f.DebugDraw();
            Assert.AreEqual(10, f.Hand.Count);
            Assert.AreEqual(1, f.DiscardPile.Count);
            Assert.AreEqual(1, f.CueSlot.Count);
        }

        [Test]
        public void Cue_StaysBetweenTurns_PlayableFromCue_PlayingEmptiesIt()
        {
            Fight f = Started();
            CardInstance cut = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(cut.Uid).Ok);
            f.EndTurn();
            Assert.AreEqual(1, f.CueSlot.Count);
            Assert.IsTrue(f.PlayCard(cut.Uid).Ok);
            Assert.AreEqual(0, f.CueSlot.Count);
            Assert.AreEqual(44, f.RivalAudience);
        }

        [Test]
        public void Cue_OnlyIntoEmptySlot()
        {
            Fight f = Started();
            CardInstance a = f.TestPutInHand(AlbumCut());
            CardInstance b = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(a.Uid).Ok);
            Assert.IsFalse(f.Cue(b.Uid).Ok);
        }

        // Cue: Decided (user, 2026-10-23), brief §2 / combat.md: free, once per turn, any time during
        // Program, only into an empty slot; doesn't end your plays; a card cued this turn plays next turn.
        [Test]
        public void Cue_IsFree_ThenKeepPlaying()
        {
            Fight f = Started();
            CardInstance a = f.TestPutInHand(AlbumCut());
            CardInstance b = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(a.Uid).Ok);
            Assert.AreEqual(3, f.Airtime, "cueing costs no Airtime");
            Assert.IsTrue(f.PlayCard(b.Uid).Ok);
            Assert.AreEqual(44, f.RivalAudience);
        }

        [Test]
        public void Cue_CardCuedThisTurn_UnplayableUntilNextTurn()
        {
            Fight f = Started();
            CardInstance a = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(a.Uid).Ok);
            Assert.IsFalse(f.CanPlay(a.Uid, out string reason));
            StringAssert.Contains("cued this turn", reason);
            Assert.IsFalse(f.PlayCard(a.Uid).Ok);
            Assert.AreEqual(1, f.CueSlot.Count);
            f.EndTurn();
            Assert.IsTrue(f.PlayCard(a.Uid).Ok);
            Assert.AreEqual(0, f.CueSlot.Count);
        }

        [Test]
        public void Cue_SecondCueInOneTurn_Refused_EvenAfterPlayingLastTurnsCuedCard()
        {
            Fight f = Started();
            CardInstance a = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(a.Uid).Ok);
            f.EndTurn();
            CardInstance b = f.TestPutInHand(AlbumCut());
            CardInstance c = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.PlayCard(a.Uid).Ok, "last turn's cued card plays from the slot");
            Assert.AreEqual(0, f.CueSlot.Count);
            Assert.IsTrue(f.Cue(b.Uid).Ok, "first cue this turn");
            Assert.IsFalse(f.CanCue(c.Uid, out string reason));
            StringAssert.Contains("once per turn", reason);
        }

        [Test]
        public void Cue_SecondCueInOneTurn_RefusedOncePerTurn()
        {
            Fight f = Started();
            CardInstance a = f.TestPutInHand(AlbumCut());
            CardInstance b = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(a.Uid).Ok);
            Assert.IsFalse(f.CanCue(b.Uid, out string reason));
            StringAssert.Contains("once per turn", reason);
        }

        [Test]
        public void Cue_SlotFullFromLastTurn_Refused()
        {
            Fight f = Started();
            CardInstance a = f.TestPutInHand(AlbumCut());
            Assert.IsTrue(f.Cue(a.Uid).Ok);
            f.EndTurn();
            CardInstance b = f.TestPutInHand(AlbumCut());
            Assert.IsFalse(f.CanCue(b.Uid, out string reason));
            StringAssert.Contains("full", reason);
        }

        [Test]
        public void CostAboveAirtime_CannotPlay()
        {
            Fight f = Started();
            f.TestSetAirtime(1);
            CardInstance pb = f.TestPutInHand(PowerBallad());
            Assert.IsFalse(f.CanPlay(pb.Uid, out string reason));
            StringAssert.Contains("costs 2 Airtime", reason);
            Assert.IsFalse(f.PlayCard(pb.Uid).Ok);
            Assert.AreEqual(1, f.Airtime);
        }

        [Test]
        public void Unplayable_CannotPlay_HoldUnplayableStaysInHand()
        {
            Fight f = Started();
            CardInstance s = f.TestPutInHand(Static());
            Assert.IsFalse(f.PlayCard(s.Uid).Ok);
            CardData heldStatic = Static();
            heldStatic.Keywords.Hold = true;
            CardInstance h = f.TestPutInHand(heldStatic);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.IsTrue(new List<CardInstance>(f.Hand).Contains(h));
            // The non-Hold Static was discarded (turn 2's reshuffle may draw it again).
            Assert.IsTrue(LogContains(f, "Discarded 1 card from hand (Hold kept: Static)"));
        }

        [Test]
        public void DeadAir_5_10_15_15_IgnoresLoyalty()
        {
            Fight f = Started(format: BigFormat());
            int[] losses = { 5, 10, 15, 15 };
            for (int i = 0; i < losses.Length; i++)
            {
                f.TestSetLoyalty(50);
                int before = f.Listeners;
                f.EndTurn();
                Assert.AreEqual(losses[i], before - f.Listeners, "Dead Air #" + (i + 1));
            }
        }

        [Test]
        public void DeadAir_BlueCollarHeartland_8_13_18()
        {
            var market = new MarketRules { Id = "market.blue_collar_heartland", Name = "Blue-Collar Heartland", DeadAirSteps = new[] { 8, 13, 18 } };
            Fight f = Started(format: BigFormat(), market: market);
            int[] losses = { 8, 13, 18, 18 };
            for (int i = 0; i < losses.Length; i++)
            {
                int before = f.Listeners;
                f.EndTurn();
                Assert.AreEqual(losses[i], before - f.Listeners);
            }
        }

        [Test]
        public void DeadAir_CuedCardIsNotPlaying()
        {
            Fight f = Started();
            CardInstance cut = f.TestPutInHand(AlbumCut());
            f.Cue(cut.Uid);
            f.EndTurn();
            Assert.AreEqual(1, f.DeadAirCount);
            Assert.AreEqual(80, f.Listeners);
        }

        [Test]
        public void NonOnAirCards_DoNotPreventDeadAir()
        {
            Fight f = Started();
            Play(f, StationId());
            Play(f, CarDealerSpot());
            f.EndTurn();
            Assert.AreEqual(1, f.DeadAirCount);
        }

        [Test]
        public void Win_RivalAtZeroMidCard_CardFinishesResolving_ThenWin()
        {
            Fight f = Started();
            CardData big = NonSongReach(60);
            big.Loyalty = 5;
            big.Effects.Add(new EffectData(EffectKind.GainHype, 2));
            Play(f, big);
            Assert.AreEqual(FightOutcome.WonAudienceZero, f.Outcome);
            Assert.AreEqual(5, f.Loyalty);
            Assert.AreEqual(2, f.Hype);
            Assert.IsFalse(f.RunEnded);
        }

        [Test]
        public void Loss_ListenersZero_FromOwnAd()
        {
            Fight f = Started(listeners: 4);
            Play(f, CarDealerSpot());
            Assert.AreEqual(FightOutcome.LostListenersZero, f.Outcome);
            Assert.IsTrue(f.RunEnded);
        }

        [Test]
        public void Loss_ListenersZero_FromDeadAir_StopsLaterEndTurnSteps()
        {
            Fight f = Started(listeners: 5);
            f.TestPutInHand(ComplaintCall());
            f.EndTurn();
            Assert.AreEqual(FightOutcome.LostListenersZero, f.Outcome);
            Assert.IsFalse(LogContains(f, "Complaint Call in hand at end of turn"));
        }

        [Test]
        public void Loss_ListenersZero_FromComplaintCall()
        {
            Fight f = Started(listeners: 3);
            f.TestPutInHand(ComplaintCall());
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(FightOutcome.LostListenersZero, f.Outcome);
            Assert.IsTrue(f.RunEnded);
        }

        [Test]
        public void Loss_ListenersZero_FromSteal()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.BigSegment, 6, hits: 2)), listeners: 6);
            Play(f, FreeOnAir());
            Assert.IsTrue(f.NextStealWouldEndRun);
            f.EndTurn();
            Assert.AreEqual(FightOutcome.LostListenersZero, f.Outcome);
            Assert.IsFalse(LogContains(f, "hit 2"), "the run ends immediately after hit 1");
        }

        [Test]
        public void RatingsCheck_TieGoesToYou()
        {
            Fight f = Started();
            Assert.AreEqual(6, PlayOut(f));
            Assert.AreEqual(FightOutcome.WonRatings, f.Outcome);
            Assert.IsTrue(LogContains(f, "A tie goes to you."));
        }

        [Test]
        public void RatingsLoss_10PercentRoundedUp_MaxUnchanged_RunContinues()
        {
            Fight f = Started(listeners: 75);
            PlayOut(f);
            // 75/85 = 88.2% < 50/50 = 100%: lose ceil(7.5) = 8
            Assert.AreEqual(FightOutcome.LostRatings, f.Outcome);
            Assert.AreEqual(8, f.RatingsLossPenalty);
            Assert.AreEqual(67, f.Listeners);
            Assert.AreEqual(85, f.MaxListeners);
            Assert.IsFalse(f.RunEnded);
        }

        [Test]
        public void RatingsLoss_PenaltyTo0_EndsRun()
        {
            Fight f = Started(listeners: 1);
            PlayOut(f);
            Assert.AreEqual(1, f.RatingsLossPenalty);
            Assert.AreEqual(0, f.Listeners);
            Assert.IsTrue(f.RunEnded);
        }

        [Test]
        public void RatingsLoss_Boss_EndsRun_NoPenalty()
        {
            Fight f = Started(kind: FightKind.Boss, listeners: 75);
            Assert.AreEqual(8, PlayOut(f));
            Assert.AreEqual(FightOutcome.LostRatings, f.Outcome);
            Assert.IsTrue(f.RunEnded);
            Assert.AreEqual(75, f.Listeners);
        }

        [Test]
        public void RatingsWin_HigherShare()
        {
            Fight f = Started(listeners: 80);
            f.TestSetRivalAudience(40);
            PlayOut(f);
            Assert.AreEqual(FightOutcome.WonRatings, f.Outcome); // 94.1% vs 80.0%
        }

        [Test]
        public void CommandsAfterFightEnds_AreRejected()
        {
            Fight f = Started();
            f.DebugLose();
            Assert.IsFalse(f.EndTurn().Ok);
            CardInstance c = f.TestPutInHand(AlbumCut());
            Assert.IsFalse(f.PlayCard(c.Uid).Ok);
            Assert.IsFalse(f.DebugDraw().Ok);
        }

        [Test]
        public void WouldTakeDeadAir_UntilOnAirPlayed()
        {
            Fight f = Started();
            Assert.IsTrue(f.WouldTakeDeadAir);
            Assert.AreEqual(5, f.NextDeadAirLoss);
            Play(f, AlbumCut());
            Assert.IsFalse(f.WouldTakeDeadAir);
        }
    }
}
