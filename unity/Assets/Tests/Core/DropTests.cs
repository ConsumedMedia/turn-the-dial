using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>flow.md Drop (Decided, brief §3) and the cards.md Drop edge cases.</summary>
    public class DropTests
    {
        [Test]
        public void HypeMan_AtFlow4_Is11_HypeIgnored_FlowZeroAfter()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetFlow(4);
            f.TestSetHype(3);
            Assert.AreEqual(11, ReachOf(f, HypeMan())); // 3 + 2 x 4, Hype ignored for a non-song
            Assert.AreEqual(0, f.Flow);
            Assert.AreEqual(3, f.Hype);
        }

        [Test]
        public void HypeMan_IntoLock6_5Through_ZeroListeners_OneHit()
        {
            Fight f = Started(format: HipHopHeat(), listeners: 50);
            f.TestSetFlow(4);
            f.TestSetLock(6);
            Play(f, HypeMan());
            Assert.AreEqual(0, f.Lock);
            Assert.AreEqual(45, f.RivalAudience);
            Assert.AreEqual(50, f.Listeners); // 10% of 5 = 0
        }

        [Test]
        public void HypeMan_Exposed_AtFlow4_Is16()
        {
            Fight f = Started(format: HipHopHeat(), rival: TestRival(50, new IntentData(IntentKind.CommercialBreak, 0)));
            f.TestSetFlow(4);
            Assert.AreEqual(16, ReachOf(f, HypeMan())); // floor(11 x 1.5)
        }

        [Test]
        public void Banger_Hype2_Flow4_Is21_NoFlowMultiplier()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetFlow(4);
            f.TestSetHype(2);
            Assert.AreEqual(21, ReachOf(f, Banger())); // 7 + 2 + 3 x 4
            Assert.AreEqual(0, f.Flow);
        }

        [Test]
        public void Banger_AfterRooftopJam_AtFlow2_Is13_ThenFlow4_ThenDropToZero()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(RooftopJam());
            f.TestSetFlow(2);
            Assert.AreEqual(13, ReachOf(f, Banger())); // 7 + 3 x 2
            Assert.IsTrue(LogContains(f, "Flow 2 -> 4"), "ramp + streak happen first");
            Assert.IsTrue(LogContains(f, "Flow 4 -> 0 (Drop)"), "then the Drop wipes it");
            Assert.AreEqual(0, f.Flow);
            Assert.AreEqual("Banger", f.LastSong.Name, "the Drop Song still becomes the last song");
        }

        [Test]
        public void Banger_ThenNextSongRampsFromTempo5()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetLastSong(RooftopJam());
            Play(f, Banger());
            Assert.AreEqual(0, f.Flow);
            // Next song T1 after T5 with Loop counts as a ramp; plus the genre streak.
            Play(f, CornerCypher());
            Assert.AreEqual(2, f.Flow);
        }

        [Test]
        public void Drop_AtFlow0_DealsBase_PlusHypeOnlyForSongs()
        {
            Fight f = Started(format: HipHopHeat());
            f.TestSetHype(2);
            Assert.AreEqual(3, ReachOf(f, HypeMan()));
            Assert.AreEqual(9, ReachOf(f, Banger())); // 7 + 2 Hype + 3 x 0
        }

        [Test]
        public void DropSong_WithOneBurnout_x0_75_OnWholeNumber()
        {
            Fight f = Started(format: HipHopHeat());
            Play(f, Banger());
            f.TestSetFlow(4);
            Assert.AreEqual(14, ReachOf(f, Banger())); // (7 + 12) x 0.75 = 14.25
        }

        [Test]
        public void AnthemOnDropSong_MultipliesWholeDropNumber()
        {
            Fight f = Started();
            CardData anthemDrop = Song("test.anthem_drop", "Anthem Drop", Genre.Rock, 3, 1, 6, drop: 2, anthem: true);
            f.TestSetFlow(3);
            f.TestSetHype(1);
            Assert.AreEqual(19, ReachOf(f, anthemDrop)); // (6 + 1 + 2 x 3) x 1.5 = 19.5
            Assert.AreEqual(0, f.Flow);
        }

        [Test]
        public void Drop_ListenerConversion_OneRoundingOnCombinedNumber()
        {
            Fight f = Started(listeners: 50);
            f.TestSetFlow(5);
            // 5 + 1 x 5 = 10 -> 1 listener. Two separate roundings (5 and 5) would give 0.
            Play(f, NonSongReach(5, drop: 1));
            Assert.AreEqual(51, f.Listeners);
        }
    }
}
