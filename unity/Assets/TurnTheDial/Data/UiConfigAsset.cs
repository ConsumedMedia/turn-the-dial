using UnityEngine;

namespace TurnTheDial.Data
{
    /// <summary>UI tunables (ui.md Tunable values). Presentation only; never a game rule.</summary>
    [CreateAssetMenu(menuName = "Turn the Dial/UI Config", fileName = "UiConfig")]
    public sealed class UiConfigAsset : ScriptableObject
    {
        [Tooltip("ui.rivalBlinkHz (Proposed, ui.md)")] public float rivalBlinkHz;
        [Tooltip("ui.minTextPx at 1280x800")] public int minTextPx;
        [Tooltip("Combat log lines kept on screen (prototype display only).")] public int logLinesShown;
        [Tooltip("On-air light flicker rate in its warning state (prototype display only).")] public float onAirWarningFlickerHz;
    }
}
