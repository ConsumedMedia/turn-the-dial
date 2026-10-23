namespace TurnTheDial.Core.Rng
{
    /// <summary>
    /// Version-stable string hash (FNV-1a, 64-bit, over UTF-16 code units). Never use
    /// string.GetHashCode for seeds: it changes between processes (save.md, RNG streams).
    /// </summary>
    public static class StableHash
    {
        private const ulong OffsetBasis = 14695981039346656037UL;
        private const ulong Prime = 1099511628211UL;

        public static ulong Fnv1a64(string text)
        {
            unchecked
            {
                ulong hash = OffsetBasis;
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    hash ^= (byte)(c & 0xFF);
                    hash *= Prime;
                    hash ^= (byte)(c >> 8);
                    hash *= Prime;
                }

                return hash;
            }
        }

        /// <summary>Seed for a named stream, derived from the run seed and the stream name.
        /// Proposed (save.md): derivation method.</summary>
        public static ulong DeriveStreamSeed(ulong runSeed, string streamName)
        {
            unchecked
            {
                return SplitMix64Rng.Mix(runSeed ^ SplitMix64Rng.Mix(Fnv1a64(streamName)));
            }
        }
    }
}
