using System;

namespace TurnTheDial.Core.Rng
{
    /// <summary>
    /// SplitMix64 generator. Fully defined by one 64-bit state, so it is trivial to save and gives
    /// the same sequence on every platform and runtime.
    /// </summary>
    public sealed class SplitMix64Rng : IRng
    {
        private const ulong Gamma = 0x9E3779B97F4A7C15UL;

        public SplitMix64Rng(string name, ulong state)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            State = state;
        }

        public string Name { get; }

        public ulong State { get; set; }

        public ulong NextULong()
        {
            unchecked
            {
                State += Gamma;
                return Mix(State);
            }
        }

        public int NextInt(int maxExclusive)
        {
            if (maxExclusive < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "maxExclusive must be at least 1.");
            }

            ulong bound = (ulong)maxExclusive;
            // Rejection sampling: no modulo bias, so results are uniform and platform-stable.
            ulong limit = ulong.MaxValue - (ulong.MaxValue % bound);
            ulong value;
            do
            {
                value = NextULong();
            }
            while (value >= limit);

            return (int)(value % bound);
        }

        /// <summary>The SplitMix64 finalizer. Also used to derive stream seeds.</summary>
        public static ulong Mix(ulong z)
        {
            unchecked
            {
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }
        }
    }
}
