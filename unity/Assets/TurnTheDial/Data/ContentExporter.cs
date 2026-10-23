using System.Collections.Generic;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;

namespace TurnTheDial.Data
{
    /// <summary>Exports ScriptableObjects to Core's plain data (no Unity types cross over).</summary>
    public static class ContentExporter
    {
        public static CardData ToCore(CardDefinition def)
        {
            CardData card = Face(def, def.card, false);
            if (def.hasUpgrade)
            {
                card.Upgrade = Face(def, def.upgrade, true);
            }

            return card;
        }

        private static CardData Face(CardDefinition def, CardFields f, bool remastered)
        {
            return new CardData
            {
                Id = def.id,
                Name = f.displayName,
                Cost = f.cost,
                Type = f.type,
                Genre = f.genre,
                Tempo = f.tempo,
                Rarity = def.rarity,
                Pool = def.pool,
                Reach = f.reach,
                Loyalty = f.loyalty,
                Hype = f.hype,
                Cash = f.cash,
                ListenerCost = f.listenerCost,
                Keywords = new CardKeywords
                {
                    OnAir = f.onAir,
                    Drop = f.drop,
                    Anthem = f.anthem,
                    Hold = f.hold,
                    Pulled = f.pulled,
                    Sticky = f.sticky,
                    Unplayable = f.unplayable,
                },
                Effects = Effects(f.effects),
                EndOfTurnInHand = Effects(f.endOfTurnInHand),
                IsRemastered = remastered,
                Flavor = f.flavor,
                ArtId = f.artId,
            };
        }

        public static List<EffectData> Effects(List<EffectEntry> entries)
        {
            var list = new List<EffectData>();
            if (entries == null) return list;
            for (int i = 0; i < entries.Count; i++)
            {
                EffectEntry e = entries[i];
                list.Add(new EffectData(e.kind, e.amount, e.card != null ? e.card.id : null, e.pile));
            }

            return list;
        }

        public static RivalData ToCore(RivalDefinition def)
        {
            var rival = new RivalData { Id = def.id, Name = def.displayName, StartingAudience = def.startingAudience };
            for (int i = 0; i < def.pattern.Count; i++)
            {
                IntentEntry e = def.pattern[i];
                rival.Pattern.Add(new IntentData(e.kind, e.amount, e.hits, e.card != null ? e.card.id : null, e.pile));
            }

            return rival;
        }

        public static GearData ToCore(GearDefinition def)
        {
            return new GearData
            {
                Id = def.id,
                Name = def.displayName,
                Trigger = def.trigger,
                TriggerCardType = def.triggerCardType,
                Effects = Effects(def.effects),
            };
        }

        public static FormatData ToCore(FormatDefinition def)
        {
            var format = new FormatData
            {
                Id = def.id,
                Name = def.displayName,
                MaxListeners = def.maxListeners,
                Genre = def.genre,
                Signature = def.signature,
                StartingGearId = def.startingGear != null ? def.startingGear.id : null,
            };
            for (int i = 0; i < def.startingDeck.Count; i++)
            {
                DeckEntryDefinition e = def.startingDeck[i];
                format.StartingDeck.Add(new DeckEntry(e.card != null ? e.card.id : null, e.count));
            }

            return format;
        }

        public static CardCatalog Catalog(IList<CardDefinition> cards)
        {
            var catalog = new CardCatalog();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null) catalog.Add(ToCore(cards[i]));
            }

            return catalog;
        }

        /// <summary>A fresh FightSetup for one seed: the format's starting deck and gear vs the rival.</summary>
        public static FightSetup BuildSetup(FightContent content, ulong seed)
        {
            CardCatalog catalog = Catalog(content.cards);
            FormatData format = ToCore(content.format);
            var gear = new List<GearData>();
            if (content.format.startingGear != null)
            {
                gear.Add(ToCore(content.format.startingGear));
            }

            return new FightSetup
            {
                Balance = content.balance.ToCore(),
                Format = format,
                Rival = ToCore(content.rival),
                Kind = FightKind.Daypart,
                Catalog = catalog,
                Deck = catalog.BuildDeck(format),
                Gear = gear,
                Market = null, // tier 1 Chill Coastal Village: base rules, no modifiers
                Rng = new RngStreams(seed),
            };
        }

        /// <summary>Runs the Core validator over every asset in the content set.</summary>
        public static List<string> Validate(FightContent content)
        {
            var errors = new List<string>();
            if (content.balance == null) errors.Add("FightContent: missing balance config.");
            if (content.format == null) errors.Add("FightContent: missing format.");
            if (content.rival == null) errors.Add("FightContent: missing rival.");
            if (errors.Count > 0) return errors;

            CardCatalog catalog = Catalog(content.cards);
            for (int i = 0; i < catalog.Cards.Count; i++)
            {
                errors.AddRange(ContentValidator.ValidateCard(catalog.Cards[i]));
            }

            errors.AddRange(ContentValidator.ValidateFormat(ToCore(content.format), catalog));
            errors.AddRange(ContentValidator.ValidateRival(ToCore(content.rival), catalog));
            if (content.format.startingGear != null)
            {
                errors.AddRange(ContentValidator.ValidateGear(ToCore(content.format.startingGear)));
            }

            return errors;
        }
    }
}
