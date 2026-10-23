using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using static TurnTheDial.Core.Tests.TestContent;
using static TurnTheDial.Core.Tests.TestPlay;

namespace TurnTheDial.Core.Tests
{
    /// <summary>rivals.md intents, Morning Zoo's pattern, shown = resolved, loop, Monitor peeks.</summary>
    public class RivalTests
    {
        [Test]
        public void MorningZoo_PatternExactlyAsRivalsDoc()
        {
            RivalData mz = MorningZoo();
            Assert.AreEqual(50, mz.StartingAudience);
            Fight f = Started(rival: mz, listeners: 85);
            string[] labels =
            {
                "Interference 2 Static -> draw pile", "Steal 6", "Interference 1 Complaint Call -> discard pile",
                "Steal 7", "Interference 2 Static -> draw pile", "Big Segment 2 x 6",
            };
            for (int t = 0; t < 6; t++)
            {
                Assert.AreEqual(t + 1, f.Turn);
                Assert.AreEqual(labels[t], f.PreviewIntent().Label);
                Play(f, FreeOnAir());
                f.EndTurn();
                if (f.IsOver) break;
            }
        }

        [Test]
        public void MorningZoo_T1_TwoStaticShuffledIntoDrawPile()
        {
            Fight f = Started(rival: MorningZoo());
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(2, f.StaticCount);
            Assert.IsTrue(LogContains(f, "Static shuffled into your draw pile"));
        }

        [Test]
        public void MorningZoo_T3_ComplaintCallToDiscard()
        {
            Fight f = Started(rival: MorningZoo());
            for (int t = 0; t < 3; t++)
            {
                Play(f, FreeOnAir());
                f.EndTurn();
            }

            // Turn 4's draw may already have drawn it from the discard pile, so check where it was added.
            Assert.IsTrue(LogContains(f, "Morning Zoo: Complaint Call added to your discard pile."));
            Assert.AreEqual(1, f.StaticCount - 2, "2 Static + 1 Complaint Call in your piles");
        }

        [Test]
        public void BigSegment_EachHitThroughLoyaltySeparately()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.BigSegment, 6, hits: 2)));
            Play(f, FreeOnAir());
            f.TestSetLoyalty(8);
            f.EndTurn();
            // hit 1: 6 absorbed (Loyalty 2 left); hit 2: 2 absorbed, 4 lost
            Assert.AreEqual(81, f.Listeners);
        }

        [Test]
        public void ShownIntentNumber_EqualsResolvedNumber_EveryMorningZooTurn_WithAndWithoutBuzz()
        {
            for (int buzz = 0; buzz <= 1; buzz++)
            {
                Fight f = Started(format: BigFormat(), rival: MorningZoo());
                for (int t = 0; t < 6 && !f.IsOver; t++)
                {
                    f.TestSetBuzz(buzz);
                    IntentPreview shown = f.PreviewIntent();
                    // Play off any Complaint Call so only the steal changes listeners.
                    for (int i = f.Hand.Count - 1; i >= 0; i--)
                    {
                        if (f.Hand[i].Card.Id == "card.int.complaint_call") Assert.IsTrue(f.PlayCard(f.Hand[i].Uid).Ok);
                    }

                    int before = f.Listeners;
                    Play(f, FreeOnAir());
                    f.EndTurn();
                    if (shown.Kind == IntentKind.Steal || shown.Kind == IntentKind.BigSegment)
                    {
                        int stolen = before - f.Listeners - f.RatingsLossPenalty; // turn 6 ends with the ratings check
                        Assert.AreEqual(shown.Total, stolen, "turn " + (t + 1) + " buzz " + buzz);
                    }
                }
            }
        }

        [Test]
        public void SteadyBuzz_StealShows9_Resolves9()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Steal, 6)));
            f.TestSetBuzz(1);
            Assert.AreEqual("Steal 9", f.PreviewIntent().Label);
            StringAssert.Contains("floor(6 x 1.5) = 9", f.PreviewIntent().Math);
        }

        [Test]
        public void Pattern_LoopsFromTurn1_WhenFightOutlastsIt()
        {
            Fight f = Started(format: BigFormat(), rival: TestRival(50,
                new IntentData(IntentKind.Steal, 6), new IntentData(IntentKind.HeavyRotation, 6), new IntentData(IntentKind.Contest, 8)));
            for (int t = 0; t < 3; t++)
            {
                Play(f, FreeOnAir());
                f.EndTurn();
            }

            Assert.AreEqual(4, f.Turn);
            Assert.AreEqual(IntentKind.Steal, f.CurrentIntent.Kind);
            Assert.AreEqual(0, f.CurrentIntentIndex);
        }

        [Test]
        public void Interference_IntoEmptyDrawPile_BecomesTheDrawPile()
        {
            Fight f = Started(rival: MorningZoo());
            Assert.AreEqual(0, f.DrawPile.Count);
            f.EndTurn();
            // T1 added 2 Static to the empty draw pile; turn 2's draw took them (no reshuffle needed).
            Assert.AreEqual(2, f.Hand.Count);
            Assert.IsTrue(f.Hand[0].IsInterference && f.Hand[1].IsInterference);
        }

        [Test]
        public void FightEndsEarly_NoFurtherIntentRevealed()
        {
            Fight f = Started(rival: MorningZoo());
            f.DebugWin();
            Assert.IsTrue(f.IsOver);
            Assert.AreEqual(1, f.Turn);
            Assert.IsFalse(f.EndTurn().Ok);
            Assert.AreEqual(IntentKind.Interference, f.CurrentIntent.Kind);
        }

        [Test]
        public void Monitor_ShowsNextIntents_TruncatedAtLastTurn()
        {
            Fight f = Started(format: BigFormat(), rival: MorningZoo());
            Play(f, TestEffectCard("Peek", CardType.DjBreak, 0, new EffectData(EffectKind.MonitorIntents, 2)));
            Assert.AreEqual(2, f.MonitorIntentCount);
            List<IntentPreview> next = f.PreviewUpcomingIntents(f.MonitorIntentCount);
            Assert.AreEqual(2, next.Count);
            Assert.AreEqual(IntentKind.Steal, next[0].Kind);
            Assert.AreEqual(IntentKind.Interference, next[1].Kind);
            for (int t = 0; t < 4; t++)
            {
                Play(f, FreeOnAir());
                f.EndTurn();
            }

            Assert.AreEqual(5, f.Turn);
            Assert.AreEqual(1, f.PreviewUpcomingIntents(2).Count, "only turn 6 remains");
        }

        [Test]
        public void CommercialBreak_WithMonitor_NextIntentStillShown()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.CommercialBreak, 0), new IntentData(IntentKind.Steal, 6)));
            Play(f, TestEffectCard("Peek", CardType.DjBreak, 0, new EffectData(EffectKind.MonitorIntents, 1)));
            List<IntentPreview> next = f.PreviewUpcomingIntents(f.MonitorIntentCount);
            Assert.AreEqual(1, next.Count);
            Assert.AreEqual("Steal 6", next[0].Label);
        }

        [Test]
        public void Trending_GainsBuzz_HeavyRotation_GainsLock()
        {
            Fight f = Started(rival: TestRival(50, new IntentData(IntentKind.Trending, 2), new IntentData(IntentKind.HeavyRotation, 6)));
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(1, f.Buzz); // 2, then -1 at the end of the rival phase
            Play(f, FreeOnAir());
            f.EndTurn();
            Assert.AreEqual(6, f.Lock);
            Assert.AreEqual(0, f.Buzz);
        }
    }
}
