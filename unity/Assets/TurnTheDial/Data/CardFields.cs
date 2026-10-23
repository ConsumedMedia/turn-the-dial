using System;
using System.Collections.Generic;
using TurnTheDial.Core.Content;
using UnityEngine;

namespace TurnTheDial.Data
{
    /// <summary>One effect-library entry as authored in a ScriptableObject.</summary>
    [Serializable]
    public sealed class EffectEntry
    {
        public EffectKind kind;
        [Min(0)] public int amount;
        [Tooltip("AddCardToPile only.")] public CardDefinition card;
        [Tooltip("AddCardToPile only.")] public PileKind pile;
    }

    /// <summary>
    /// The fields of one card face (base or remastered). The remastered face is a full card
    /// (brief §4 "upgrade: the full remastered card, same fields").
    /// </summary>
    [Serializable]
    public sealed class CardFields
    {
        public string displayName;
        [Range(0, 3)] public int cost;
        public CardType type;
        public Genre genre;
        [Range(0, 5)] public int tempo;
        [Min(0)] public int reach;
        [Min(0)] public int loyalty;
        [Min(0)] public int hype;
        [Min(0)] public int cash;
        [Min(0)] public int listenerCost;

        [Header("Keywords")]
        public bool onAir;
        [Min(0)] public int drop;
        public bool anthem;
        public bool hold;
        public bool pulled;
        public bool sticky;
        public bool unplayable;

        [Header("Effects")]
        public List<EffectEntry> effects = new List<EffectEntry>();
        [Tooltip("Resolve at end of turn while the card is in hand (e.g. Complaint Call).")]
        public List<EffectEntry> endOfTurnInHand = new List<EffectEntry>();

        [TextArea] public string flavor;
        public string artId;
    }
}
