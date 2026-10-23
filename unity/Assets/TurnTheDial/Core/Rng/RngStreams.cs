using System.Collections.Generic;

namespace TurnTheDial.Core.Rng
{
    /// <summary>
    /// The run's named RNG streams (save.md, RNG streams table). Each is a separate generator
    /// derived from the run seed and its stream name, so one system's rolls never shift another's.
    /// </summary>
    public sealed class RngStreams
    {
        public const string Shuffle = "rng.shuffle";
        public const string Rewards = "rng.rewards";
        public const string Shop = "rng.shop";
        public const string Map = "rng.map";
        public const string Rivals = "rng.rivals";
        public const string Events = "rng.events";
        public const string Staff = "rng.staff";

        /// <summary>Every stream name, in a fixed order (saves iterate this list, never a dictionary).</summary>
        public static readonly IReadOnlyList<string> AllNames = new[] { Shuffle, Rewards, Shop, Map, Rivals, Events, Staff };

        private readonly List<IRng> _streams = new List<IRng>();

        public RngStreams(ulong runSeed)
        {
            RunSeed = runSeed;
            for (int i = 0; i < AllNames.Count; i++)
            {
                string name = AllNames[i];
                _streams.Add(new SplitMix64Rng(name, StableHash.DeriveStreamSeed(runSeed, name)));
            }
        }

        public ulong RunSeed { get; }

        public IReadOnlyList<IRng> All => _streams;

        public IRng Get(string name)
        {
            for (int i = 0; i < _streams.Count; i++)
            {
                if (_streams[i].Name == name)
                {
                    return _streams[i];
                }
            }

            throw new KeyNotFoundException("Unknown RNG stream: " + name);
        }
    }
}
