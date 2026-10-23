namespace TurnTheDial.Core.Content
{
    /// <summary>
    /// Proposed (statuses.md): Cash never goes negative. An action whose Cash cost is greater than
    /// the Cash you have is unavailable rather than clamped.
    /// </summary>
    public static class CashRules
    {
        public static bool CanPay(int cash, int cost)
        {
            return cost >= 0 && cost <= cash;
        }

        /// <summary>Returns false and leaves cash unchanged when the cost can't be paid.</summary>
        public static bool TryPay(ref int cash, int cost)
        {
            if (!CanPay(cash, cost))
            {
                return false;
            }

            cash -= cost;
            return true;
        }
    }
}
