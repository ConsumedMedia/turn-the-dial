using System.Collections.Generic;
using System.Text;

namespace TurnTheDial.Core.Content
{
    /// <summary>Rules text generated from card data (brief §4: "rules text is generated from data where possible").</summary>
    public static class CardTextGenerator
    {
        public static string TypeLine(CardData card)
        {
            string type = TypeName(card.Type);
            if (card.Type == CardType.Song)
            {
                return type + " · " + GenreName(card.Genre) + " · T" + NumberFormat.Int(card.Tempo);
            }

            return type;
        }

        public static string TypeName(CardType type)
        {
            switch (type)
            {
                case CardType.Song: return "Song";
                case CardType.DjBreak: return "DJ Break";
                case CardType.Ad: return "Ad";
                case CardType.Promo: return "Promo";
                case CardType.Liner: return "Liner";
                default: return "Interference";
            }
        }

        public static string GenreName(Genre genre)
        {
            switch (genre)
            {
                case Genre.Rock: return "Rock";
                case Genre.HipHop: return "Hip-Hop";
                case Genre.Country: return "Country";
                case Genre.Pop: return "Pop";
                default: return "";
            }
        }

        /// <summary>Keywords on the card, in a fixed display order.</summary>
        public static List<Keyword> KeywordsOf(CardData card)
        {
            var list = new List<Keyword>();
            CardKeywords k = card.Keywords;
            if (k.Unplayable) list.Add(Keyword.Unplayable);
            if (k.OnAir) list.Add(Keyword.OnAir);
            if (k.Anthem) list.Add(Keyword.Anthem);
            if (k.Drop > 0) list.Add(Keyword.Drop);
            if (k.Hold) list.Add(Keyword.Hold);
            if (k.Pulled) list.Add(Keyword.Pulled);
            if (k.Sticky) list.Add(Keyword.Sticky);
            if (HasMonitor(card)) list.Add(Keyword.Monitor);
            return list;
        }

        public static string KeywordName(Keyword keyword, CardData card)
        {
            switch (keyword)
            {
                case Keyword.OnAir: return "On Air";
                case Keyword.Drop: return card != null ? "Drop " + NumberFormat.Int(card.Keywords.Drop) : "Drop N";
                case Keyword.Anthem: return "Anthem";
                case Keyword.Hold: return "Hold";
                case Keyword.Monitor: return "Monitor";
                case Keyword.Pulled: return "Pulled";
                case Keyword.Sticky: return "Sticky";
                default: return "Unplayable";
            }
        }

        public static string RulesText(CardData card, CardCatalog catalog = null)
        {
            var sb = new StringBuilder();
            List<Keyword> keywords = KeywordsOf(card);
            for (int i = 0; i < keywords.Count; i++)
            {
                if (keywords[i] == Keyword.Drop || keywords[i] == Keyword.Monitor)
                {
                    continue; // stated with their numbers below
                }

                Append(sb, KeywordName(keywords[i], card) + ".");
            }

            if (card.Keywords.Drop > 0)
            {
                Append(sb, NumberFormat.Int(card.Reach) + " Reach + Drop " + NumberFormat.Int(card.Keywords.Drop) + ".");
            }
            else if (card.Reach > 0)
            {
                Append(sb, NumberFormat.Int(card.Reach) + " Reach.");
            }

            if (card.Loyalty > 0) Append(sb, NumberFormat.Int(card.Loyalty) + " Loyalty.");
            if (card.Hype > 0) Append(sb, "+" + NumberFormat.Int(card.Hype) + " Hype.");
            if (card.Cash > 0) Append(sb, "+" + NumberFormat.Int(card.Cash) + " Cash.");
            if (card.ListenerCost > 0) Append(sb, "Lose " + NumberFormat.Int(card.ListenerCost) + " listeners.");

            for (int i = 0; i < card.Effects.Count; i++)
            {
                Append(sb, EffectText(card.Effects[i], catalog));
            }

            for (int t = 0; t < card.Triggers.Count; t++)
            {
                CardTrigger trigger = card.Triggers[t];
                var inner = new StringBuilder();
                for (int i = 0; i < trigger.Effects.Count; i++)
                {
                    Append(inner, EffectText(trigger.Effects[i], catalog));
                }

                Append(sb, TriggerLabel(trigger.Kind) + ": " + LowerFirst(inner.ToString()));
            }

            return sb.ToString();
        }

        public static string EffectText(EffectData e, CardCatalog catalog)
        {
            string n = NumberFormat.Int(e.Amount);
            switch (e.Kind)
            {
                case EffectKind.DrawCards: return "Draw " + n + ".";
                case EffectKind.GainFlow: return "+" + n + " Flow.";
                case EffectKind.GainHype: return "+" + n + " Hype.";
                case EffectKind.GainLoyalty: return n + " Loyalty.";
                case EffectKind.GainCash: return "+" + n + " Cash.";
                case EffectKind.LoseListeners: return "Lose " + n + " listeners.";
                case EffectKind.MonitorIntents: return "Monitor " + n + ": see the rival's next " + n + " intents.";
                case EffectKind.MonitorDraw: return "Monitor " + n + ": see the top " + n + " cards of your draw pile.";
                case EffectKind.NextSongCountsAsRamp: return "Your next Song counts as a rising ramp.";
                case EffectKind.RaiseFlowCap: return "+" + n + " Flow cap this daypart.";
                case EffectKind.AddCardToPile:
                    string name = e.CardId;
                    if (catalog != null && catalog.TryGet(e.CardId, out CardData added))
                    {
                        name = added.Name;
                    }

                    return "Add " + n + " " + name + " to your " + PileName(e.Pile) + ".";
                default: return "";
            }
        }

        public static string TriggerLabel(CardTriggerKind kind)
        {
            switch (kind)
            {
                default: return "In hand at end of turn";
            }
        }

        public static string PileName(PileKind pile)
        {
            switch (pile)
            {
                case PileKind.Discard: return "discard pile";
                case PileKind.Hand: return "hand";
                default: return "draw pile";
            }
        }

        private static bool HasMonitor(CardData card)
        {
            for (int i = 0; i < card.Effects.Count; i++)
            {
                if (card.Effects[i].Kind == EffectKind.MonitorIntents || card.Effects[i].Kind == EffectKind.MonitorDraw)
                {
                    return true;
                }
            }

            return false;
        }

        private static void Append(StringBuilder sb, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(text);
        }

        private static string LowerFirst(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }
    }
}
