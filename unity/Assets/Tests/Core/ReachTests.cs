using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>combat.md Reach resolution and Applying Reach; brief §2.</summary>
    public class ReachTests
    {
        [Test]
        public void Song_BaseReach_NoFlowNoHype()
        {
            Fight f = Started();
            Assert.AreEqual(6, ReachOf(f, AlbumCut()));
            Assert.AreEqual(44, f.RivalAudience);
        }

        [Test]
        public void Song_HypeAddedBeforeFlowMultiplier_LogShowsMath()
        {
            Fight f = Started();
            f.TestSetHype(1);
            f.TestSetFlow(2);
            f.TestSetLock(3);
            Play(f, AlbumCut());
            // (6 + 1) x 1.2 = 8.4 -> 8; Lock absorbs 3; 5 through; 10% of 5 = 0.
            Assert.AreEqual(45, f.RivalAudience);
            Assert.AreEqual(0, f.Lock);
            Assert.IsTrue(LogContains(f, "Album Cut: (6 + 1 Hype) x 1.2 Flow = 8 Reach, Lock absorbs 3, 5 to the rival, +0 listeners"));
        }

        [Test]
        public void Song_HypeBeforeMultiplier_NotAfter()
        {
            Fight f = Started();
            f.TestSetHype(2);
            f.TestSetFlow(5);
            // (6 + 2) x 1.5 = 12, not 6 x 1.5 + 2 = 11.
            Assert.AreEqual(12, ReachOf(f, AlbumCut()));
        }

        [Test]
        public void NonSong_SkipsHypeAndFlow()
        {
            Fight f = Started();
            f.TestSetHype(3);
            f.TestSetFlow(5);
            Assert.AreEqual(7, ReachOf(f, NonSongReach(7)));
        }

        [Test]
        public void Exposed_Song_x1_5()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.CommercialBreak, 0)));
            Assert.IsTrue(f.IsRivalExposed);
            Assert.AreEqual(9, ReachOf(f, AlbumCut()));
        }

        [Test]
        public void Exposed_NonSong_Floors()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.CommercialBreak, 0)));
            // floor(7 x 1.5) = 10
            Assert.AreEqual(10, ReachOf(f, NonSongReach(7)));
        }

        [Test]
        public void Exposed_OnlyWhileShownIntentIsCommercialBreak()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.CommercialBreak, 0), new IntentData(IntentKind.Contest, 0)));
            Assert.IsTrue(f.IsRivalExposed);
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(IntentKind.Contest, f.CurrentIntent.Kind);
            Assert.IsFalse(f.IsRivalExposed);
            Assert.AreEqual(6, ReachOf(f, AlbumCut()));
        }

        [Test]
        public void Burnout_StacksByName_FloorAt25Percent()
        {
            Fight f = Started();
            int[] expected = { 6, 4, 3, 1, 1 }; // 6, 6x0.75=4.5, 6x0.5=3, 6x0.25=1.5, floor stays 0.25
            for (int i = 0; i < expected.Length; i++)
            {
                f.TestSetFlow(0);
                Assert.AreEqual(expected[i], ReachOf(f, AlbumCut()), "play " + (i + 1));
            }

            Assert.AreEqual(4, f.BurnoutStacksFor("Album Cut"));
        }

        [Test]
        public void Burnout_RemasteredVersionSharesNameAndCount()
        {
            Fight f = Started();
            Play(f, AlbumCut());
            f.TestSetFlow(0);
            // Album Cut RM (9) as the 2nd play: 9 x 0.75 = 6.75 -> 6
            Assert.AreEqual(6, ReachOf(f, AlbumCut().Upgrade));
            Assert.AreEqual(1, f.BurnoutStacksFor("Album Cut"));
        }

        [Test]
        public void Anthem_x1_5_AfterFlow_AndLoyaltyTempoTimesTwo()
        {
            Fight f = Started();
            f.TestSetFlow(2);
            // 9 x 1.2 x 1.5 = 16.2 -> 16; Loyalty T2 x 2 = 4
            Assert.AreEqual(16, ReachOf(f, PowerBallad()));
            Assert.AreEqual(4, f.Loyalty);
        }

        [Test]
        public void Lock_AbsorbsFirst_AllAbsorbed_NoListeners()
        {
            Fight f = Started(listeners: 50);
            f.TestSetLock(6);
            Assert.AreEqual(0, ReachOf(f, AlbumCut()));
            Assert.AreEqual(0, f.Lock);
            Assert.AreEqual(50, f.Listeners);
        }

        [Test]
        public void Conversion_10Percent_RoundsDown()
        {
            Fight f = Started(listeners: 50);
            Play(f, NonSongReach(19));
            Assert.AreEqual(51, f.Listeners); // 10% of 19 = 1.9 -> 1
        }

        [Test]
        public void Conversion_20Percent_Mailbag()
        {
            Fight f = Started(format: CountryRoads(), listeners: 50);
            Play(f, NonSongReach(19));
            Assert.AreEqual(53, f.Listeners); // 20% of 19 = 3.8 -> 3
        }

        [Test]
        public void Conversion_CappedAtMax_ExcessLost()
        {
            Fight f = Started(listeners: 84);
            Play(f, NonSongReach(30));
            Assert.AreEqual(85, f.Listeners);
            Assert.AreEqual(85, f.MaxListeners);
        }

        [Test]
        public void Conversion_CountsFullPostLockReach_PastRemainingAudience()
        {
            Fight f = Started(rival: QuietRival(5), listeners: 50);
            Play(f, NonSongReach(30));
            Assert.AreEqual(0, f.RivalAudience);
            Assert.AreEqual(53, f.Listeners);
        }

        [Test]
        public void ReachZeroAfterFloor_NoChange_FlowStillUpdates()
        {
            Fight f = Started(listeners: 50);
            CardData zero = Song("test.zero", "Zero Song", Genre.Rock, 4, 0, 0);
            f.TestSetLastSong(AlbumCut()); // T3 Rock
            Assert.AreEqual(0, ReachOf(f, zero));
            Assert.AreEqual(50, f.Listeners);
            Assert.AreEqual(2, f.Flow); // ramp T3->T4 + Rock streak
        }

        [Test]
        public void ReachZero_FromBurnoutFloor()
        {
            Fight f = Started();
            CardData one = Song("test.one", "One Song", Genre.Rock, 1, 0, 1);
            Assert.AreEqual(1, ReachOf(f, one));
            f.TestSetFlow(0);
            Assert.AreEqual(0, ReachOf(f, one)); // 1 x 0.75 = 0.75 -> 0
        }
    }
}
