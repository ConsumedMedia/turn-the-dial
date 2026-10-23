using System.Collections.Generic;

namespace TurnTheDial.Core.Content
{
    public sealed class DeckEntry
    {
        public DeckEntry(string cardId, int count)
        {
            CardId = cardId;
            Count = count;
        }

        public string CardId { get; }

        public int Count { get; }
    }

    /// <summary>A format (formats.md): max listeners, genre, signature, starting gear and deck.</summary>
    public sealed class FormatData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int MaxListeners { get; set; }

        public Genre Genre { get; set; }

        public Signature Signature { get; set; }

        public string StartingGearId { get; set; }

        public List<DeckEntry> StartingDeck { get; set; } = new List<DeckEntry>();
    }

    /// <summary>A gear piece (gear.md): a trigger plus effects from the card effect library.</summary>
    public sealed class GearData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public GearTrigger Trigger { get; set; }

        /// <summary>CardPlayed / FirstCardOfTypeEachTurn filter.</summary>
        public CardType TriggerCardType { get; set; }

        public List<EffectData> Effects { get; set; } = new List<EffectData>();
    }

    /// <summary>Market modifiers that touch a fight (markets.md). Tier 1 has none.</summary>
    public sealed class MarketRules
    {
        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>Overrides the balance Dead Air steps when set (Blue-Collar Heartland and up).</summary>
        public int[] DeadAirSteps { get; set; }

        public int RivalBuzzAtStart { get; set; }
    }
}
