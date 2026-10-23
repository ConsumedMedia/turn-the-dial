using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>Anything with a tooltip: shows it on mouse hover or on keyboard/gamepad focus.</summary>
    public sealed class TooltipSource : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TooltipPanel panel;

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                if (panel != null) panel.Refresh();
            }
        }

        public void SetPanel(TooltipPanel tooltipPanel)
        {
            panel = tooltipPanel;
        }

        public void OnPointerEnter(PointerEventData eventData) => panel?.SetHovered(this);

        public void OnPointerExit(PointerEventData eventData) => panel?.ClearHovered(this);

        public void OnSelect(BaseEventData eventData) => panel?.SetFocused(this);

        public void OnDeselect(BaseEventData eventData) => panel?.ClearFocused(this);

        private void OnDisable()
        {
            if (panel == null) return;
            panel.ClearHovered(this);
            panel.ClearFocused(this);
        }
    }
}
