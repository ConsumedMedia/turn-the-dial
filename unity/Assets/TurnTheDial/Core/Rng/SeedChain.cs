namespace TurnTheDial.Core.Rng
{
    /// <summary>
    /// Derives the next seed from the current one through IRng (no clock time). Tooling only: the
    /// sim walks a reproducible seed list with it, and the Game layer's debug restart key picks its
    /// new run seed with it. It is not a run stream, never used inside a run, and never saved
    /// (save.md, RNG streams; Decided, user, 2026-10-23).
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
