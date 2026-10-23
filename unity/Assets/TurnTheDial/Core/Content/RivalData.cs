using System.Collections.Generic;

namespace TurnTheDial.Core.Content
{
    /// <summary>One turn of a rival's fixed pattern (rivals.md). Amounts are base values before Buzz.</summary>
    public sealed class IntentData
    {
        public IntentData(IntentKind kind, int amount, int hits = 1, string cardId = null, PileKind pile = PileKind.Draw)
        {
            Kind = kind;
            Amount = amount;
            Hits = hits;
            CardId = cardId;
            Pile = pile;
        }

        public IntentKind Kind { get; }

        /// <summary>Steal / per-hit Big Segment amount, Contest heal, Lock, Buzz, or Interference copies.</summary>
        public int Amount { get; }

        /// <summary>Big Segment hits (1 for single-number segments).</summary>
        public int Hits { get; }

        /// <summary>Interference only: the card added.</summary>
        public string CardId { get; }

        /// <summary>Interference only: draw or discard pile.</summary>
        public PileKind Pile { get; }
    }

    public sealed class RivalData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int StartingAudience { get; set; }

        public List<IntentData> Pattern { get; set; } = new List<IntentData>();
    }
}
