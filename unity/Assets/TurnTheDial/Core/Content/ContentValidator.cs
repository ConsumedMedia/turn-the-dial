using System;
using System.Collections.Generic;

namespace TurnTheDial.Core.Content
{
    /// <summary>
    /// Data validation (cards.md Done checklist; combat.md daypart-start Loyalty; rivals.md).
    /// Returns human-readable errors; an empty list means valid.
    /// </summary>
    public static class ContentValidator
    {
        public static List<string> ValidateCard(CardData card)
        {
            var errors = new List<string>();
            if (card == null)
            {
                errors.Add("Card is null.");
                return errors;
            }

            string label = string.IsNullOrEmpty(card.Id) ? "(no id)" : card.Id;
            ValidateCardFields(card, label, errors);

            if (card.Type == CardType.Interference)
            {
                if (card.Pool != CardPool.Rival)
                {
                    errors.Add(label + ": Interference cards must be in the Rival pool.");
                }
            }
            else if (card.Upgrade == null)
            {
                errors.Add(label + ": needs a remastered upgrade (only Interference has none).");
            }

            if (card.Upgrade != null)
            {
                CardData up = card.Upgrade;
                ValidateCardFields(up, label + " RM", errors);
                if (up.Id != card.Id)
                {
                    errors.Add(label + " RM: upgrade must keep the id.");
                }

                if (up.Name != card.Name)
                {
                    errors.Add(label + " RM: upgrade must keep the name (Burnout matches on name).");
                }

                if (!up.IsRemastered)
                {
                    errors.Add(label + " RM: upgrade must be flagged remastered.");
                }

                if (up.Upgrade != null)
                {
                    errors.Add(label + " RM: a card can be remastered once only.");
                }

                if (up.Type != card.Type || up.Pool != card.Pool || up.Rarity != card.Rarity)
                {
                    errors.Add(label + " RM: upgrade must keep type, pool and rarity.");
                }
            }

            if (card.IsRemastered)
            {
                errors.Add(label + ": a base card cannot be flagged remastered.");
            }

            return errors;
        }

        private static void ValidateCardFields(CardData card, string label, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(card.Id))
            {
                errors.Add(label + ": missing id.");
            }

            if (string.IsNullOrWhiteSpace(card.Name))
            {
                errors.Add(label + ": missing name.");
            }

            if (card.Cost < CardRules.CostMin || card.Cost > CardRules.CostMax)
            {
                errors.Add(label + ": cost " + card.Cost + " is outside 0 to 3.");
            }

            if (card.Type == CardType.Liner && card.Cost > CardRules.LinerCostMax)
            {
                errors.Add(label + ": Liners cost 0 to 1.");
            }

            if (card.Type == CardType.Song)
            {
                if (card.Tempo < CardRules.TempoMin || card.Tempo > CardRules.TempoMax)
                {
                    errors.Add(label + ": Song tempo " + card.Tempo + " is outside 1 to 5.");
                }

                if (card.Genre == Genre.None)
                {
                    errors.Add(label + ": Songs need a genre.");
                }
            }
            else
            {
                if (card.Genre != Genre.None || card.Tempo != 0)
                {
                    errors.Add(label + ": only Songs have a genre and tempo.");
                }
            }

            if (card.Keywords == null)
            {
                errors.Add(label + ": missing keywords.");
            }
            else
            {
                if (card.Keywords.Drop < 0)
                {
                    errors.Add(label + ": Drop N cannot be negative.");
                }

                if (card.Keywords.Anthem && card.Type != CardType.Song)
                {
                    errors.Add(label + ": Anthem is a Song keyword.");
                }

                if (card.Keywords.Sticky && card.Type != CardType.Interference)
                {
                    errors.Add(label + ": Sticky is an Interference keyword.");
                }
            }

            if (card.Reach < 0 || card.Loyalty < 0 || card.Hype < 0 || card.Cash < 0 || card.ListenerCost < 0)
            {
                errors.Add(label + ": base numbers cannot be negative.");
            }

            ValidateEffects(card.Effects, label + " effects", errors);
            if (card.Triggers == null)
            {
                errors.Add(label + ": missing triggers list.");
            }
            else
            {
                for (int i = 0; i < card.Triggers.Count; i++)
                {
                    CardTrigger t = card.Triggers[i];
                    if (t == null || !Enum.IsDefined(typeof(CardTriggerKind), t.Kind))
                    {
                        errors.Add(label + " trigger " + i + ": unknown trigger.");
                        continue;
                    }

                    ValidateEffects(t.Effects, label + " trigger " + i + " effects", errors);
                }
            }
        }

