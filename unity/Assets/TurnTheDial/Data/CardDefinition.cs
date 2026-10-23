using TurnTheDial.Core.Content;
using UnityEngine;

namespace TurnTheDial.Data
{
    /// <summary>A card asset (brief §4). Exports to <see cref="CardData"/> via <see cref="ContentExporter"/>.</summary>
    [CreateAssetMenu(menuName = "Turn the Dial/Card", fileName = "Card")]
    public sealed class CardDefinition : ScriptableObject
    {
        [Tooltip("Stable dotted id, e.g. card.cr.album_cut. Never rename once shipped.")]
        public string id;
        public Rarity rarity;
        public CardPool pool;
        public CardFields card = new CardFields();

        [Tooltip("Every card except Interference has a remastered version.")]
        public bool hasUpgrade = true;
        public CardFields upgrade = new CardFields();
    }
}
