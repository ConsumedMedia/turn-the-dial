using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// Desk actions on the new Input System (ui.md proposed map). Navigation and Submit (Enter / A,
    /// mouse click) come from the EventSystem's InputSystemUIInputModule; this adds:
    /// Cue = C / gamepad X (west) / right click on a card, Tooltip = T / gamepad Y (north),
    /// End turn = E / gamepad Start. C cues the card under the mouse if there is one, else the
    /// focused card (focus is never empty, so "hovered when nothing is focused" can't occur).
    /// The default UI Navigate action also binds W/A/S/D, which collides with the prototype's
    /// debug keys (W win, S seed, D draw), so those four bindings are disabled; arrows, d-pad and
    /// stick still navigate.
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

        private static readonly string[] DebugKeyNavigationBindings = { "<Keyboard>/w", "<Keyboard>/a", "<Keyboard>/s", "<Keyboard>/d" };

        private InputAction _overriddenMove;

        private void Start()
        {
            var module = FindAnyObjectByType<InputSystemUIInputModule>();
            if (module == null || module.move == null || module.move.action == null) return;
            InputAction move = module.move.action;
            _overriddenMove = move;
            for (int i = 0; i < DebugKeyNavigationBindings.Length; i++)
            {
                // An empty override path disables the binding.
                move.ApplyBindingOverride(new InputBinding { path = DebugKeyNavigationBindings[i], overridePath = "" });
            }
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
            if (_overriddenMove != null)
            {
                for (int i = 0; i < DebugKeyNavigationBindings.Length; i++)
                {
                    _overriddenMove.RemoveBindingOverride(new InputBinding { path = DebugKeyNavigationBindings[i] });
                }

                _overriddenMove = null;
            }

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
