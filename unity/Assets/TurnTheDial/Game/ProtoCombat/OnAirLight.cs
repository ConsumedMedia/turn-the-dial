using TMPro;
using TurnTheDial.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// The red on-air light (brief §10, §11): steady while on air; flickers and dims as the danger
    /// warning when ending the turn would cause Dead Air or the shown steal would end the run.
    /// Core decides both conditions. Reduce motion (brief §10, ui.md) turns the flicker off: the
    /// warning lamp stays dimmed instead.
    /// </summary>
    public sealed class OnAirLight : MonoBehaviour
    {
        [SerializeField] private Image lamp;
        [SerializeField] private TMP_Text label;
        [SerializeField] private UiConfigAsset uiConfig;
        [SerializeField] private Color onColor = new Color(0.86f, 0.12f, 0.10f);
        [SerializeField] private Color dimColor = new Color(0.35f, 0.06f, 0.05f);

        private bool _warning;

        /// <param name="offAir">The fight is over: the lamp is dark.</param>
        public void Set(bool warning, bool offAir, string text)
        {
            _warning = warning && !offAir;
            label.text = text;
            if (offAir) lamp.color = dimColor;
            else if (!_warning) lamp.color = onColor;
        }

        private void Update()
        {
            if (!_warning) return;
            if (uiConfig != null && uiConfig.reduceMotion)
            {
                lamp.color = dimColor;
                return;
            }

            float hz = uiConfig != null ? uiConfig.onAirFlickerHz : 0f;
            bool bright = hz > 0f && Mathf.Repeat(Time.unscaledTime * hz, 1f) < 0.5f;
            lamp.color = bright ? onColor : dimColor;
        }
    }
}
