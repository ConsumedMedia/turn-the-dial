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
    /// as commands, and never decides a rule. Debug keys (prototype only): R restart with the next
    /// seed, S show the seed, D draw, W win, L lose.
    /// </summary>
    public sealed class ProtoCombatController : MonoBehaviour
    {
        [SerializeField] private FightContent content;
        [Tooltip("First fight's seed. R derives the next seed from the current one through Core's SeedChain (no clock time).")]
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
            Keyboard kb = Keyboard.current;
            if (kb == null) return;
            if (kb.rKey.wasPressedThisFrame) DebugRestart();
            if (kb.sKey.wasPressedThisFrame) DebugShowSeed();
            if (kb.dKey.wasPressedThisFrame) { Message = ""; Report(Fight.DebugDraw()); }
            if (kb.wKey.wasPressedThisFrame) { Message = ""; Report(Fight.DebugWin()); }
            if (kb.lKey.wasPressedThisFrame) { Message = ""; Report(Fight.DebugLose()); }
        }

        public void NewFight(ulong seed)
        {
            if (Fight != null) Fight.StateChanged -= OnStateChanged;
            Seed = seed;
            Fight = new Fight(ContentExporter.BuildSetup(content, seed));
            Fight.StateChanged += OnStateChanged;
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
                Report(CommandResult.Fail("The fight is over. Press R to restart."));
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

        private void OnStateChanged()
        {
            FightChanged?.Invoke();
        }
    }
}
