// ------------------------------------------------------------------------------
// OutlineWelcomeWindow.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// Welcome window shown automatically the first time the package is imported.
    /// Can be reopened via Tools → Advanced Outline System → Welcome.
    /// </summary>
    public class OutlineWelcomeWindow : EditorWindow
    {
        private const string ShownKey = "AdvancedOutlineSystem_WelcomeShown_1_0_7";

        private static readonly string Version    = "1.0.7";
        private static readonly string RepoUrl    = "https://github.com/MostafaMahmoudAbdelrahman/AdvancedOutlineSystem";
        private static readonly string PortfolioUrl = "https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/";

        private Vector2 _scroll;

        // ── Auto-show on import ───────────────────────────────────────────────

        [InitializeOnLoadMethod]
        private static void AutoShow()
        {
            if (!SessionState.GetBool(ShownKey, false))
            {
                SessionState.SetBool(ShownKey, true);
                EditorApplication.delayCall += ShowWindow;
            }
        }

        // ── Menu item ─────────────────────────────────────────────────────────

        [MenuItem("Tools/Advanced Outline System/Welcome")]
        public static void ShowWindow()
        {
            var win = GetWindow<OutlineWelcomeWindow>(true, "Advanced Outline System", true);
            win.minSize = new Vector2(520, 600);
            win.maxSize = new Vector2(520, 700);
            win.Show();
        }

        // ── GUI ───────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawBanner();
            DrawSection("What is this?",
                "Advanced Outline System is a high-performance outline rendering package for Unity URP.\n\n" +
                "It supports three outline techniques:\n" +
                "  • 3D Outline — inverted-hull, works on any mesh\n" +
                "  • Screen-Space Outline — full-screen depth + normal edge detection\n" +
                "  • 2D Outline — alpha-based edge detection for sprites");

            DrawSection("Quick Setup",
                "1. Select your URP Renderer asset\n" +
                "   (Assets/Settings/UniversalRenderer.asset)\n\n" +
                "2. Click Add Renderer Feature → Outline Render Feature\n\n" +
                "3. Add an OutlineManager component to any scene GameObject\n\n" +
                "4. Add OutlineObject to any mesh or sprite GameObject\n\n" +
                "5. Configure Color, Thickness and Mode in the Inspector");

            DrawSection("Requirements",
                "• Unity 2021.3 LTS or newer\n" +
                "• Universal Render Pipeline 12.0.0+\n" +
                "• Compatibility Mode enabled (Unity 6+)\n" +
                "  Project Settings → Graphics → Render Graph → Compatibility Mode");

            DrawButtons();
            EditorGUILayout.EndScrollView();
        }

        private void DrawBanner()
        {
            EditorGUILayout.Space(10);
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 18,
                alignment = TextAnchor.MiddleCenter
            };
            var subStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 11
            };

            EditorGUILayout.LabelField("Advanced Outline System (URP)", titleStyle,
                GUILayout.Height(28));
            EditorGUILayout.LabelField($"Version {Version}  —  Mostafa Mahmoud Abdelrahman",
                subStyle, GUILayout.Height(18));

            EditorGUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
            EditorGUILayout.Space(8);
        }

        private static void DrawSection(string title, string body)
        {
            var headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
            var bodyStyle   = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize  = 11,
                wordWrap  = true,
                richText  = true
            };

            EditorGUILayout.LabelField(title, headerStyle);
            EditorGUILayout.Space(2);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField(body, bodyStyle);
                EditorGUILayout.Space(4);
            }
            EditorGUILayout.Space(8);
        }

        private void DrawButtons()
        {
            EditorGUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.35f, 0.35f, 0.35f));
            EditorGUILayout.Space(8);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open Documentation", GUILayout.Height(32)))
                    OutlineDocumentationWindow.ShowWindow();

                if (GUILayout.Button("Validate Setup", GUILayout.Height(32)))
                    OutlineValidator.RunValidation();
            }

            EditorGUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("GitHub Repository", GUILayout.Height(28)))
                    Application.OpenURL(RepoUrl);

                if (GUILayout.Button("Author Portfolio", GUILayout.Height(28)))
                    Application.OpenURL(PortfolioUrl);
            }

            EditorGUILayout.Space(10);
        }
    }
}