        /// <summary>
        /// Effects must be library entries with valid amounts. Drop is a keyword, never an effect
        /// (cards.md): the effect enum has no Drop member, so any undefined value is rejected here.
        /// </summary>
        public static void ValidateEffects(List<EffectData> effects, string label, List<string> errors)
        {
            if (effects == null)
            {
                errors.Add(label + ": missing list.");
                return;
            }

            for (int i = 0; i < effects.Count; i++)
            {
                EffectData e = effects[i];
                if (e == null)
                {
                    errors.Add(label + "[" + i + "]: empty entry.");
                    continue;
                }

                if (!Enum.IsDefined(typeof(EffectKind), e.Kind))
                {
                    errors.Add(label + "[" + i + "]: unknown effect " + (int)e.Kind + ".");
                }

                if (e.Amount < 0)
                {
                    errors.Add(label + "[" + i + "]: negative amount.");
                }

                if (e.Kind == EffectKind.AddCardToPile && string.IsNullOrEmpty(e.CardId))
                {
                    errors.Add(label + "[" + i + "]: AddCardToPile needs a card id.");
                }
            }
        }

        public static List<string> ValidateGear(GearData gear)
        {
            var errors = new List<string>();
            string label = gear?.Id ?? "(gear)";
            if (gear == null || string.IsNullOrWhiteSpace(gear.Id) || string.IsNullOrWhiteSpace(gear.Name))
            {
                errors.Add(label + ": missing id or name.");
                return errors;
            }

            ValidateEffects(gear.Effects, label + " effects", errors);

            // Proposed (combat.md): daypart-start effects grant no Loyalty, because turn 1's
            // Loyalty clear would wipe it. Use the turn-start trigger instead.
            if (gear.Trigger == GearTrigger.DaypartStart && gear.Effects != null)
            {
                for (int i = 0; i < gear.Effects.Count; i++)
                {
                    if (gear.Effects[i] != null && gear.Effects[i].Kind == EffectKind.GainLoyalty)
                    {
                        errors.Add(label + ": GainLoyalty is not allowed on the daypart-start trigger.");
                    }
                }
            }

            return errors;
        }

        public static List<string> ValidateRival(RivalData rival, CardCatalog catalog)
        {
            var errors = new List<string>();
            string label = rival?.Id ?? "(rival)";
            if (rival == null || string.IsNullOrWhiteSpace(rival.Id) || string.IsNullOrWhiteSpace(rival.Name))
            {
                errors.Add(label + ": missing id or name.");
                return errors;
            }

            if (rival.StartingAudience <= 0)
            {
                errors.Add(label + ": starting audience must be above 0.");
            }

            if (rival.Pattern == null || rival.Pattern.Count == 0)
            {
                errors.Add(label + ": empty intent pattern.");
                return errors;
            }

            for (int i = 0; i < rival.Pattern.Count; i++)
            {
                IntentData intent = rival.Pattern[i];
                string at = label + " T" + (i + 1);
                if (intent.Hits < 1)
                {
                    errors.Add(at + ": hits must be at least 1.");
                }

                if (intent.Amount < 0)
                {
                    errors.Add(at + ": negative amount.");
                }

                if (intent.Kind == IntentKind.Interference)
                {
                    if (catalog == null || !catalog.TryGet(intent.CardId, out CardData card))
                    {
                        errors.Add(at + ": unknown Interference card '" + intent.CardId + "'.");
                    }
                    else if (card.Type != CardType.Interference)
                    {
                        errors.Add(at + ": " + intent.CardId + " is not an Interference card.");
                    }

                    if (intent.Pile == PileKind.Hand)
                    {
                        errors.Add(at + ": Interference goes to the draw or discard pile.");
                    }
                }
            }

            return errors;
        }

        public static List<string> ValidateFormat(FormatData format, CardCatalog catalog)
        {
            var errors = new List<string>();
            string label = format?.Id ?? "(format)";
            if (format == null || string.IsNullOrWhiteSpace(format.Id))
            {
                errors.Add(label + ": missing id.");
                return errors;
            }

            if (format.MaxListeners <= 0)
            {
                errors.Add(label + ": max listeners must be above 0.");
            }

            for (int i = 0; i < format.StartingDeck.Count; i++)
            {
                DeckEntry entry = format.StartingDeck[i];
                if (catalog == null || !catalog.TryGet(entry.CardId, out _))
                {
                    errors.Add(label + ": unknown deck card '" + entry.CardId + "'.");
                }

                if (entry.Count < 1)
                {
                    errors.Add(label + ": deck entry " + entry.CardId + " has count below 1.");
                }
            }

            return errors;
        }
    }
}
