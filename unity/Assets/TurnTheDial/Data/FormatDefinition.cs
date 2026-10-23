using System;
using System.Collections.Generic;
using TurnTheDial.Core.Content;
using UnityEngine;

namespace TurnTheDial.Data
{
    [Serializable]
    public sealed class DeckEntryDefinition
    {
        public CardDefinition card;
        [Min(1)] public int count = 1;
    }

    /// <summary>A format (formats.md).</summary>
    [CreateAssetMenu(menuName = "Turn the Dial/Format", fileName = "Format")]
    public sealed class FormatDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [Min(1)] public int maxListeners;
        public Genre genre;
        public Signature signature;
        public GearDefinition startingGear;
        public List<DeckEntryDefinition> startingDeck = new List<DeckEntryDefinition>();
    }
}
