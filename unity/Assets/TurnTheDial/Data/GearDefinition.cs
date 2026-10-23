using System.Collections.Generic;
using TurnTheDial.Core.Content;
using UnityEngine;

namespace TurnTheDial.Data
{
    /// <summary>A gear piece (gear.md): trigger + effects from the card effect library.</summary>
    [CreateAssetMenu(menuName = "Turn the Dial/Gear", fileName = "Gear")]
    public sealed class GearDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public GearTrigger trigger;
        [Tooltip("CardPlayed / FirstCardOfTypeEachTurn filter.")]
        public CardType triggerCardType;
        public List<EffectEntry> effects = new List<EffectEntry>();
        [TextArea] public string tooltip;
    }
}
