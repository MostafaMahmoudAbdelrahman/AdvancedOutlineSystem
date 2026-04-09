// ------------------------------------------------------------------------------
// OutlineDocumentationWindow.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// In-editor documentation browser for the Advanced Outline System.
    /// Open via Tools → Advanced Outline System → Documentation.
    /// </summary>
    public class OutlineDocumentationWindow : EditorWindow
    {
        private enum Tab { HowToUse, SetupGuide, API, Troubleshooting }

        private Tab     _tab;
        private Vector2 _scroll;

        [MenuItem("Tools/Advanced Outline System/Documentation")]
        public static void ShowWindow()
        {
            var win = GetWindow<OutlineDocumentationWindow>(false, "Outline Docs", true);
            win.minSize = new Vector2(560, 500);
            win.Show();
        }

        private void OnGUI()
        {
            DrawToolbar();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.Space(6);

            switch (_tab)
            {
                case Tab.HowToUse:      DrawHowToUse();      break;
                case Tab.SetupGuide:    DrawSetupGuide();    break;
                case Tab.API:           DrawAPI();           break;
                case Tab.Troubleshooting: DrawTroubleshooting(); break;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.EndScrollView();
        }

        // ── Toolbar ───────────────────────────────────────────────────────────

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Toggle(_tab == Tab.HowToUse,    "How To Use",     EditorStyles.toolbarButton)) _tab = Tab.HowToUse;
                if (GUILayout.Toggle(_tab == Tab.SetupGuide,  "Setup Guide",    EditorStyles.toolbarButton)) _tab = Tab.SetupGuide;
                if (GUILayout.Toggle(_tab == Tab.API,         "API Reference",  EditorStyles.toolbarButton)) _tab = Tab.API;
                if (GUILayout.Toggle(_tab == Tab.Troubleshooting, "Troubleshooting", EditorStyles.toolbarButton)) _tab = Tab.Troubleshooting;
            }
        }

        // ── How To Use ────────────────────────────────────────────────────────

        private void DrawHowToUse()
        {
            H1("How To Use");

            H2("Step 1 — Add the Renderer Feature");
            Body(
                "1. Find your URP Renderer asset in the Project window.\n" +
                "   Default location: Assets/Settings/UniversalRenderer.asset\n\n" +
                "2. Select it and click Add Renderer Feature in the Inspector.\n\n" +
                "3. Choose Outline Render Feature from the list.\n\n" +
                "4. The feature is now active with default settings.");

            H2("Step 2 — Add OutlineManager to your scene");
            Body(
                "Create an empty GameObject and add the OutlineManager component.\n\n" +
                "• It is a persistent singleton (DontDestroyOnLoad).\n" +
                "• Only one instance is needed per project.\n" +
                "• The demo DemoSceneSetup creates it automatically if missing.");

            H2("Step 3 — Add outlines to objects");
            Body(
                "Select any GameObject that has a MeshRenderer, SkinnedMeshRenderer,\n" +
                "or SpriteRenderer and add the OutlineObject component.\n\n" +
                "Configure in the Inspector:\n" +
                "  • Enabled       — toggle outline on/off\n" +
                "  • Outline Mode  — Outline3D, ScreenSpace, or Outline2D\n" +
                "  • Color         — RGBA outline color\n" +
                "  • Thickness     — 0 to 20");

            H2("Step 4 — Runtime control");
            Code(
                "using AdvancedOutlineSystem;\n\n" +
                "// Get the component\n" +
                "var outline = GetComponent<OutlineObject>();\n\n" +
                "// Enable with custom color\n" +
                "outline.OutlineColor     = Color.yellow;\n" +
                "outline.OutlineThickness = 5f;\n" +
                "outline.OutlineEnabled   = true;\n\n" +
                "// Disable\n" +
                "outline.OutlineEnabled = false;");

            H2("Outline Modes");
            Body(
                "Outline3D\n" +
                "  Inverted-hull technique. Expands vertices along normals and renders\n" +
                "  back-faces only. Best for opaque 3D meshes.\n\n" +
                "ScreenSpace\n" +
                "  Full-screen depth + normal edge detection (Roberts cross).\n" +
                "  Applies globally to the camera. Best for a stylised scene-wide look.\n\n" +
                "Outline2D\n" +
                "  Alpha-based neighbour sampling. Best for sprites and UI elements.");

            H2("Unity 6 — Compatibility Mode");
            Body(
                "This package uses the Execute() path of ScriptableRenderPass.\n" +
                "In Unity 6 you must enable Compatibility Mode:\n\n" +
                "  Project Settings → Graphics → Render Graph\n" +
                "  → check Compatibility Mode (Render Graph Disabled)");

            EditorGUILayout.Space(6);
            if (GUILayout.Button("Validate Setup Now", GUILayout.Height(30)))
                OutlineValidator.RunValidation();
        }

        // ── Setup Guide ───────────────────────────────────────────────────────

        private void DrawSetupGuide()
        {
            H1("Setup Guide");

            H2("URP Asset — Required Settings");
            Body(
                "Select your URP Asset (UniversalRenderPipelineAsset.asset):\n\n" +
                "  • Depth Texture      → Enable  (required for ScreenSpace mode)\n" +
                "  • Opaque Texture     → Enable  (recommended)\n" +
                "  • Additional Lights Shadow Atlas → 4096 or higher\n" +
                "    (avoids the 'shadow atlas too small' warning)");

            H2("URP Renderer — Add Feature");
            Body(
                "Select your URP Renderer asset (UniversalRenderer.asset):\n\n" +
                "  1. Scroll to the bottom of the Inspector\n" +
                "  2. Click Add Renderer Feature\n" +
                "  3. Select Outline Render Feature\n\n" +
                "Feature settings:\n" +
                "  • Render Pass Event  — default: Before Rendering Post Processing\n" +
                "  • Default Thickness  — 3\n" +
                "  • Default Color      — White");

            H2("Scene Setup");
            Body(
                "Every scene that uses outlines needs one OutlineManager.\n\n" +
                "  1. Create an empty GameObject, name it OutlineManager\n" +
                "  2. Add Component → OutlineManager\n\n" +
                "Optionally add OutlineShadowAtlasFix to the same GameObject\n" +
                "to get a startup warning if the shadow atlas is too small.");

            H2("Shadow Atlas Warning");
            Body(
                "If you see:\n" +
                "  'Reduced additional punctual light shadows resolution'\n\n" +
                "Fix it in your URP Asset:\n" +
                "  Shadows → Additional Lights → Shadow Atlas Resolution → 4096");
        }

        // ── API ───────────────────────────────────────────────────────────────

        private void DrawAPI()
        {
            H1("API Reference");

            H2("OutlineObject");
            Body("MonoBehaviour — attach to any GameObject to enable outline rendering.");
            Code(
                "// Properties\n" +
                "Color  OutlineColor      // RGBA outline color\n" +
                "float  OutlineThickness  // 0–20\n" +
                "bool   OutlineEnabled    // toggle without removing component\n" +
                "OutlineMode Mode         // Outline3D | ScreenSpace | Outline2D\n\n" +
                "// Event\n" +
                "event Action OnChanged   // fires when any property changes");

            H2("OutlineManager");
            Body("Singleton MonoBehaviour — tracks all active OutlineObjects.");
            Code(
                "// Access\n" +
                "OutlineManager.Instance\n\n" +
                "// Read-only lists\n" +
                "IReadOnlyList<OutlineObject> Objects3D\n" +
                "IReadOnlyList<OutlineObject> ObjectsScreen\n" +
                "IReadOnlyList<OutlineObject> Objects2D\n\n" +
                "// Methods\n" +
                "void Register(OutlineObject obj)\n" +
                "void Unregister(OutlineObject obj)\n" +
                "void RefreshObject(OutlineObject obj)");

            H2("OutlineMode enum");
            Code(
                "OutlineMode.Outline3D     // inverted-hull\n" +
                "OutlineMode.ScreenSpace   // depth + normal edge detection\n" +
                "OutlineMode.Outline2D     // sprite alpha edge detection");

            H2("Hover Highlight Example");
            Code(
                "using AdvancedOutlineSystem;\n" +
                "using UnityEngine;\n\n" +
                "public class HoverHighlight : MonoBehaviour\n" +
                "{\n" +
                "    private OutlineObject _outline;\n" +
                "    void Awake() => _outline = GetComponent<OutlineObject>();\n\n" +
                "    void OnMouseEnter()\n" +
                "    {\n" +
                "        _outline.OutlineColor     = Color.yellow;\n" +
                "        _outline.OutlineThickness = 5f;\n" +
                "        _outline.OutlineEnabled   = true;\n" +
                "    }\n\n" +
                "    void OnMouseExit() => _outline.OutlineEnabled = false;\n" +
                "}");

            H2("Selection Manager Example");
            Code(
                "public class SelectionManager : MonoBehaviour\n" +
                "{\n" +
                "    private OutlineObject _current;\n\n" +
                "    public void Select(OutlineObject target)\n" +
                "    {\n" +
                "        if (_current != null) _current.OutlineEnabled = false;\n" +
                "        _current = target;\n" +
                "        _current.OutlineColor     = Color.white;\n" +
                "        _current.OutlineThickness = 5f;\n" +
                "        _current.OutlineEnabled   = true;\n" +
                "    }\n\n" +
                "    public void Deselect()\n" +
                "    {\n" +
                "        if (_current == null) return;\n" +
                "        _current.OutlineEnabled = false;\n" +
                "        _current = null;\n" +
                "    }\n" +
                "}");
        }

        // ── Troubleshooting ───────────────────────────────────────────────────

        private void DrawTroubleshooting()
        {
            H1("Troubleshooting");

            Problem("No outline visible",
                "• Outline Render Feature not added to URP Renderer\n" +
                "• OutlineManager missing from scene\n" +
                "• OutlineObject.OutlineEnabled is false\n" +
                "• Thickness is 0");

            Problem("Screen-space outline all black",
                "• Depth Texture not enabled on URP Asset\n" +
                "• Normals prepass not running (requires URP 12+)");

            Problem("2D outline not showing",
                "• OutlineObject.Mode is not set to Outline2D\n" +
                "• SpriteRenderer has no sprite assigned");

            Problem("Outline flickers",
                "• Multiple OutlineManager instances in scene\n" +
                "  — ensure only one OutlineManager exists");

            Problem("Shader not found error",
                "• Re-import the package\n" +
                "• Run Tools → Advanced Outline System → Validate Setup");

            Problem("Shadow atlas warning",
                "• URP Asset → Shadows → Additional Lights Shadow Atlas → set to 4096\n" +
                "• Or add OutlineShadowAtlasFix component to OutlineManager GameObject");

            Problem("Unity 6 — outlines not rendering",
                "• Enable Compatibility Mode:\n" +
                "  Project Settings → Graphics → Render Graph\n" +
                "  → Compatibility Mode (Render Graph Disabled)");

            Problem("Package stuck on old version",
                "• Remove the package in Package Manager\n" +
                "• Re-add with pinned tag:\n" +
                "  https://github.com/MostafaMahmoudAbdelrahman/AdvancedOutlineSystem.git#v1.0.7");

            EditorGUILayout.Space(6);
            if (GUILayout.Button("Run Validate Setup", GUILayout.Height(30)))
                OutlineValidator.RunValidation();

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Open GitHub Issues", GUILayout.Height(28)))
                Application.OpenURL("https://github.com/MostafaMahmoudAbdelrahman/AdvancedOutlineSystem/issues");
        }

        // ── Style helpers ─────────────────────────────────────────────────────

        private static void H1(string text)
        {
            var s = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 };
            EditorGUILayout.LabelField(text, s);
            var r = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(r, new Color(0.4f, 0.4f, 0.4f));
            EditorGUILayout.Space(6);
        }

        private static void H2(string text)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        }

        private static void Body(string text)
        {
            var s = new GUIStyle(EditorStyles.wordWrappedLabel) { wordWrap = true, fontSize = 11 };
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                EditorGUILayout.LabelField(text, s);
        }

        private static void Code(string text)
        {
            var s = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                wordWrap  = true,
                fontSize  = 10,
                fontStyle = FontStyle.Normal,
                font      = (Font)EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf")
                            ?? EditorStyles.label.font
            };
            using (new EditorGUILayout.VerticalScope("box"))
                EditorGUILayout.LabelField(text, s);
        }

        private static void Problem(string title, string body)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            var s = new GUIStyle(EditorStyles.wordWrappedLabel) { wordWrap = true, fontSize = 11 };
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                EditorGUILayout.LabelField(body, s);
        }
    }
}
