using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>statuses.md: every row's gain/clear, Buzz, Interference, Complaint Call, Cash.</summary>
    public class StatusTests
    {
        [Test]
        public void Start_Values_CashListenersAirtime()
        {
            Fight f = Started();
            Assert.AreEqual(50, f.Cash);
            Assert.AreEqual(85, f.Listeners);
            Assert.AreEqual(85, f.MaxListeners);
            Assert.AreEqual(3, f.Airtime);
            Assert.AreEqual(0, f.Flow);
            Assert.AreEqual(0, f.Hype);
            Assert.AreEqual(0, f.Loyalty);
            Assert.AreEqual(0, f.DeadAirCount);
            Assert.AreEqual(0, f.CueSlot.Count);
        }

        [Test]
        public void Airtime_SetTo3EachTurn_UnspentLost()
        {
            Fight f = Started();
            Play(f, FreeOnAir());
            Assert.AreEqual(3, f.Airtime);
            f.EndTurn();
            Assert.AreEqual(3, f.Airtime);
            Assert.IsTrue(LogContains(f, "Unspent Airtime lost: 3."));
        }

        [Test]
        public void Loyalty_ClearsAtTurnStart()
        {
            Fight f = Started();
            Play(f, RequestLine());
            Assert.AreEqual(5, f.Loyalty);
            f.EndTurn();
            Assert.AreEqual(0, f.Loyalty);
        }

        [Test]
        public void Loyalty_AbsorbsSteal()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Steal, 6)));
            Play(f, RequestLine());
            f.EndTurn();
            Assert.AreEqual(84, f.Listeners); // 6 - 5 Loyalty = 1
        }

        [Test]
        public void Loyalty_DoesNotBlockAdCost()
        {
            Fight f = Started();
            f.TestSetLoyalty(10);
            Play(f, CarDealerSpot());
            Assert.AreEqual(81, f.Listeners);
            Assert.AreEqual(10, f.Loyalty);
            Assert.AreEqual(75, f.Cash);
        }

        [Test]
        public void Loyalty_DoesNotBlockDeadAirOrComplaintCall()
        {
            Fight f = Started();
            f.TestPutInHand(ComplaintCall());
            f.TestSetLoyalty(20);
            f.EndTurn();
            Assert.AreEqual(85 - 5 - 3, f.Listeners);
        }

        [Test]
        public void Lock_ClearsAtStartOfRivalPhase_HeavyRotationProtectsNextPlayerPhase()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.HeavyRotation, 6)));
            f.TestSetLock(4);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(6, f.Lock, "old Lock 4 cleared, then Heavy Rotation 6");
            Assert.IsTrue(LogContains(f, "Lock 4 clears to 0"));
        }

        [Test]
        public void Buzz_StealPlus50Percent_RoundedDown_Decrements()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Steal, 7)));
            f.TestSetBuzz(1);
            Assert.AreEqual(10, f.PreviewIntent().PerHit); // floor(7 x 1.5)
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(75, f.Listeners);
            Assert.AreEqual(0, f.Buzz);
        }

        [Test]
        public void Buzz_PerHitRounding_OnBigSegment()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.BigSegment, 7, hits: 2)));
            f.TestSetBuzz(1);
            IntentPreview p = f.PreviewIntent();
            Assert.AreEqual(10, p.PerHit);
            Assert.AreEqual(20, p.Total); // not floor(14 x 1.5) = 21
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(65, f.Listeners);
        }

        [Test]
        public void Buzz_Zero_NoBonus_NoDecrementBelowZero()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Steal, 6)));
            Assert.AreEqual(6, f.PreviewIntent().PerHit);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(0, f.Buzz);
            Assert.AreEqual(79, f.Listeners);
        }

        [Test]
        public void Buzz_Timing_Trending2_LeavesBuzz1ForNextIntent()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Trending, 2), new IntentData(IntentKind.Steal, 6)));
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(1, f.Buzz);
            Assert.AreEqual(9, f.PreviewIntent().PerHit);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(76, f.Listeners);
            Assert.AreEqual(0, f.Buzz);
        }

        [Test]
        public void Contest_AtStart_HealsZero_BelowStart_CapsAtStart()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Contest, 8)));
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(50, f.RivalAudience);
            f.TestSetRivalAudience(45);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(50, f.RivalAudience);
            f.TestSetRivalAudience(30);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(38, f.RivalAudience);
        }

        [Test]
        public void Cash_NeverNegative_CostAboveCashUnavailable()
        {
            int cash = 50;
            Assert.IsFalse(CashRules.TryPay(ref cash, 60));
            Assert.AreEqual(50, cash);
            Assert.IsTrue(CashRules.TryPay(ref cash, 50));
            Assert.AreEqual(0, cash);
            Assert.IsFalse(CashRules.CanPay(cash, 1));
        }

        [Test]
        public void Static_CountsInterferenceInPiles_DrawVsDiscard()
        {
            Fight f = Started(rival: TestRival(50,
                new IntentData(IntentKind.Interference, 2, cardId: "card.int.static", pile: PileKind.Draw),
                new IntentData(IntentKind.Interference, 1, cardId: "card.int.complaint_call", pile: PileKind.Discard)));
            Play(f, FreeOnAir());
            f.EndTurn();
            // Turn 2 drew from the draw pile (2 Static + nothing else); both are in hand now.
            Assert.AreEqual(2, f.StaticCount);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(3, f.StaticCount);
            Assert.IsTrue(LogContains(f, "Complaint Call added to your discard pile"));
        }

        [Test]
        public void Interference_LeavesAfterDaypart_StickyStays()
        {
            List<CardData> deck = ClassicRockDeck();
            deck.Add(TestStickyStatic());
            deck.Add(Static());
            Fight f = NewFight(deck: deck);
            f.Start();
            List<CardData> after = f.DeckAfterDaypart();
            Assert.AreEqual(11, after.Count);
            Assert.IsTrue(after.Exists(c => c.Id == "test.sticky_static"));
            Assert.IsFalse(after.Exists(c => c.Id == "card.int.static"));
        }

        [Test]
        public void Interference_AddedDuringFight_LeavesAfterDaypart()
        {
            Fight f = NewFight(deck: ClassicRockDeck(), rival: MorningZoo());
            f.Start();
            f.EndTurn();
            Assert.AreEqual(2, f.StaticCount);
            Assert.AreEqual(10, f.DeckAfterDaypart().Count);
        }

        [Test]
        public void ComplaintCall_Play_Costs1_RemovedForDaypart_NoLoss()
        {
            Fight f = Started();
            CardInstance cc = f.TestPutInHand(ComplaintCall());
            Assert.IsTrue(f.PlayCard(cc.Uid).Ok);
            Assert.AreEqual(2, f.Airtime);
            Assert.AreEqual(1, f.RemovedPile.Count);
            Assert.AreEqual(0, f.DiscardPile.Count);
            Assert.AreEqual(0, f.StaticCount);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(85, f.Listeners);
        }

        [Test]
        public void ComplaintCall_IsNotOnAir()
        {
            Fight f = Started();
            Play(f, ComplaintCall());
            f.EndTurn();
            Assert.AreEqual(1, f.DeadAirCount);
        }

        [Test]
        public void ComplaintCall_InHandAtEndOfTurn_Lose3()
        {
            Fight f = Started();
            f.TestPutInHand(ComplaintCall());
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(82, f.Listeners);
        }

        [Test]
        public void ComplaintCall_AndDeadAir_BothApply_DeadAirFirst()
        {
            Fight f = Started();
            f.TestPutInHand(ComplaintCall());
            f.EndTurn();
            Assert.AreEqual(77, f.Listeners);
            int deadAir = f.Log.Lines.Count;
            int cc = -1;
            for (int i = 0; i < f.Log.Lines.Count; i++)
            {
                if (deadAir == f.Log.Lines.Count && f.Log.Lines[i].StartsWith("Dead Air: lose")) deadAir = i;
                if (f.Log.Lines[i].StartsWith("Complaint Call in hand at end of turn")) cc = i;
            }

            Assert.Less(deadAir, cc);
        }

        [Test]
        public void ComplaintCall_Cued_DoesNotTrigger()
        {
            Fight f = Started();
            CardInstance cc = f.TestPutInHand(ComplaintCall());
            Play(f, FreeOnAir());
            Assert.IsTrue(f.Cue(cc.Uid).Ok);
            f.EndTurn();
            Assert.AreEqual(85, f.Listeners);
        }

        [Test]
        public void ComplaintCall_HeldInHand_TriggersEachTurn()
        {
            Fight f = Started();
            CardData held = ComplaintCall();
            held.Keywords.Hold = true;
            f.TestPutInHand(held);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(82, f.Listeners);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(79, f.Listeners);
        }

        [Test]
        public void ListenersNeverAboveMax_MaxUnchangedByRatingsLoss()
        {
            Fight f = Started(listeners: 85);
            Play(f, NonSongReach(40));
            Assert.AreEqual(85, f.Listeners);
            Assert.AreEqual(85, f.MaxListeners);
        }
    }
}
