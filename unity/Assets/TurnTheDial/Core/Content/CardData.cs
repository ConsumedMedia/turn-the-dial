using System.Collections.Generic;

namespace TurnTheDial.Core.Content
{
    /// <summary>
    /// Plain card data (brief §4, cards.md Fields). No per-card code: behavior is keywords plus the
    /// ordered effect list. Treat as immutable once built.
    /// </summary>
    public sealed class CardData
    {
        public string Id { get; set; }

        /// <summary>Display name. Burnout matches on it (remastered versions share it).</summary>
        public string Name { get; set; }

        public int Cost { get; set; }

        public CardType Type { get; set; }

        public Genre Genre { get; set; }

        /// <summary>Songs only, 1 to 5.</summary>
        public int Tempo { get; set; }

        public Rarity Rarity { get; set; }

        public CardPool Pool { get; set; }

        public int Reach { get; set; }

        public int Loyalty { get; set; }

        public int Hype { get; set; }

        public int Cash { get; set; }

        public int ListenerCost { get; set; }

        public CardKeywords Keywords { get; set; } = new CardKeywords();

        /// <summary>Resolve on play, in order, after the card's Reach and base numbers (cards.md step 6).</summary>
        public List<EffectData> Effects { get; set; } = new List<EffectData>();

        /// <summary>
        /// Resolve at end of turn (end-turn step 2) while the card is in hand, e.g. Complaint Call's
        /// "lose 3 listeners". Uses the same effect library.
        /// </summary>
        public List<EffectData> EndOfTurnInHand { get; set; } = new List<EffectData>();

        /// <summary>The full remastered card, or null (Interference has none).</summary>
        public CardData Upgrade { get; set; }

        public bool IsRemastered { get; set; }

        public string Flavor { get; set; }

        public string ArtId { get; set; }

        public bool IsSong => Type == CardType.Song;

        /// <summary>Name with the RM stamp for remastered cards.</summary>
        public string DisplayName => IsRemastered ? Name + " RM" : Name;
    }
}
