namespace TurnTheDial.Core.Content
{
    /// <summary>
    /// Every fight tunable from the design docs' Tunable values tables. Core holds no starting
    /// values: they live in the balance config asset (TurnTheDial.Data) and in test fixtures.
    /// Fractions are stored as whole percents (0.10 = 10) so all math is exact integer math.
    /// </summary>
    public sealed class BalanceConfig
    {
        // combat.md
        public int TurnsDaypart { get; set; }
        public int TurnsPrimeTime { get; set; }
        public int TurnsBoss { get; set; }
        public int AirtimePerTurn { get; set; }
        public int DrawPerTurn { get; set; }
        public int HandLimit { get; set; }
        public int ListenerConversionPercent { get; set; }
        public int ExposedPercent { get; set; }
        public int[] DeadAirSteps { get; set; }
        public int RatingsLossPenaltyPercent { get; set; }
        public int CueSlots { get; set; }

        // flow.md
        public int FlowCap { get; set; }
        public int FlowCapLoop { get; set; }
        public int FlowReachPerStackPercent { get; set; }
        public int FlowRampGain { get; set; }
        public int FlowStreakGain { get; set; }
        public int HypeDecayPerTurn { get; set; }

        // statuses.md
        public int CashStart { get; set; }
        public int RequestTokensStart { get; set; }
        public int BuzzStealBonusPercent { get; set; }
        public int BurnoutPerStackPercent { get; set; }
        public int BurnoutFloorPercent { get; set; }

        // formats.md
        public int AnthemReachPercent { get; set; }
        public int AnthemLoyaltyPerTempo { get; set; }
        public int MailbagConversionPercent { get; set; }

        public int TurnsFor(FightKind kind)
        {
            switch (kind)
            {
                case FightKind.PrimeTime: return TurnsPrimeTime;
                case FightKind.Boss: return TurnsBoss;
                default: return TurnsDaypart;
            }
        }
    }
}
