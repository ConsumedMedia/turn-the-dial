namespace TurnTheDial.Core.Content
{
    /// <summary>Board terms that get a tooltip (ui.md: every keyword and calculated number).</summary>
    public enum Term
    {
        Listeners = 0,
        RivalAudience = 1,
        Airtime = 2,
        Cash = 3,
        Loyalty = 4,
        Lock = 5,
        Flow = 6,
        Hype = 7,
        Buzz = 8,
        Burnout = 9,
        Exposed = 10,
        Static = 11,
        DeadAir = 12,
        CueSlot = 13,
        DrawPile = 14,
        DiscardPile = 15,
        Turn = 16,
        Intent = 17,
    }

    /// <summary>
    /// Tooltip text for keywords and board terms (brief §3, §4). Numbers come from the balance
    /// config so tooltips never drift from the rules.
    /// </summary>
    public static class Glossary
    {
        public static string KeywordText(Keyword keyword, BalanceConfig b)
        {
            switch (keyword)
            {
                case Keyword.OnAir:
                    return "On Air: counts against Dead Air. End a turn without playing an On Air card and you take Dead Air.";
                case Keyword.Drop:
                    return "Drop N: the card deals one number, (base + Hype if a Song + N x Flow), with no Flow multiplier, as a single hit. Then Flow goes to 0.";
                case Keyword.Anthem:
                    return "Anthem: the first Anthem Song you play each turn gets x" + NumberFormat.Factor(b.AnthemReachPercent)
                        + " Reach (after the Flow multiplier) and gives Loyalty equal to its tempo x " + NumberFormat.Int(b.AnthemLoyaltyPerTempo) + ".";
                case Keyword.Hold:
                    return "Hold: stays in hand at end of turn.";
                case Keyword.Monitor:
                    return "Monitor N: on intents, you see the rival's next N intents for the rest of the daypart (always N ahead). On your draw pile, a one-time look at the top N cards.";
                case Keyword.Pulled:
                    return "Pulled: removed for the rest of the daypart once played.";
                case Keyword.Sticky:
                    return "Sticky: Interference that stays in your deck after the daypart.";
                default:
                    return "Unplayable: can't be played.";
            }
        }

        public static string TermText(Term term, BalanceConfig b) => TermText(term, b, b.FlowCap);

        /// <param name="flowCap">The fight's current Flow cap (Loop, gear and cards can raise it).</param>
        public static string TermText(Term term, BalanceConfig b, int flowCap)
        {
            switch (term)
            {
                case Term.Listeners:
                    return "Listeners: your health for the whole run. Reach that gets past Lock converts "
                        + NumberFormat.Int(b.ListenerConversionPercent) + "% (rounded down) into listeners, up to your maximum. 0 ends the run.";
                case Term.RivalAudience:
                    return "Rival audience: the rival's health. 0 and you win. At the ratings check, shares are compared against its starting audience.";
                case Term.Airtime:
                    return "Airtime: " + NumberFormat.Int(b.AirtimePerTurn) + " per turn, spent to play cards. Unspent Airtime is lost at end of turn.";
                case Term.Cash:
                    return "Cash: earned mostly from Ads, spent at Sponsor Meetings. Kept for the run.";
                case Term.Loyalty:
                    return "Loyalty: absorbs rival steals before your listeners take any, and nothing else. Clears at the start of your turn.";
                case Term.Lock:
                    return "Lock: the rival's block. Absorbs your Reach first, once per card. Clears at the start of the rival phase.";
                case Term.Flow:
                    return "Flow: +" + NumberFormat.Int(b.FlowReachPerStackPercent) + "% Song Reach per stack, cap " + NumberFormat.Int(flowCap)
                        + ". +" + NumberFormat.Int(b.FlowRampGain) + " when a Song's tempo is higher than the last song's, +" + NumberFormat.Int(b.FlowStreakGain)
                        + " when it and the last song are your format's genre. Breaks to 0 on an Ad, Dead Air or a Drop.";
                case Term.Hype:
                    return "Hype: each stack gives every Song +1 Reach (before the Flow multiplier). -" + NumberFormat.Int(b.HypeDecayPerTurn) + " at end of your turn.";
                case Term.Buzz:
                    return "Buzz: while above 0, the rival's steals take " + NumberFormat.Int(b.BuzzStealBonusPercent)
                        + "% more (rounded down, per hit). -1 at the end of the rival phase. Already in the intent number.";
                case Term.Burnout:
                    return "Burnout: each repeat play of a song name this daypart adds 1. -" + NumberFormat.Int(b.BurnoutPerStackPercent)
                        + "% Reach per stack, never below " + NumberFormat.Int(b.BurnoutFloorPercent) + "%.";
                case Term.Exposed:
                    return "Exposed: while the rival's shown intent is Commercial Break, your Reach against it gets x" + NumberFormat.Factor(b.ExposedPercent) + ".";
                case Term.Static:
                    return "Static: the number of Interference cards in your piles. They leave your deck when the daypart ends, except Sticky ones.";
                case Term.DeadAir:
                    return "Dead Air: ending a turn without an On Air card loses " + StepsText(b.DeadAirSteps)
                        + " listeners (first, second, each after). Ignores Loyalty. Flow drops to 0.";
                case Term.CueSlot:
                    return "Cue slot: once per turn, any time during your turn, move a card from your hand here for free (only if the slot is empty). A card cued this turn can be played from here next turn; it stays between turns.";
                case Term.DrawPile:
                    return "Draw pile: when it runs out, the discard pile is shuffled into it.";
                case Term.DiscardPile:
                    return "Discard pile: played and discarded cards.";
                case Term.Turn:
                    return "Turns: after the last rival phase, the ratings check compares shares (listeners / max vs audience / start). A tie goes to you.";
                default:
                    return "Intent: the rival's next move with its exact final number (Buzz and Exposed included).";
            }
        }

        private static string StepsText(int[] steps)
        {
            if (steps == null || steps.Length == 0)
            {
                return "";
            }

            string text = "";
            for (int i = 0; i < steps.Length; i++)
            {
                text += (i > 0 ? " / " : "") + NumberFormat.Int(steps[i]);
            }

            return text;
        }
    }
}
