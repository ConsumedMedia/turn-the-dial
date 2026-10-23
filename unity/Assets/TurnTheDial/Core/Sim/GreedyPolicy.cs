using System.Collections.Generic;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;

namespace TurnTheDial.Core.Sim
{
    /// <summary>
    /// Simple deterministic bot (sim only, not a game rule):
    /// 1. If any playable card's Reach would finish the rival (after Lock), play it.
    /// 2. Play 0-cost draw Liners first.
    /// 3. Play On Air cards greedily: the best Reach song (Anthem first while unused), and DJ Breaks
    ///    for Loyalty when the shown intent steals. Interference that costs listeners at end of turn
    ///    is played off if Airtime allows.
    /// 4. Ads only when listeners are comfortably above the Ad's cost.
    /// 5. Cue the leftover card with the highest Reach (or any non-Interference card).
    /// Ties break on hand order, so the bot is deterministic.
    /// </summary>
    public sealed class GreedyPolicy : IFightPolicy
    {
        /// <summary>Sim policy setting, not a game rule: play an Ad only above this share of max listeners (percent).</summary>
        public int AdListenerFloorPercent { get; set; } = 60;

        public string Name => "Greedy (finish > draw liners > best Reach / Loyalty vs steals > clear Complaint Calls > Ads above " + AdListenerFloorPercent + "% listeners > cue best leftover)";

        public void PlayTurn(Fight fight)
        {
            for (int guard = 0; guard < FightSimulator.MaxActionsPerTurn && !fight.IsOver; guard++)
            {
                int uid = Choose(fight);
                if (uid < 0) break;
                if (!fight.PlayCard(uid).Ok) break;
            }

            if (fight.IsOver || fight.Phase != FightPhase.Player || fight.CueSlot.Count > 0) return;
            int cueUid = ChooseCue(fight);
            if (cueUid >= 0) fight.Cue(cueUid);
        }

        private int Choose(Fight fight)
        {
            List<CardInstance> playable = Playable(fight);
            if (playable.Count == 0) return -1;

            // 1. Finisher.
            for (int i = 0; i < playable.Count; i++)
            {
                int reach = fight.PreviewReach(playable[i].Uid, out _);
                if (reach > 0 && reach - fight.Lock >= fight.RivalAudience) return playable[i].Uid;
            }

            // 2. Free draw.
            for (int i = 0; i < playable.Count; i++)
            {
                CardData c = playable[i].Card;
                if (c.Cost == 0 && c.Type == CardType.Liner && HasEffect(c, EffectKind.DrawCards)) return playable[i].Uid;
            }

            bool steals = fight.CurrentIntent != null
                && (fight.CurrentIntent.Kind == IntentKind.Steal || fight.CurrentIntent.Kind == IntentKind.BigSegment);
            int stealTotal = steals ? fight.PreviewIntent().Total : 0;

            // 3. Best On Air value: Reach for songs, Loyalty (up to the coming steal) for DJ Breaks.
            int best = -1;
            int bestScore = 0;
            for (int i = 0; i < playable.Count; i++)
            {
                CardData c = playable[i].Card;
                if (!c.Keywords.OnAir) continue;
                int score = fight.PreviewReach(playable[i].Uid, out _);
                if (c.Loyalty > 0 && steals)
                {
                    int useful = System.Math.Max(0, System.Math.Min(c.Loyalty, stealTotal - fight.Loyalty));
                    score = System.Math.Max(score, useful);
                }

                if (score > bestScore)
                {
                    best = playable[i].Uid;
                    bestScore = score;
                }
            }

            if (best >= 0) return best;

            // Any On Air card left avoids Dead Air.
            if (!fight.OnAirPlayedThisTurn)
            {
                for (int i = 0; i < playable.Count; i++)
                {
                    if (playable[i].Card.Keywords.OnAir) return playable[i].Uid;
                }
            }

            // 4. Clear Interference that hurts at end of turn.
            for (int i = 0; i < playable.Count; i++)
            {
                if (playable[i].Card.EndOfTurnInHand.Count > 0) return playable[i].Uid;
            }

            // 5. Ads when listeners are healthy.
            for (int i = 0; i < playable.Count; i++)
            {
                CardData c = playable[i].Card;
                if (c.Type == CardType.Ad && fight.Listeners * 100 >= fight.MaxListeners * AdListenerFloorPercent && fight.Listeners > c.ListenerCost)
                {
                    return playable[i].Uid;
                }
            }

            return -1;
        }

        private static int ChooseCue(Fight fight)
        {
            int best = -1;
            int bestScore = -1;
            for (int i = 0; i < fight.Hand.Count; i++)
            {
                CardInstance inst = fight.Hand[i];
                if (inst.IsInterference) continue;
                int score = inst.Card.Reach + inst.Card.Keywords.Drop + inst.Card.Loyalty;
                if (score > bestScore)
                {
                    best = inst.Uid;
                    bestScore = score;
                }
            }

            return best;
        }

        private static List<CardInstance> Playable(Fight fight)
        {
            var list = new List<CardInstance>();
            for (int i = 0; i < fight.CueSlot.Count; i++)
            {
                if (fight.CanPlay(fight.CueSlot[i].Uid, out _)) list.Add(fight.CueSlot[i]);
            }

            for (int i = 0; i < fight.Hand.Count; i++)
            {
                if (fight.CanPlay(fight.Hand[i].Uid, out _)) list.Add(fight.Hand[i]);
            }

            return list;
        }

        private static bool HasEffect(CardData c, EffectKind kind)
        {
            for (int i = 0; i < c.Effects.Count; i++)
            {
                if (c.Effects[i].Kind == kind) return true;
            }

            return false;
        }
    }
}
