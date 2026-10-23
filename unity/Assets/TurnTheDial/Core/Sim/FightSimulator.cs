using System;
using System.Collections.Generic;
using System.Text;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;

namespace TurnTheDial.Core.Sim
{
    /// <summary>A bot that plays one fight through Core's commands only.</summary>
    public interface IFightPolicy
    {
        string Name { get; }

        /// <summary>Plays the current player turn (cards, optional cue). The simulator ends the turn.</summary>
        void PlayTurn(Fight fight);
    }

    /// <summary>One simulated fight's summary.</summary>
    public sealed class SimFightResult
    {
        public ulong Seed;
        public FightOutcome Outcome;
        public int TurnsPlayed;
        public int FinalListeners;
        public int MaxListeners;
        public int FinalRivalAudience;
        public int RivalStartingAudience;
        public int DeadAirCount;
        public int RatingsLossPenalty;
        public IReadOnlyList<string> Log;
    }

    /// <summary>
    /// Headless fight runner (tools/sim). Pure Core: no Unity, no IO. Callers write the report.
    /// </summary>
    public static class FightSimulator
    {
        /// <summary>Hard stop so a broken policy can't loop forever.</summary>
        public const int MaxActionsPerTurn = 64;

        public static SimFightResult RunOne(Func<ulong, FightSetup> setupForSeed, ulong seed, IFightPolicy policy)
        {
            FightSetup setup = setupForSeed(seed);
            var fight = new Fight(setup);
            fight.Start();
            while (!fight.IsOver)
            {
                policy.PlayTurn(fight);
                if (!fight.IsOver)
                {
                    fight.EndTurn();
                }
            }

            return new SimFightResult
            {
                Seed = seed,
                Outcome = fight.Outcome,
                TurnsPlayed = fight.Turn,
                FinalListeners = fight.Listeners,
                MaxListeners = fight.MaxListeners,
                FinalRivalAudience = fight.RivalAudience,
                RivalStartingAudience = fight.RivalStartingAudience,
                DeadAirCount = fight.DeadAirCount,
                RatingsLossPenalty = fight.RatingsLossPenalty,
                Log = fight.Log.Lines,
            };
        }

        /// <summary>Runs <paramref name="count"/> seeds: the first seed, then each next one from <see cref="SeedChain"/>.</summary>
        public static List<SimFightResult> RunMany(Func<ulong, FightSetup> setupForSeed, ulong firstSeed, int count, IFightPolicy policy)
        {
            var results = new List<SimFightResult>();
            ulong seed = firstSeed;
            for (int i = 0; i < count; i++)
            {
                results.Add(RunOne(setupForSeed, seed, policy));
                seed = SeedChain.Next(seed);
            }

            return results;
        }

        public static string OutcomeText(FightOutcome outcome)
        {
            switch (outcome)
            {
                case FightOutcome.WonAudienceZero: return "win (rival audience 0)";
                case FightOutcome.WonRatings: return "ratings win";
                case FightOutcome.LostRatings: return "ratings loss";
                case FightOutcome.LostListenersZero: return "loss (listeners 0)";
                default: return "unfinished";
            }
        }

        /// <summary>Markdown report. Includes the full log of the first fight.</summary>
        public static string Report(string title, string policyName, IReadOnlyList<SimFightResult> results, bool includeFirstLog)
        {
            var sb = new StringBuilder();
            sb.Append("# ").Append(title).Append('\n').Append('\n');
            sb.Append("Policy: ").Append(policyName).Append('\n').Append('\n');
            sb.Append("| Seed | Turns played | Result | Final listeners | Rival audience | Dead Air | Ratings-loss penalty |\n");
            sb.Append("| --- | --- | --- | --- | --- | --- | --- |\n");
            int wins = 0;
            for (int i = 0; i < results.Count; i++)
            {
                SimFightResult r = results[i];
                bool won = r.Outcome == FightOutcome.WonAudienceZero || r.Outcome == FightOutcome.WonRatings;
                if (won) wins++;
                sb.Append("| ").Append(r.Seed.ToString(System.Globalization.CultureInfo.InvariantCulture))
                  .Append(" | ").Append(NumberFormat.Int(r.TurnsPlayed))
                  .Append(" | ").Append(OutcomeText(r.Outcome))
                  .Append(" | ").Append(NumberFormat.Int(r.FinalListeners)).Append("/").Append(NumberFormat.Int(r.MaxListeners))
                  .Append(" | ").Append(NumberFormat.Int(r.FinalRivalAudience)).Append("/").Append(NumberFormat.Int(r.RivalStartingAudience))
                  .Append(" | ").Append(NumberFormat.Int(r.DeadAirCount))
                  .Append(" | ").Append(NumberFormat.Int(r.RatingsLossPenalty))
                  .Append(" |\n");
            }

            sb.Append('\n').Append("Wins: ").Append(NumberFormat.Int(wins)).Append(" of ").Append(NumberFormat.Int(results.Count)).Append('\n');
            if (includeFirstLog && results.Count > 0)
            {
                sb.Append('\n').Append("## Log, seed ").Append(results[0].Seed.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("\n\n```\n");
                for (int i = 0; i < results[0].Log.Count; i++)
                {
                    sb.Append(results[0].Log[i]).Append('\n');
                }

                sb.Append("```\n");
            }

            return sb.ToString();
        }
    }
}
