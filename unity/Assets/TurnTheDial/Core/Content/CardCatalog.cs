using System.Collections.Generic;

namespace TurnTheDial.Core.Content
{
    /// <summary>All known cards by id. Lookup only; enumeration uses the insertion-ordered list.</summary>
    public sealed class CardCatalog
    {
        private readonly List<CardData> _cards = new List<CardData>();
        private readonly Dictionary<string, CardData> _byId = new Dictionary<string, CardData>();

        public IReadOnlyList<CardData> Cards => _cards;

        public void Add(CardData card)
        {
            _cards.Add(card);
            _byId[card.Id] = card;
        }

        public bool TryGet(string id, out CardData card)
        {
            if (id == null)
            {
                card = null;
                return false;
            }

            return _byId.TryGetValue(id, out card);
        }

        public CardData Get(string id)
        {
            if (!TryGet(id, out CardData card))
            {
                throw new KeyNotFoundException("Unknown card id: " + id);
            }

            return card;
        }

        /// <summary>Expands a format's starting deck into card data, in list order.</summary>
        public List<CardData> BuildDeck(FormatData format)
        {
            var deck = new List<CardData>();
            for (int i = 0; i < format.StartingDeck.Count; i++)
            {
                DeckEntry entry = format.StartingDeck[i];
                CardData card = Get(entry.CardId);
                for (int n = 0; n < entry.Count; n++)
                {
                    deck.Add(card);
                }
            }

            return deck;
        }
    }
}
