using System;
using System.Collections.Generic;
using TurnTheDial.Core.Content;
using UnityEngine;

namespace TurnTheDial.Data
{
    [Serializable]
    public sealed class IntentEntry
    {
        public IntentKind kind;
        [Tooltip("Base value before Buzz: Steal / per-hit amount, Contest, Lock, Buzz, or Interference copies.")]
        [Min(0)] public int amount;
        [Min(1)] public int hits = 1;
        [Tooltip("Interference only.")] public CardDefinition card;
        [Tooltip("Interference only: Draw or Discard.")] public PileKind pile;
    }

    /// <summary>A rival show with its fixed intent pattern (rivals.md).</summary>
    [CreateAssetMenu(menuName = "Turn the Dial/Rival", fileName = "Rival")]
    public sealed class RivalDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        [Min(1)] public int startingAudience;
        public List<IntentEntry> pattern = new List<IntentEntry>();
    }
}
