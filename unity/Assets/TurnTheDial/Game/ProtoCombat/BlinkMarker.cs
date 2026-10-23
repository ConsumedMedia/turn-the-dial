using TurnTheDial.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>The rival's teal diamond blinks (brief §11 color rule: your lights are steady, the rival's blink);
    /// under reduce motion it pulses slowly instead (ui.md).</summary>
    [RequireComponent(typeof(Graphic))]
    public sealed class BlinkMarker : MonoBehaviour
    {
        [SerializeField] private UiConfigAsset uiConfig;
        [SerializeField] private Graphic graphic;

        private Color _baseColor;

        private void Awake()
        {
            if (graphic == null) graphic = GetComponent<Graphic>();
            _baseColor = graphic.color;
        }

        private void Update()
        {
            if (uiConfig == null) return;
            // Reduce motion (ui.md, Decided user 2026-10-23): a slow, smooth pulse instead of a sharp blink,
            // so the rival keeps its motion cue (brief §11).
            bool pulse = uiConfig.reduceMotion;
            float hz = pulse ? uiConfig.rivalPulseHz : uiConfig.rivalBlinkHz;
            if (hz <= 0f) return;
            float phase = Mathf.Repeat(Time.unscaledTime * hz, 1f); // unscaled time: UI only
            // Pulse: cosine ease from full to clear and back. Blink: on for the first half, off for the second.
            float level = pulse ? 0.5f + 0.5f * Mathf.Cos(phase * 2f * Mathf.PI) : (phase < 0.5f ? 1f : 0f);
            Color c = _baseColor;
            c.a = _baseColor.a * level;
            graphic.color = c;
        }
    }
}
