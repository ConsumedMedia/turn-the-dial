using System.Collections.Generic;
using NUnit.Framework;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;
using TurnTheDial.Core.Sim;
using static TurnTheDial.Core.Tests.TestContent;

namespace TurnTheDial.Core.Tests
{
    /// <summary>save.md RNG streams; CLAUDE.md Determinism; combat.md "same seed + same inputs".</summary>
    public class DeterminismTests
    {
        private static FightSetup MorningZooSetup(ulong seed)
        {
            return new FightSetup
            {
                Balance = Balance(),
                Format = ClassicRock(),
                Rival = MorningZoo(),
                Catalog = Catalog(),
                Deck = ClassicRockDeck(),
                Gear = new List<GearData> { RibbonMic() },
                Rng = new RngStreams(seed),
            };
        }

        [Test]
        public void SplitMix64_GoldenValue_VersionStable()
        {
            // Published SplitMix64 reference: first output for state 0.
            Assert.AreEqual(0xE220A8397B1DCDAFUL, new SplitMix64Rng("x", 0).NextULong());
        }

        [Test]
        public void StableHash_Fnv1a_EmptyStringIsOffsetBasis()
        {
            Assert.AreEqual(14695981039346656037UL, StableHash.Fnv1a64(""));
            Assert.AreNotEqual(StableHash.Fnv1a64("rng.shuffle"), StableHash.Fnv1a64("rng.rewards"));
        }

        [Test]
        public void Streams_SameSeedSameSequence_DifferentStreamsDiffer()
        {
            var a = new RngStreams(42);
            var b = new RngStreams(42);
            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(a.Get(RngStreams.Shuffle).NextInt(1000), b.Get(RngStreams.Shuffle).NextInt(1000));
            }

            Assert.AreNotEqual(new RngStreams(42).Get(RngStreams.Shuffle).NextULong(), new RngStreams(42).Get(RngStreams.Rewards).NextULong());
        }

        [Test]
        public void NextInt_InRange()
        {
            var rng = new SplitMix64Rng("t", 7);
            for (int i = 0; i < 1000; i++)
            {
                int v = rng.NextInt(5);
                Assert.IsTrue(v >= 0 && v < 5);
            }

            Assert.AreEqual(0, rng.NextInt(1));
        }

        [Test]
        public void Fight_UsesOnlyShuffleStream()
        {
            FightSetup setup = MorningZooSetup(99);
            var fresh = new RngStreams(99);
            var fight = new Fight(setup);
            fight.Start();
            FightSimulator.RunOne(s => setup, 99, new GreedyPolicy());
            for (int i = 0; i < RngStreams.AllNames.Count; i++)
            {
                string name = RngStreams.AllNames[i];
                if (name == RngStreams.Shuffle) continue;
                Assert.AreEqual(fresh.Get(name).State, setup.Rng.Get(name).State, name + " must be untouched");
            }

            Assert.AreNotEqual(fresh.Get(RngStreams.Shuffle).State, setup.Rng.Get(RngStreams.Shuffle).State);
        }

        [Test]
        public void DaypartShuffle_IsFisherYatesOnRngShuffle()
        {
            FightSetup setup = MorningZooSetup(5);
            var fight = new Fight(setup);
            fight.Start();
            // Reproduce: uids 1..10 in deck order, Fisher-Yates from the end with rng.shuffle.
            IRng rng = new RngStreams(5).Get(RngStreams.Shuffle);
            var order = new List<int>();
            for (int i = 1; i <= 10; i++) order.Add(i);
            for (int i = order.Count - 1; i > 0; i--)
            {
                int j = rng.NextInt(i + 1);
                int t = order[i];
                order[i] = order[j];
                order[j] = t;
            }

            var actual = new List<int>();
            foreach (CardInstance c in fight.Hand) actual.Add(c.Uid);
            foreach (CardInstance c in fight.DrawPile) actual.Add(c.Uid);
            CollectionAssert.AreEqual(order, actual);
        }

        [Test]
        public void SameSeedSameInputs_IdenticalLog()
        {
            SimFightResult a = FightSimulator.RunOne(MorningZooSetup, 12345, new GreedyPolicy());
            SimFightResult b = FightSimulator.RunOne(MorningZooSetup, 12345, new GreedyPolicy());
            CollectionAssert.AreEqual(a.Log, b.Log);
            Assert.AreEqual(a.Outcome, b.Outcome);
            Assert.AreEqual(a.FinalListeners, b.FinalListeners);
        }

        [Test]
        public void DifferentSeed_DifferentShuffle()
        {
            SimFightResult a = FightSimulator.RunOne(MorningZooSetup, 1, new GreedyPolicy());
            SimFightResult b = FightSimulator.RunOne(MorningZooSetup, 2, new GreedyPolicy());
            CollectionAssert.AreNotEqual(a.Log, b.Log);
        }

        [Test]
        public void SeedChain_DeterministicAndAdvances()
        {
            Assert.AreEqual(SeedChain.Next(1), SeedChain.Next(1));
            Assert.AreNotEqual(1UL, SeedChain.Next(1));
            Assert.AreNotEqual(SeedChain.Next(1), SeedChain.Next(SeedChain.Next(1)));
        }

        [Test]
        [Category("Sim")]
        public void Sim_MorningZoo_FiveSeeds_Completes()
        {
            List<SimFightResult> results = FightSimulator.RunMany(MorningZooSetup, 1, 5, new GreedyPolicy());
            Assert.AreEqual(5, results.Count);
            foreach (SimFightResult r in results)
            {
                Assert.AreNotEqual(FightOutcome.None, r.Outcome);
                Assert.LessOrEqual(r.TurnsPlayed, 6);
            }
        }
    }
}
