using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// Desk actions on the new Input System (ui.md proposed map). Navigation and Submit (Enter / A,
    /// mouse click) come from the EventSystem's InputSystemUIInputModule; this adds:
    /// Cue = C / gamepad X (west) / right click on a card, Tooltip = T / gamepad Y (north),
    /// End turn = E / gamepad Start. C cues the card under the mouse if there is one, else the
    /// focused card (focus is never empty, so "hovered when nothing is focused" can't occur).
    /// W/A/S/D stay on navigation (the debug keys are F5-F9, ui.md). Cue is free and doesn't end
    /// your plays (brief §2).
    /// </summary>
    public sealed class ProtoInput : MonoBehaviour
    {
        [SerializeField] private ProtoCombatController controller;
        [SerializeField] private TooltipPanel tooltipPanel;

        private InputAction _cue;
        private InputAction _tooltip;
        private InputAction _endTurn;

        private void Awake()
        {
            _cue = new InputAction("Cue", InputActionType.Button);
            _cue.AddBinding("<Keyboard>/c");
            _cue.AddBinding("<Gamepad>/buttonWest");

            _tooltip = new InputAction("Tooltip", InputActionType.Button);
            _tooltip.AddBinding("<Keyboard>/t");
            _tooltip.AddBinding("<Gamepad>/buttonNorth");

            _endTurn = new InputAction("EndTurn", InputActionType.Button);
            _endTurn.AddBinding("<Keyboard>/e");
            _endTurn.AddBinding("<Gamepad>/start");
        }

        private void OnEnable()
        {
            _cue.performed += OnCue;
            _tooltip.performed += OnTooltip;
            _endTurn.performed += OnEndTurn;
            _cue.Enable();
            _tooltip.Enable();
            _endTurn.Enable();
        }

        private void OnDisable()
        {
            _cue.performed -= OnCue;
            _tooltip.performed -= OnTooltip;
            _endTurn.performed -= OnEndTurn;
            _cue.Disable();
            _tooltip.Disable();
            _endTurn.Disable();
        }

        private void OnDestroy()
        {
            _cue.Dispose();
            _tooltip.Dispose();
            _endTurn.Dispose();
        }

        private void OnCue(InputAction.CallbackContext ctx)
        {
            CardView card = CardView.Hovered != null && CardView.Hovered.isActiveAndEnabled ? CardView.Hovered : SelectedCard();
            if (card == null || card.InCueSlot || card.Uid < 0) return;
            controller.Cue(card.Uid);
        }

        private void OnTooltip(InputAction.CallbackContext ctx)
        {
            tooltipPanel.ToggleVisible();
        }

        private void OnEndTurn(InputAction.CallbackContext ctx)
        {
            controller.EndTurn();
        }

        private static CardView SelectedCard()
        {
            EventSystem es = EventSystem.current;
            if (es == null || es.currentSelectedGameObject == null) return null;
            return es.currentSelectedGameObject.GetComponent<CardView>();
        }
    }
}
