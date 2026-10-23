namespace TurnTheDial.Core.Content
{
    /// <summary>One entry of a card's or gear's ordered effect list (cards.md, Effect library).</summary>
    public sealed class EffectData
    {
        public EffectData(EffectKind kind, int amount, string cardId = null, PileKind pile = PileKind.Draw)
        {
            Kind = kind;
            Amount = amount;
            CardId = cardId;
            Pile = pile;
        }

        public EffectKind Kind { get; }

        /// <summary>N for every effect (cards to draw, Flow, Hype, copies to add...).</summary>
        public int Amount { get; }

        /// <summary>AddCardToPile only.</summary>
        public string CardId { get; }

        /// <summary>AddCardToPile only.</summary>
        public PileKind Pile { get; }
    }
}
