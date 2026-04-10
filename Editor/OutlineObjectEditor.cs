// ------------------------------------------------------------------------------
// OutlineObjectEditor.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace AdvancedOutlineSystem.Editor
{
    [CustomEditor(typeof(OutlineObject))]
    public class OutlineObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _color;
        private SerializedProperty _thickness;
        private SerializedProperty _enabled;
        private SerializedProperty _autoDetectMode;
        private SerializedProperty _mode;

        private void OnEnable()
        {
            _color = serializedObject.FindProperty("_outlineColor");
            _thickness = serializedObject.FindProperty("_outlineThickness");
            _enabled = serializedObject.FindProperty("_outlineEnabled");
            _autoDetectMode = serializedObject.FindProperty("_autoDetectMode");
            _mode = serializedObject.FindProperty("_outlineMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(4);
            DrawHeader();
            EditorGUILayout.Space(6);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_enabled, new GUIContent("Enabled"));
                EditorGUILayout.Space(2);
                EditorGUILayout.PropertyField(_autoDetectMode, new GUIContent("Auto Detect Mode", 
                    "When enabled, the outline mode is automatically detected based on the renderer type:\n" +
                    "• SpriteRenderer → 2D Outline\n" +
                    "• MeshRenderer/SkinnedMeshRenderer → 3D Outline"));

                if (_autoDetectMode.boolValue)
                {
                    // Show detected mode as read-only
                    var outlineObject = (OutlineObject)target;
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(_mode, new GUIContent("Detected Mode"));
                    GUI.enabled = true;

                    // Show detected renderer type
                    EditorGUILayout.LabelField("Renderer Type", GetRendererTypeDescription(outlineObject));
                }
                else
                {
                    EditorGUILayout.PropertyField(_mode, new GUIContent("Outline Mode"));
                }

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(_color, new GUIContent("Color"));
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
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("Advanced Outline System", style);
            var rect = GUILayoutUtility.GetLastRect();
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f));
            GUILayout.Space(4);
        }

        private string GetRendererTypeDescription(OutlineObject outlineObject)
        {
            if (outlineObject.IsSpriteRenderer) return "2D Sprite (SpriteRenderer)";
            if (outlineObject.IsMeshRenderer) return "3D Mesh (MeshRenderer/SkinnedMeshRenderer)";
            return "Unknown Renderer";
        }

        private void DrawHelpBox()
        {
            var obj = (OutlineObject)target;
            switch (obj.Mode)
            {
                case OutlineMode.Outline3D:
                    EditorGUILayout.HelpBox(
                        "3D Outline: Inverted-hull technique. Works on MeshRenderer and SkinnedMeshRenderer. " +
                        "Expands mesh vertices along normals to create the outline effect.",
                        MessageType.Info);
                    break;
                case OutlineMode.ScreenSpace:
                    EditorGUILayout.HelpBox(
                        "Screen-Space Outline: Depth + normal edge detection. Applies globally to the camera. " +
                        "Note: All objects with this mode share the same color and thickness.",
                        MessageType.Info);
                    break;
                case OutlineMode.Outline2D:
                    EditorGUILayout.HelpBox(
                        "2D Outline: Alpha-based edge detection for SpriteRenderer. " +
                        "Detects edges by comparing pixel alpha values with neighbors.",
                        MessageType.Info);
                    break;
            }

            // Multi-material warning
            var renderer = obj.CachedRenderer;
            if (renderer is MeshRenderer mr && mr.sharedMaterials != null && mr.sharedMaterials.Length > 1)
            {
                EditorGUILayout.HelpBox(
                    "This MeshRenderer has multiple materials. The outline will be applied to all sub-meshes.",
                    MessageType.Warning);
            }

            // Editor preview notice
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Outline preview is visible in Scene View. Changes to color and thickness update in real-time.",
                    MessageType.Info);
            }
        }
    }
}