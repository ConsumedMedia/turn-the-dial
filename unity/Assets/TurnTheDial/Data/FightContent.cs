using System.Collections.Generic;
using UnityEngine;

namespace TurnTheDial.Data
{
    /// <summary>
    /// The content one fight needs: format (deck + gear), rival, balance, and every card the
    /// fight can reference (deck cards and Interference).
    /// </summary>
    [CreateAssetMenu(menuName = "Turn the Dial/Fight Content", fileName = "FightContent")]
    public sealed class FightContent : ScriptableObject
    {
        public BalanceConfigAsset balance;
        public FormatDefinition format;
        public RivalDefinition rival;
        public List<CardDefinition> cards = new List<CardDefinition>();
    }
}
