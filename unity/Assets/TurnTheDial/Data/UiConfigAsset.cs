using UnityEngine;
using UnityEngine.Serialization;

namespace TurnTheDial.Data
{
    /// <summary>UI tunables (ui.md Tunable values). Presentation only; never a game rule.</summary>
    [CreateAssetMenu(menuName = "Turn the Dial/UI Config", fileName = "UiConfig")]
    public sealed class UiConfigAsset : ScriptableObject
    {
        [Tooltip("ui.rivalBlinkHz (Proposed, ui.md)")] public float rivalBlinkHz;
        [Tooltip("ui.minTextPx at 1280x800")] public int minTextPx;
        [Tooltip("ui.logLines: combat log lines kept on the clipboard (Decided, user, 2026-10-23)")]
        [FormerlySerializedAs("logLinesShown")] public int logLines;
        [Tooltip("ui.onAirFlickerHz: on-air warning flicker, under 3 flashes/second (Decided, user, 2026-10-23)")]
        [FormerlySerializedAs("onAirWarningFlickerHz")] public float onAirFlickerHz;
        [Tooltip("Reduce motion (brief §10): turns off the on-air flicker; the warning still dims and the rival still blinks. Settings toggle later; default off.")]
        public bool reduceMotion;
    }
}
