namespace TurnTheDial.Core.Rng
{
    /// <summary>
    /// Derives the next seed from the current one through IRng (no clock time). Used by the
    /// prototype's restart key and by the sim to walk a reproducible list of seeds.
    /// </summary>
    public static class SeedChain
    {
        public const string StreamName = "seed.next";

        public static ulong Next(ulong seed)
        {
            IRng rng = new SplitMix64Rng(StreamName, StableHash.DeriveStreamSeed(seed, StreamName));
            return rng.NextULong();
        }
    }
}
