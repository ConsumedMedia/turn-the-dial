using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnTheDial.Game.ProtoCombat
{
    /// <summary>
    /// Keeps focus always visible (ui.md): a frame follows the selected UI object, and if nothing
    /// is selected (e.g. a focused card was played) focus falls back to the first available target.
    /// </summary>
    public sealed class FocusFrame : MonoBehaviour
    {
        [SerializeField] private RectTransform frame;
        [SerializeField] private float padding = 6f;
        [Tooltip("First choice when nothing is focused: the first interactable child (the hand).")]
        [SerializeField] private Transform preferred;
        [SerializeField] private GameObject[] fallbacks;

        private void LateUpdate()
        {
            EventSystem es = EventSystem.current;
            if (es == null) return;
            GameObject selected = es.currentSelectedGameObject;
            if (selected == null || !selected.activeInHierarchy)
            {
                selected = FirstFallback();
                if (selected != null) es.SetSelectedGameObject(selected);
            }

            if (selected == null)
            {
                frame.gameObject.SetActive(false);
                return;
            }

            var target = selected.transform as RectTransform;
            if (target == null) return;
            frame.gameObject.SetActive(true);
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            var parent = frame.parent as RectTransform;
            Vector2 min = parent.InverseTransformPoint(corners[0]);
            Vector2 max = parent.InverseTransformPoint(corners[2]);
            frame.anchorMin = frame.anchorMax = new Vector2(0.5f, 0.5f);
            frame.pivot = new Vector2(0.5f, 0.5f);
            frame.localPosition = (min + max) * 0.5f;
            frame.sizeDelta = (max - min) + new Vector2(padding * 2f, padding * 2f);
            frame.SetAsLastSibling();
        }

        private GameObject FirstFallback()
        {
            if (preferred != null)
            {
                for (int i = 0; i < preferred.childCount; i++)
                {
                    var sel = preferred.GetChild(i).GetComponent<UnityEngine.UI.Selectable>();
                    if (sel != null && sel.gameObject.activeInHierarchy && sel.IsInteractable()) return sel.gameObject;
                }
            }

            if (fallbacks == null) return null;
            for (int i = 0; i < fallbacks.Length; i++)
            {
                if (fallbacks[i] != null && fallbacks[i].activeInHierarchy) return fallbacks[i];
            }

            return null;
        }
    }
}
