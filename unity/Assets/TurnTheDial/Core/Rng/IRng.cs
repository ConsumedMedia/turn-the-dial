namespace TurnTheDial.Core.Rng
{
    /// <summary>
    /// The only source of randomness in the game (CLAUDE.md, Determinism). Every roll names its
    /// stream from save.md's RNG streams table.
    /// </summary>
    public interface IRng
    {
        /// <summary>Stream name, e.g. "rng.shuffle".</summary>
        string Name { get; }

        /// <summary>Current generator state. Saves store it (save.md).</summary>
        ulong State { get; set; }

        /// <summary>Uniform integer in [0, maxExclusive). maxExclusive must be at least 1.</summary>
        int NextInt(int maxExclusive);

        /// <summary>Next raw 64-bit value.</summary>
        ulong NextULong();
    }
}
