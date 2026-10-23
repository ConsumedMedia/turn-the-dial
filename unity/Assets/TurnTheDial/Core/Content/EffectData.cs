namespace TurnTheDial.Core.Content
{
    /// <summary>A card trigger (brief §4 `triggers`): an effect list that fires on a trigger instead of on play.</summary>
    public sealed class CardTrigger
    {
        public CardTrigger(CardTriggerKind kind, System.Collections.Generic.List<EffectData> effects)
        {
            Kind = kind;
            Effects = effects ?? new System.Collections.Generic.List<EffectData>();
        }

        public CardTriggerKind Kind { get; }

        public System.Collections.Generic.List<EffectData> Effects { get; }
    }

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
