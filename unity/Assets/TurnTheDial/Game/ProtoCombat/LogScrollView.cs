using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// Makes the combat log reachable and scrollable without a mouse (ui.md: every screen works with
    /// keyboard and gamepad, no dead ends). When focused, up/down (arrows, d-pad, stick) scroll the
    /// log; at the top or bottom edge they move focus on, and left/right always move focus on.
    /// Page Up / Page Down scroll it at any time. The mouse wheel works through the ScrollRect.
    /// </summary>
    public sealed class LogScrollView : Selectable
    {
        [SerializeField] private ScrollRect scroll;
        [Tooltip("Scroll step per move, in canvas units (presentation only).")]
        [SerializeField] private float lineStep = 48f;
        [Tooltip("Page Up / Page Down step, as a share of the visible height.")]
        [SerializeField] private float pageShare = 0.9f;

        public override void OnMove(AxisEventData eventData)
        {
            if (scroll != null && (eventData.moveDir == MoveDirection.Up || eventData.moveDir == MoveDirection.Down))
            {
                bool up = eventData.moveDir == MoveDirection.Up;
                bool atEdge = up ? scroll.verticalNormalizedPosition >= 0.999f : scroll.verticalNormalizedPosition <= 0.001f;
                if (!atEdge && Scrollable() > 0f)
                {
                    ScrollBy(up ? lineStep : -lineStep);
                    eventData.Use();
                    return;
                }
            }

            base.OnMove(eventData);
        }

        private void Update()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null || scroll == null) return;
            float page = scroll.viewport != null ? scroll.viewport.rect.height * pageShare : lineStep;
            if (kb.pageUpKey.wasPressedThisFrame) ScrollBy(page);
            if (kb.pageDownKey.wasPressedThisFrame) ScrollBy(-page);
        }

        private float Scrollable()
        {
            if (scroll.content == null || scroll.viewport == null) return 0f;
            return scroll.content.rect.height - scroll.viewport.rect.height;
        }

        /// <summary>Positive = toward older lines (up).</summary>
        private void ScrollBy(float amount)
        {
            float range = Scrollable();
            if (range <= 0f) return;
            scroll.verticalNormalizedPosition = Mathf.Clamp01(scroll.verticalNormalizedPosition + amount / range);
        }
    }
}
