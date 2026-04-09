// ------------------------------------------------------------------------------
// OutlineObjectEditor.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using AdvancedOutlineSystem;

namespace AdvancedOutlineSystem.Editor
{
    [CustomEditor(typeof(OutlineObject))]
    public class OutlineObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _thickness;
        private SerializedProperty _enabled;
        private SerializedProperty _mode;

        private void OnEnable()
        {
            _color     = serializedObject.FindProperty("_outlineColor");
            _thickness = serializedObject.FindProperty("_outlineThickness");
            _enabled   = serializedObject.FindProperty("_outlineEnabled");
            _mode      = serializedObject.FindProperty("_outlineMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(4);
            DrawHeader();
            EditorGUILayout.Space(6);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_enabled,   new GUIContent("Enabled"));
                EditorGUILayout.PropertyField(_mode,      new GUIContent("Outline Mode"));
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(_color,     new GUIContent("Color"));
                EditorGUILayout.Slider(_thickness, 0f, 20f, new GUIContent("Thickness"));
            }

            EditorGUILayout.Space(4);
            DrawHelpBox();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 13,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("Advanced Outline System", style);
            var rect = GUILayoutUtility.GetLastRect();
            rect.y      += EditorGUIUtility.singleLineHeight + 2;
            rect.height  = 1;
            EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f));
            GUILayout.Space(4);
        }

        private void DrawHelpBox()
        {
            var obj = (OutlineObject)target;
            switch (obj.Mode)
            {
                case OutlineMode.Outline3D:
                    EditorGUILayout.HelpBox(
                        "3D Outline: Inverted-hull technique. Works on MeshRenderer and SkinnedMeshRenderer.",
                        MessageType.Info);
                    break;
                case OutlineMode.ScreenSpace:
                    EditorGUILayout.HelpBox(
                        "Screen-Space Outline: Depth + normal edge detection. Applies globally to the camera.",
                        MessageType.Info);
                    break;
                case OutlineMode.Outline2D:
                    EditorGUILayout.HelpBox(
                        "2D Outline: Alpha-based edge detection for SpriteRenderer.",
                        MessageType.Info);
                    break;
            }
        }
    }
}
