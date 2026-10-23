using System;
using System.Globalization;
using TurnTheDial.Core.Combat;
using TurnTheDial.Core.Content;
using TurnTheDial.Core.Rng;
using TurnTheDial.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// Owns the prototype fight. Exports the content assets to Core, sends player actions to Core
    /// as commands, and never decides a rule. Debug keys (development only, ui.md; Decided, user,
    /// 2026-10-23): F5 restart with a new seed, F6 show the seed, F7 draw, F8 win, F9 lose.
    /// </summary>
    public sealed class ProtoCombatController : MonoBehaviour
    {
        [SerializeField] private FightContent content;
        [Tooltip("First fight's seed. F5 picks the next seed here in the Game layer from the current one via SeedChain (no clock time); not a run stream, never saved (save.md).")]
        [SerializeField] private string initialSeed = "1";

        private bool _endTurnConfirmPending;

        public event Action FightChanged;
        public event Action FightReset;

        public Fight Fight { get; private set; }

        public ulong Seed { get; private set; }

        /// <summary>Last message for the player (rejected command, confirm prompt, debug info).</summary>
        public string Message { get; private set; } = "";

        private void Start()
        {
            if (!ulong.TryParse(initialSeed, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong seed))
            {
                seed = 1;
            }

            NewFight(seed);
        }

        private void Update()
        {
            // Debug keys are development builds only (ui.md). Editor and development builds report true.
            if (!Debug.isDebugBuild) return;
            Keyboard kb = Keyboard.current;
            if (kb == null) return;
            if (kb.f5Key.wasPressedThisFrame) DebugRestart();
            if (kb.f6Key.wasPressedThisFrame) DebugShowSeed();
            if (kb.f7Key.wasPressedThisFrame) { Message = ""; Report(Fight.DebugDraw()); }
            if (kb.f8Key.wasPressedThisFrame) { Message = ""; Report(Fight.DebugWin()); }
            if (kb.f9Key.wasPressedThisFrame) { Message = ""; Report(Fight.DebugLose()); }
        }

        public void NewFight(ulong seed)
        {
            if (Fight != null)
            {
                Fight.StateChanged -= OnStateChanged;
                Fight.DrawMonitored -= OnDrawMonitored;
            }
            Seed = seed;
            Fight = new Fight(ContentExporter.BuildSetup(content, seed));
            Fight.StateChanged += OnStateChanged;
            Fight.DrawMonitored += OnDrawMonitored;
            _endTurnConfirmPending = false;
            Message = "";
            FightReset?.Invoke();
            Fight.Start();
        }

        public void PlayCard(int uid)
        {
            _endTurnConfirmPending = false;
            Message = "";
            Report(Fight.PlayCard(uid));
        }

        public void Cue(int uid)
        {
            _endTurnConfirmPending = false;
            Message = "";
            Report(Fight.Cue(uid));
        }

        /// <summary>
        /// Ends the turn. Proposed (ui.md): when it would cause Dead Air, the first press asks for
        /// confirmation and the second press ends the turn. Core says whether Dead Air would happen.
        /// </summary>
        public void EndTurn()
        {
            if (Fight.IsOver)
            {
                Report(CommandResult.Fail("The fight is over. Press F5 to restart."));
                return;
            }

            if (Fight.WouldTakeDeadAir && !_endTurnConfirmPending)
            {
                _endTurnConfirmPending = true;
                Message = "No On Air card played: ending now causes Dead Air (lose " + Fight.NextDeadAirLoss + " listeners, Flow to 0). End the turn again to confirm.";
                FightChanged?.Invoke();
                return;
            }

            _endTurnConfirmPending = false;
            Message = "";
            Report(Fight.EndTurn());
        }

        public void DebugRestart()
        {
            NewFight(SeedChain.Next(Seed));
            Message = "[debug] Restarted with seed " + Seed.ToString(CultureInfo.InvariantCulture) + ".";
            FightChanged?.Invoke();
        }

        public void DebugShowSeed()
        {
            string text = "[debug] Seed: " + Seed.ToString(CultureInfo.InvariantCulture);
            Debug.Log(text);
            Message = text;
            Fight.AddNote(text);
        }

        private void Report(CommandResult result)
        {
            if (!result.Ok)
            {
                Message = result.Reason;
                FightChanged?.Invoke();
            }
        }

        /// <summary>A Monitor draw-pile peek is a one-time look: shown as the message, not kept (brief §4).</summary>
        private void OnDrawMonitored(System.Collections.Generic.IReadOnlyList<CardInstance> top)
        {
            var names = new System.Collections.Generic.List<string>();
            for (int i = 0; i < top.Count; i++) names.Add(top[i].Card.DisplayName);
            Message = "Monitor, top of your draw pile: " + (names.Count > 0 ? string.Join(", ", names) : "(empty)") + ".";
        }

        private void OnStateChanged()
        {
            FightChanged?.Invoke();
        }
    }
}
