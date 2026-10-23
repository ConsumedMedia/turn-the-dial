using TMPro;
using TurnTheDial.Data;
using TurnTheDial.Game.ProtoCombat;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TurnTheDial.Game.Editor
{
    /// <summary>
    /// Builds the Proto_Combat scene layout (plain uGUI, reference 1280x800). The scene holds only
    /// layout and references; all behavior is in TurnTheDial.Game and all rules in Core.
    /// </summary>
    public static class ProtoCombatSceneBuilder
    {
        public const string ScenePath = "Assets/TurnTheDial/Scenes/Proto_Combat.unity";
        public const string ContentPath = "Assets/TurnTheDial/Content/ProtoCombatContent.asset";
        public const string UiConfigPath = "Assets/TurnTheDial/Content/UI/UiConfig.asset";

        // Layout palette. Amber = you (steady round dot, left). Teal = rival (blinking diamond,
        // right), dimmer and deeper than amber. Red only on the on-air light (danger state).
        private static readonly Color Amber = new Color(1.00f, 0.71f, 0.16f);
        private static readonly Color Teal = new Color(0.10f, 0.47f, 0.47f);
        private static readonly Color Cream = new Color(0.96f, 0.93f, 0.85f);
        private static readonly Color PanelColor = new Color(0.14f, 0.13f, 0.15f);
        private static readonly Color BarColor = new Color(0.18f, 0.16f, 0.17f);
        private static readonly Color ChipColor = new Color(0.20f, 0.19f, 0.21f);
        private static readonly Color Background = new Color(0.09f, 0.08f, 0.10f);

        private enum Side
        {
            None,
            You,
            Rival,
        }

        private const float MarkerSize = 12f;

        private static TMP_FontAsset _font;
        private static UiConfigAsset _uiConfig;
        private static TooltipPanel _tooltipPanel;

        [MenuItem("Turn the Dial/Scenes/Build Proto_Combat")]
        public static void Build()
        {
            _font = TMP_Settings.defaultFontAsset;
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Background;
            cam.orthographic = true;
            camGo.transform.position = new Vector3(0f, 0f, -10f);

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            var module = esGo.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();

            var canvasGo = new GameObject("Canvas", typeof(RectTransform));
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 800f);
            // Expand: the whole 1280x800 layout is always on screen (exact 1:1 at 1280x800).
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasGo.AddComponent<GraphicRaycaster>();
            Transform root = canvasGo.transform;

            var uiConfig = AssetDatabase.LoadAssetAtPath<UiConfigAsset>(UiConfigPath);
            _uiConfig = uiConfig;
            var content = AssetDatabase.LoadAssetAtPath<FightContent>(ContentPath);

            // Systems
            var systems = new GameObject("ProtoCombat");
            var controller = systems.AddComponent<ProtoCombatController>();
            Wire(controller, "content", content);

            // Tooltip panel (fixed area, center top)
            var tooltipRoot = Rect("Tooltip", root, 490, 72, 380, 220);
            var tooltipBox = Panel("Panel", tooltipRoot, 0, 0, 380, 220, PanelColor);
            var tooltipText = Text("Body", tooltipBox.transform, 10, 8, 360, 204, 14, TextAlignmentOptions.TopLeft, Cream);
            _tooltipPanel = tooltipRoot.gameObject.AddComponent<TooltipPanel>();
            Wire(_tooltipPanel, "body", tooltipText);
            Wire(_tooltipPanel, "panel", tooltipBox.gameObject);

            // Top bar: you left, rival right
            Panel("Top Bar", root, 0, 0, 1280, 64, BarColor);
            var dot = Panel("You Marker (amber dot, steady)", root, 16, 18, 28, 28, Amber);
            dot.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            var youName = Text("You Name", root, 52, 6, 360, 22, 16, TextAlignmentOptions.Left, Amber);
            Readout listeners = MakeReadout("Listeners", root, 52, 30, 260, 30, 20, Amber, Side.You);
            Readout turn = MakeReadout("Turn", root, 540, 14, 200, 36, 20, Cream, Side.None, TextAlignmentOptions.Center);
            var rivalName = Text("Rival Name", root, 868, 6, 360, 22, 16, TextAlignmentOptions.Right, TealText());
            Readout audience = MakeReadout("Audience", root, 968, 30, 260, 30, 20, TealText(), Side.Rival, TextAlignmentOptions.Right);
            var diamond = Panel("Rival Marker (teal diamond, blinking)", root, 1238, 20, 22, 22, Teal);
            diamond.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            diamond.rectTransform.anchoredPosition = new Vector2(1249f, -31f);
            diamond.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var blink = diamond.gameObject.AddComponent<BlinkMarker>();
            Wire(blink, "uiConfig", uiConfig);
            Wire(blink, "graphic", diamond);

            // Rival area (right)
            Readout intent = MakeReadout("Intent", root, 880, 72, 390, 56, 18, TealText(), Side.Rival, TextAlignmentOptions.Right);
            Readout rivalLock = MakeReadout("Rival Lock", root, 880, 134, 125, 32, 15, TealText(), Side.Rival, TextAlignmentOptions.Center);
            Readout buzz = MakeReadout("Rival Buzz", root, 1012, 134, 125, 32, 15, TealText(), Side.Rival, TextAlignmentOptions.Center);
            Readout exposed = MakeReadout("Rival Exposed", root, 1144, 134, 126, 32, 15, TealText(), Side.Rival, TextAlignmentOptions.Center);

            // Log (right)
            ScrollRect logScroll = MakeLog(root, out TextMeshProUGUI logText);

            // Your statuses (left)
            string[] names = { "Airtime", "Flow", "Hype", "Loyalty", "Cash", "Dead Air", "Static", "Draw Pile", "Discard", "Burnout" };
            var readouts = new Readout[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                readouts[i] = MakeReadout(names[i], root, 10, 72 + i * 38, 280, 34, 16, Cream, Side.You);
            }

            // On-air light (red: danger signal only when it flickers)
            var onAirRoot = Rect("On Air Light", root, 10, 460, 280, 60);
            var lamp = Panel("Lamp", onAirRoot, 0, 16, 26, 26, new Color(0.86f, 0.12f, 0.10f));
            lamp.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            var lampText = Text("Label", onAirRoot, 34, 0, 246, 60, 13, TextAlignmentOptions.Left, Cream);
            var onAir = onAirRoot.gameObject.AddComponent<OnAirLight>();
            Wire(onAir, "lamp", lamp);
            Wire(onAir, "label", lampText);
            Wire(onAir, "uiConfig", uiConfig);

            // Center: cue slot, result, message, hint
            CardView cue = MakeCard("Cue Slot", root, 300, 72, 180, 150, controller);
            AddMarker(cue.transform, Side.You, 180f - MarkerSize - 6f, 6f);
            var resultPanel = Panel("Result", root, 300, 300, 570, 90, PanelColor);
            var resultText = Text("Text", resultPanel.transform, 12, 8, 546, 74, 18, TextAlignmentOptions.Center, Cream);
            var message = Text("Message", root, 300, 395, 570, 40, 15, TextAlignmentOptions.Left, Cream);
            Text("Controls", root, 300, 436, 570, 90, 13, TextAlignmentOptions.Left, new Color(0.75f, 0.72f, 0.66f)).text =
                "Play: click a card, or Enter / gamepad A on the focused card.  Cue: right-click a card, or C / gamepad X "
                + "(C cues the card under the mouse, else the focused one). Cueing is free; the cued card plays next turn.\n"
                + "Move focus: arrows / WASD / d-pad / stick.  Tooltips: hover or focus; T / gamepad Y hides or shows them.  "
                + "End turn: E / gamepad Start.  Log: focus it and press up / down, or Page Up / Page Down.";

            // Hand (bottom): 2 rows of 5
            var handRect = Rect("Hand", root, 10, 530, 860, 260);
            var grid = handRect.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(166f, 124f);
            grid.spacing = new Vector2(6f, 6f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            CardView template = MakeCard("Card Template", handRect, 0, 0, 166, 124, controller);

            // End turn and seed
            Button endTurn = MakeButton("End Turn", root, 1090, 710, 180, 70, "End Turn");
            var seedLabel = Text("Seed", root, 880, 706, 200, 80, 12, TextAlignmentOptions.TopLeft, new Color(0.75f, 0.72f, 0.66f));

            // Focus frame (always visible focus)
            var frameRect = Rect("Focus Frame", root, 0, 0, 10, 10);
            MakeFrame(frameRect);
            var focus = frameRect.gameObject.AddComponent<FocusFrame>();
            Wire(focus, "frame", frameRect);
            Wire(focus, "preferred", handRect);
            var fallbackProp = new SerializedObject(focus);
            SerializedProperty arr = fallbackProp.FindProperty("fallbacks");
            arr.arraySize = 2;
            arr.GetArrayElementAtIndex(0).objectReferenceValue = endTurn.gameObject;
            arr.GetArrayElementAtIndex(1).objectReferenceValue = listeners.gameObject;
            fallbackProp.ApplyModifiedPropertiesWithoutUndo();

            // View
            var view = systems.AddComponent<ProtoCombatView>();
            Wire(view, "controller", controller);
            Wire(view, "uiConfig", uiConfig);
            Wire(view, "tooltipPanel", _tooltipPanel);
            Wire(view, "youName", youName);
            Wire(view, "listeners", listeners);
            Wire(view, "airtime", readouts[0]);
            Wire(view, "flow", readouts[1]);
            Wire(view, "hype", readouts[2]);
            Wire(view, "loyalty", readouts[3]);
            Wire(view, "cash", readouts[4]);
            Wire(view, "deadAir", readouts[5]);
            Wire(view, "staticCount", readouts[6]);
            Wire(view, "drawPile", readouts[7]);
            Wire(view, "discardPile", readouts[8]);
            Wire(view, "burnout", readouts[9]);
            Wire(view, "onAirLight", onAir);
            Wire(view, "rivalName", rivalName);
            Wire(view, "audience", audience);
            Wire(view, "intent", intent);
            Wire(view, "rivalLock", rivalLock);
            Wire(view, "buzz", buzz);
            Wire(view, "exposed", exposed);
            Wire(view, "turn", turn);
            Wire(view, "cueSlot", cue);
            Wire(view, "hand", handRect);
            Wire(view, "cardTemplate", template);
            Wire(view, "endTurnButton", endTurn);
            Wire(view, "message", message);
            Wire(view, "result", resultText);
            Wire(view, "resultPanel", resultPanel.gameObject);
            Wire(view, "seedLabel", seedLabel);
            Wire(view, "log", logText);
            Wire(view, "logScroll", logScroll);

            var input = systems.AddComponent<ProtoInput>();
            Wire(input, "controller", controller);
            Wire(input, "tooltipPanel", _tooltipPanel);

            tooltipRoot.SetAsLastSibling();
            frameRect.SetAsLastSibling();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
        }

        private static Color TealText()
        {
            // Rival text uses a lighter tint of the same teal so 12+ px text stays readable.
            return new Color(0.38f, 0.75f, 0.74f);
        }

        private static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(x, -y);
            rt.sizeDelta = new Vector2(w, h);
            return rt;
        }

        private static Image Panel(string name, Transform parent, float x, float y, float w, float h, Color color)
        {
            RectTransform rt = Rect(name, parent, x, y, w, h);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        private static TextMeshProUGUI Text(string name, Transform parent, float x, float y, float w, float h, float size, TextAlignmentOptions align, Color color)
        {
            RectTransform rt = Rect(name, parent, x, y, w, h);
            var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
            t.font = _font;
            t.fontSize = size;
            t.alignment = align;
            t.color = color;
            t.textWrappingMode = TextWrappingModes.Normal;
            t.raycastTarget = false;
            t.text = "";
            return t;
        }

        private static Readout MakeReadout(string name, Transform parent, float x, float y, float w, float h, float size, Color color, Side side, TextAlignmentOptions align = TextAlignmentOptions.Left)
        {
            Image bg = Panel(name, parent, x, y, w, h, ChipColor);
            var selectable = bg.gameObject.AddComponent<Selectable>();
            selectable.targetGraphic = bg;
            selectable.colors = Tint();
            var tip = bg.gameObject.AddComponent<TooltipSource>();
            tip.SetPanel(_tooltipPanel);
            Wire(tip, "panel", _tooltipPanel);
            // Color rule (brief §11): your readouts carry a steady amber dot on their left edge, the
            // rival's a blinking teal diamond on their right edge.
            float textX = side == Side.You ? 8f + MarkerSize + 6f : 8f;
            float textW = w - 16f - (side == Side.None ? 0f : MarkerSize + 6f);
            TextMeshProUGUI label = Text("Label", bg.transform, textX, 0, textW, h, size, align, color);
            label.verticalAlignment = VerticalAlignmentOptions.Middle;
            if (side == Side.You) AddMarker(bg.transform, side, 8f, (h - MarkerSize) * 0.5f);
            if (side == Side.Rival) AddMarker(bg.transform, side, w - 8f - MarkerSize, (h - MarkerSize) * 0.5f);
            var readout = bg.gameObject.AddComponent<Readout>();
            Wire(readout, "label", label);
            Wire(readout, "tooltip", tip);
            return readout;
        }

        /// <summary>Amber steady round dot (you) or teal blinking diamond (rival).</summary>
        private static void AddMarker(Transform parent, Side side, float x, float y)
        {
            if (side == Side.None) return;
            Image marker = Panel(side == Side.You ? "You Marker (amber dot)" : "Rival Marker (teal diamond)", parent, x, y, MarkerSize, MarkerSize, side == Side.You ? Amber : Teal);
            marker.raycastTarget = false;
            if (side == Side.You)
            {
                marker.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                return;
            }

            // Rotate about the center so the diamond stays inside its square.
            RectTransform rt = marker.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x + MarkerSize * 0.5f, -y - MarkerSize * 0.5f);
            rt.sizeDelta = new Vector2(MarkerSize * 0.75f, MarkerSize * 0.75f);
            rt.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var blink = marker.gameObject.AddComponent<BlinkMarker>();
            Wire(blink, "uiConfig", _uiConfig);
            Wire(blink, "graphic", marker);
        }

        private static CardView MakeCard(string name, Transform parent, float x, float y, float w, float h, ProtoCombatController controller)
        {
            Image bg = Panel(name, parent, x, y, w, h, ChipColor);
            var button = bg.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;
            button.colors = Tint();
            var tip = bg.gameObject.AddComponent<TooltipSource>();
            Wire(tip, "panel", _tooltipPanel);
            TextMeshProUGUI title = Text("Title", bg.transform, 6, 4, w - 12, 20, 15, TextAlignmentOptions.TopLeft, Cream);
            title.fontStyle = FontStyles.Bold;
            TextMeshProUGUI meta = Text("Meta", bg.transform, 6, 25, w - 12, 18, 12, TextAlignmentOptions.TopLeft, Cream);
            TextMeshProUGUI rules = Text("Rules", bg.transform, 6, 45, w - 12, h - 49, 13, TextAlignmentOptions.TopLeft, Cream);
            var card = bg.gameObject.AddComponent<CardView>();
            Wire(card, "title", title);
            Wire(card, "meta", meta);
            Wire(card, "rules", rules);
            Wire(card, "background", bg);
            Wire(card, "button", button);
            Wire(card, "tooltip", tip);
            return card;
        }

        private static Button MakeButton(string name, Transform parent, float x, float y, float w, float h, string label)
        {
            Image bg = Panel(name, parent, x, y, w, h, new Color(0.30f, 0.27f, 0.24f));
            var button = bg.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;
            button.colors = Tint();
            Text("Label", bg.transform, 0, 0, w, h, 20, TextAlignmentOptions.Center, Cream).text = label;
            return button;
        }

        private static ScrollRect MakeLog(Transform root, out TextMeshProUGUI text)
        {
            Image bg = Panel("Combat Log", root, 880, 176, 390, 520, PanelColor);
            var scroll = bg.gameObject.AddComponent<ScrollRect>();
            var logFocus = bg.gameObject.AddComponent<LogScrollView>();
            logFocus.targetGraphic = bg;
            logFocus.colors = Tint();
            Wire(logFocus, "scroll", scroll);
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;
            RectTransform viewport = Rect("Viewport", bg.transform, 6, 6, 378, 508);
            viewport.gameObject.AddComponent<RectMask2D>();
            RectTransform contentRt = Rect("Content", viewport, 0, 0, 378, 508);
            contentRt.anchorMin = new Vector2(0f, 1f);
            contentRt.anchorMax = new Vector2(1f, 1f);
            contentRt.sizeDelta = new Vector2(0f, 508f);
            text = contentRt.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = _font;
            text.fontSize = 12;
            text.color = Cream;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.raycastTarget = false;
            var fitter = contentRt.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport;
            scroll.content = contentRt;
            return scroll;
        }

        private static void MakeFrame(RectTransform frame)
        {
            const float t = 3f;
            Color c = Cream;
            Edge(frame, "Top", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, t), c);
            Edge(frame, "Bottom", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, t), c);
            Edge(frame, "Left", new Vector2(0, 0), new Vector2(0, 1), new Vector2(t, 0), c);
            Edge(frame, "Right", new Vector2(1, 0), new Vector2(1, 1), new Vector2(t, 0), c);
        }

        private static void Edge(RectTransform parent, string name, Vector2 min, Vector2 max, Vector2 size, Color c)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = c;
            img.raycastTarget = false;
        }

        private static ColorBlock Tint()
        {
            ColorBlock cb = ColorBlock.defaultColorBlock;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.25f, 1.25f, 1.25f);
            cb.selectedColor = new Color(1.35f, 1.35f, 1.35f);
            cb.pressedColor = new Color(0.8f, 0.8f, 0.8f);
            cb.disabledColor = new Color(0.6f, 0.6f, 0.6f);
            cb.colorMultiplier = 1.5f;
            return cb;
        }

        private static void Wire(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            SerializedProperty prop = so.FindProperty(field);
            if (prop == null)
            {
                Debug.LogError("ProtoCombatSceneBuilder: no field '" + field + "' on " + target.GetType().Name);
                return;
            }

            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
