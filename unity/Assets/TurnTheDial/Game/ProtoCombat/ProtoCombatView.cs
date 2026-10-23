using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using TurnTheDial.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// Draws the fight from Core's read-only state. Every number and tooltip comes from Core;
    /// this class formats it and never computes a rule. You are on the left, the rival on the right.
    /// </summary>
    public sealed class ProtoCombatView : MonoBehaviour
    {
        [SerializeField] private ProtoCombatController controller;
        [SerializeField] private UiConfigAsset uiConfig;
        [SerializeField] private TooltipPanel tooltipPanel;

        [Header("You (left)")]
        [SerializeField] private TMP_Text youName;
        [SerializeField] private Readout listeners;
        [SerializeField] private Readout airtime;
        [SerializeField] private Readout flow;
        [SerializeField] private Readout hype;
        [SerializeField] private Readout loyalty;
        [SerializeField] private Readout cash;
        [SerializeField] private Readout deadAir;
        [SerializeField] private Readout staticCount;
        [SerializeField] private Readout drawPile;
        [SerializeField] private Readout discardPile;
        [SerializeField] private Readout burnout;
        [SerializeField] private OnAirLight onAirLight;

        [Header("Rival (right)")]
        [SerializeField] private TMP_Text rivalName;
        [SerializeField] private Readout audience;
        [SerializeField] private Readout intent;
        [SerializeField] private Readout rivalLock;
        [SerializeField] private Readout buzz;
        [SerializeField] private Readout exposed;

        [Header("Center")]
        [SerializeField] private Readout turn;
        [SerializeField] private CardView cueSlot;
        [SerializeField] private RectTransform hand;
        [SerializeField] private CardView cardTemplate;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TMP_Text message;
        [SerializeField] private TMP_Text result;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text seedLabel;

        [Header("Log")]
        [SerializeField] private TMP_Text log;
        [SerializeField] private ScrollRect logScroll;

        private readonly List<CardView> _cards = new List<CardView>();
        private bool _logDirty;

        private void Awake()
        {
            cardTemplate.gameObject.SetActive(false);
            endTurnButton.onClick.AddListener(() => controller.EndTurn());
            controller.FightReset += OnFightReset;
            controller.FightChanged += Refresh;
        }

        private void OnDestroy()
        {
            controller.FightReset -= OnFightReset;
            controller.FightChanged -= Refresh;
        }

        private void OnFightReset()
        {
            controller.Fight.Log.LineAdded += OnLogLine;
            _logDirty = true;
        }

        private void OnLogLine(string line)
        {
            _logDirty = true;
        }

        private void LateUpdate()
        {
            if (!_logDirty || controller.Fight == null) return;
            _logDirty = false;
            IReadOnlyList<string> lines = controller.Fight.Log.Lines;
            int shown = uiConfig != null && uiConfig.logLines > 0 ? uiConfig.logLines : lines.Count;
            int start = Mathf.Max(0, lines.Count - shown);
            var sb = new StringBuilder();
            for (int i = start; i < lines.Count; i++)
            {
                if (i > start) sb.Append('\n');
                sb.Append(lines[i]);
            }

            log.text = sb.ToString();
            Canvas.ForceUpdateCanvases();
            logScroll.verticalNormalizedPosition = 0f;
        }

        public void Refresh()
        {
            Fight f = controller.Fight;
            if (f == null) return;
            BalanceConfig b = f.Balance;

            youName.text = "YOU · " + f.Format.Name;
            rivalName.text = f.Rival.Name + " · RIVAL";
            seedLabel.text = "Seed " + controller.Seed.ToString(CultureInfo.InvariantCulture) + "  ·  Debug: F5 restart, F6 seed, F7 draw, F8 win, F9 lose";

            listeners.Set("Listeners " + f.Listeners + " / " + f.MaxListeners, Glossary.TermText(Term.Listeners, b));
            audience.Set("Audience " + f.RivalAudience + " / " + f.RivalStartingAudience, Glossary.TermText(Term.RivalAudience, b));
            turn.Set("Turn " + f.Turn + " of " + f.TurnsTotal, Glossary.TermText(Term.Turn, b));

            airtime.Set("Airtime " + f.Airtime, Glossary.TermText(Term.Airtime, b));
            flow.Set("Flow " + f.Flow + " / " + f.FlowCap, Glossary.TermText(Term.Flow, b, f.FlowCap)
                + "\nLast song: " + (f.LastSong != null ? f.LastSong.DisplayName + " (T" + f.LastSong.Tempo + ")" : "none"));
            hype.Set("Hype " + f.Hype, Glossary.TermText(Term.Hype, b));
            loyalty.Set("Loyalty " + f.Loyalty, Glossary.TermText(Term.Loyalty, b));
            cash.Set("Cash " + f.Cash, Glossary.TermText(Term.Cash, b));
            deadAir.Set("Dead Air " + f.DeadAirCount + " (next -" + f.NextDeadAirLoss + ")", Glossary.TermText(Term.DeadAir, b));
            staticCount.Set("Static " + f.StaticCount, Glossary.TermText(Term.Static, b));
            drawPile.Set("Draw pile " + f.DrawPile.Count, Glossary.TermText(Term.DrawPile, b));
            discardPile.Set("Discard " + f.DiscardPile.Count, Glossary.TermText(Term.DiscardPile, b));
            burnout.Set(BurnoutText(f), Glossary.TermText(Term.Burnout, b));

            IntentPreview p = f.PreviewIntent();
            string upcoming = UpcomingText(f);
            if (f.IsOver)
            {
                intent.Set("Next: -", Glossary.TermText(Term.Intent, b));
            }
            else if (p != null)
            {
                intent.Set("Next: " + p.Label, p.Label + "\n" + p.Math + upcoming);
            }

            rivalLock.Set("Lock " + f.Lock, Glossary.TermText(Term.Lock, b));
            buzz.Set("Buzz " + f.Buzz, Glossary.TermText(Term.Buzz, b));
            exposed.Set(f.IsRivalExposed ? "Exposed x" + NumberFormat.Factor(b.ExposedPercent) : "Exposed -", Glossary.TermText(Term.Exposed, b));

            bool warning = !f.IsOver && (f.WouldTakeDeadAir || f.NextStealWouldEndRun);
            string lightText = f.IsOver ? "OFF AIR" :
                f.NextStealWouldEndRun ? "ON AIR  ·  WARNING: the next steal ends the run" :
                f.WouldTakeDeadAir ? "ON AIR  ·  WARNING: ending now is Dead Air (-" + f.NextDeadAirLoss + ")" : "ON AIR";
            onAirLight.Set(warning, f.IsOver, lightText);

            RefreshHand(f);
            cueSlot.Bind(controller, f, f.CueSlot.Count > 0 ? f.CueSlot[0] : null, true);
            cueSlot.GetComponent<Button>().interactable = f.CueSlot.Count > 0;

            message.text = controller.Message;
            resultPanel.SetActive(f.IsOver);
            if (f.IsOver) result.text = OutcomeText(f) + "\nPress F5 for a new fight (next seed).";
            endTurnButton.interactable = !f.IsOver;
        }

        private void RefreshHand(Fight f)
        {
            while (_cards.Count < f.Hand.Count)
            {
                CardView view = Instantiate(cardTemplate, hand);
                view.gameObject.name = "Card " + _cards.Count;
                view.GetComponent<TooltipSource>().SetPanel(tooltipPanel);
                _cards.Add(view);
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                bool active = i < f.Hand.Count;
                if (_cards[i].gameObject.activeSelf != active) _cards[i].gameObject.SetActive(active);
                if (active) _cards[i].Bind(controller, f, f.Hand[i], false);
            }
        }

        private static string BurnoutText(Fight f)
        {
            var parts = new List<string>();
            for (int i = 0; i < f.Burnout.Count; i++)
            {
                if (f.Burnout[i].Stacks > 0) parts.Add(f.Burnout[i].Name + " " + f.Burnout[i].Stacks);
            }

            return parts.Count == 0 ? "Burnout -" : "Burnout " + string.Join(", ", parts);
        }

        private static string UpcomingText(Fight f)
        {
            if (f.MonitorIntentCount <= 0) return "";
            List<IntentPreview> next = f.PreviewUpcomingIntents(f.MonitorIntentCount);
            var sb = new StringBuilder("\nMonitor:");
            for (int i = 0; i < next.Count; i++)
            {
                sb.Append("\n  Turn ").Append(next[i].TurnNumber).Append(": ").Append(next[i].Label).Append(" (before Buzz)");
            }

            return sb.ToString();
        }

        private static string OutcomeText(Fight f)
        {
            switch (f.Outcome)
            {
                case FightOutcome.WonAudienceZero: return "YOU WIN: " + f.Rival.Name + "'s audience hit 0.";
                case FightOutcome.WonRatings: return "YOU WIN on the ratings check.";
                case FightOutcome.LostRatings:
                    return f.RunEnded
                        ? "RATINGS LOSS. Your listeners hit 0: the run ends."
                        : "RATINGS LOSS: lost " + f.RatingsLossPenalty + " listeners (" + f.Listeners + " left). The run would continue.";
                case FightOutcome.LostListenersZero: return "YOUR LISTENERS HIT 0: the run ends.";
                default: return "";
            }
        }
    }
}
