using TurnTheDial.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>The rival's teal diamond blinks (brief §11 color rule: your lights are steady, the rival's blink).</summary>
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
            float hz = uiConfig != null ? uiConfig.rivalBlinkHz : 0f;
            if (hz <= 0f) return;
            // On for the first half of each period, off for the second (unscaled time: UI only).
            bool on = Mathf.Repeat(Time.unscaledTime * hz, 1f) < 0.5f;
            Color c = _baseColor;
            c.a = on ? _baseColor.a : 0f;
            graphic.color = c;
        }
    }
}
