namespace TurnTheDial.Core.Content
{
    /// <summary>A card's keywords (brief §4). Monitor N lives in the effect list (MonitorIntents /
    /// MonitorDraw) because the card states which one it peeks.</summary>
    public sealed class CardKeywords
    {
        public bool OnAir { get; set; }

        /// <summary>Drop N. 0 means no Drop.</summary>
        public int Drop { get; set; }

        public bool Anthem { get; set; }

        public bool Hold { get; set; }

        public bool Pulled { get; set; }

        public bool Sticky { get; set; }

        public bool Unplayable { get; set; }

        public CardKeywords Clone()
        {
            return (CardKeywords)MemberwiseClone();
        }
    }
}
