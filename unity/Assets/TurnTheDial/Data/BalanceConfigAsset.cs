using TurnTheDial.Core.Content;
using UnityEngine;

namespace TurnTheDial.Data
{
    /// <summary>
    /// Fight tunables, one field per key in the design docs' Tunable values tables. Fractions are
    /// whole percents (0.10 = 10). Starting values live in the asset, not in code.
    /// </summary>
    [CreateAssetMenu(menuName = "Turn the Dial/Balance Config", fileName = "BalanceConfig")]
    public sealed class BalanceConfigAsset : ScriptableObject
    {
        [Header("combat.md")]
        [Tooltip("turns.daypart")] public int turnsDaypart;
        [Tooltip("turns.primeTime")] public int turnsPrimeTime;
        [Tooltip("turns.boss")] public int turnsBoss;
        [Tooltip("airtime.perTurn")] public int airtimePerTurn;
        [Tooltip("draw.perTurn")] public int drawPerTurn;
        [Tooltip("hand.limit")] public int handLimit;
        [Tooltip("reach.listenerConversion, percent")] public int listenerConversionPercent;
        [Tooltip("exposed.factor, percent")] public int exposedPercent;
        [Tooltip("deadAir.steps")] public int[] deadAirSteps;
        [Tooltip("ratingsLoss.penalty, percent, rounded up")] public int ratingsLossPenaltyPercent;
        [Tooltip("cueSlots")] public int cueSlots;

        [Header("flow.md")]
        [Tooltip("flow.cap")] public int flowCap;
        [Tooltip("flow.capLoop")] public int flowCapLoop;
        [Tooltip("flow.reachPerStack, percent")] public int flowReachPerStackPercent;
        [Tooltip("flow.rampGain")] public int flowRampGain;
        [Tooltip("flow.streakGain")] public int flowStreakGain;
        [Tooltip("hype.decayPerTurn")] public int hypeDecayPerTurn;

        [Header("statuses.md")]
        [Tooltip("cash.start")] public int cashStart;
        [Tooltip("requestTokens.start")] public int requestTokensStart;
        [Tooltip("buzz.stealBonus, percent")] public int buzzStealBonusPercent;
        [Tooltip("burnout.perStack, percent")] public int burnoutPerStackPercent;
        [Tooltip("burnout.floor, percent")] public int burnoutFloorPercent;

        [Header("formats.md")]
        [Tooltip("anthem.reachFactor, percent")] public int anthemReachPercent;
        [Tooltip("anthem.loyaltyPerTempo")] public int anthemLoyaltyPerTempo;
        [Tooltip("mailbag.conversion, percent")] public int mailbagConversionPercent;

        public BalanceConfig ToCore()
        {
            return new BalanceConfig
            {
                TurnsDaypart = turnsDaypart,
                TurnsPrimeTime = turnsPrimeTime,
                TurnsBoss = turnsBoss,
                AirtimePerTurn = airtimePerTurn,
                DrawPerTurn = drawPerTurn,
                HandLimit = handLimit,
                ListenerConversionPercent = listenerConversionPercent,
                ExposedPercent = exposedPercent,
                DeadAirSteps = deadAirSteps == null ? new int[0] : (int[])deadAirSteps.Clone(),
                RatingsLossPenaltyPercent = ratingsLossPenaltyPercent,
                CueSlots = cueSlots,
                FlowCap = flowCap,
                FlowCapLoop = flowCapLoop,
                FlowReachPerStackPercent = flowReachPerStackPercent,
                FlowRampGain = flowRampGain,
                FlowStreakGain = flowStreakGain,
                HypeDecayPerTurn = hypeDecayPerTurn,
                CashStart = cashStart,
                RequestTokensStart = requestTokensStart,
                BuzzStealBonusPercent = buzzStealBonusPercent,
                BurnoutPerStackPercent = burnoutPerStackPercent,
                BurnoutFloorPercent = burnoutFloorPercent,
                AnthemReachPercent = anthemReachPercent,
                AnthemLoyaltyPerTempo = anthemLoyaltyPerTempo,
                MailbagConversionPercent = mailbagConversionPercent,
            };
        }
    }
}
