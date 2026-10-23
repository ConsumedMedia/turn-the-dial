namespace TurnTheDial.Core.Content
{
    /// <summary>Fixed ranges the brief states as rules, not tunables (brief §3, §4).</summary>
    public static class CardRules
    {
        /// <summary>Card costs are 0 to 3 (brief §3 Airtime row, §4 cost).</summary>
        public const int CostMin = 0;
        public const int CostMax = 3;

        /// <summary>Liners cost 0 to 1 (brief §4 type defaults).</summary>
        public const int LinerCostMax = 1;

        /// <summary>Song tempo 1 to 5 (brief §4). Loop's wrap is tempo 5 to tempo 1 (brief §5).</summary>
        public const int TempoMin = 1;
        public const int TempoMax = 5;

        /// <summary>Percent base for factor math (100 = ×1).</summary>
        public const int PercentBase = 100;
    }
}
