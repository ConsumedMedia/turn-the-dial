using System.Globalization;

namespace TurnTheDial.Core.Content
{
    /// <summary>Culture-invariant number text, so logs are identical on every machine.</summary>
    public static class NumberFormat
    {
        public static string Int(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>A percent factor as a multiplier: 150 → "1.5", 120 → "1.2", 75 → "0.75", 100 → "1".</summary>
        public static string Factor(int percent)
        {
            int whole = percent / CardRules.PercentBase;
            int frac = percent % CardRules.PercentBase;
            if (frac == 0)
            {
                return Int(whole);
            }

            string fracText = frac.ToString("00", CultureInfo.InvariantCulture).TrimEnd('0');
            return Int(whole) + "." + fracText;
        }

        /// <summary>A share as a percent with one decimal, rounded down: 70/85 → "82.3%".</summary>
        public static string Share(long numerator, long denominator)
        {
            if (denominator <= 0)
            {
                return "0.0%";
            }

            long perMille = numerator * 1000 / denominator;
            return Int(perMille / 10) + "." + Int(perMille % 10) + "%";
        }
    }
}
