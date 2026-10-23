using TMPro;
using UnityEngine;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>A focusable number on the board (status, resource, intent) with a tooltip.</summary>
    [RequireComponent(typeof(TooltipSource))]
    public sealed class Readout : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TooltipSource tooltip;

        public void Set(string text, string tooltipText)
        {
            if (label.text != text) label.text = text;
            tooltip.Text = tooltipText;
        }

        public void SetColor(Color color)
        {
            label.color = color;
        }
    }
}
