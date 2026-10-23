using TMPro;
using UnityEngine;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// One fixed tooltip area. Shows the tooltip of whatever is hovered (mouse) or focused
    /// (keyboard / gamepad). T / Y hides or shows it.
    /// </summary>
    public sealed class TooltipPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text body;
        [SerializeField] private GameObject panel;

        private TooltipSource _hovered;
        private TooltipSource _focused;
        private bool _hiddenByPlayer;

        public void SetHovered(TooltipSource source)
        {
            _hovered = source;
            Refresh();
        }

        public void ClearHovered(TooltipSource source)
        {
            if (_hovered == source) _hovered = null;
            Refresh();
        }

        public void SetFocused(TooltipSource source)
        {
            _focused = source;
            Refresh();
        }

        public void ClearFocused(TooltipSource source)
        {
            if (_focused == source) _focused = null;
            Refresh();
        }

        public void ToggleVisible()
        {
            _hiddenByPlayer = !_hiddenByPlayer;
            Refresh();
        }

        public void Refresh()
        {
            TooltipSource source = _hovered != null && _hovered.isActiveAndEnabled ? _hovered : _focused;
            string text = source != null && source.isActiveAndEnabled ? source.Text : null;
            bool show = !_hiddenByPlayer && !string.IsNullOrEmpty(text);
            panel.SetActive(show);
            if (show) body.text = text;
        }
    }
}
