// ------------------------------------------------------------------------------
// OutlineSceneViewRenderer.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// Renders 3D outlines in the Scene View for real-time editing preview.
    /// This allows users to see outline changes immediately in the editor.
    /// </summary>
    [InitializeOnLoad]
    public static class OutlineSceneViewRenderer
    {
        private static Material _outlineMaterial;
        private static MaterialPropertyBlock _mpb;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");

        static OutlineSceneViewRenderer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.update += CleanupMaterial;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!EditorApplication.isPlaying)
            {
                DrawOutlinesInSceneView();
            }
        }

        private static void DrawOutlinesInSceneView()
        {
            var manager = OutlineManager.Instance;
            if (manager == null || manager.Objects3D.Count == 0) return;

            var mat = GetMaterial();
            if (mat == null) return;

            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            foreach (var obj in manager.Objects3D)
            {
                if (obj == null || !obj.OutlineEnabled) continue;

                // MeshRenderer - handles multiple materials
                var mr = obj.GetComponent<MeshRenderer>();
                var mf = obj.GetComponent<MeshFilter>();
                if (mr != null && mf != null && mf.sharedMesh != null)
                {
                    mr.GetPropertyBlock(_mpb);
                    _mpb.SetColor(ColorProp, obj.OutlineColor);
                    _mpb.SetFloat(ThicknessProp, obj.OutlineThickness);
                    
                    // Draw all sub-meshes (handles multiple materials)
                    var mesh = mf.sharedMesh;
                    for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
                    {
                        Graphics.DrawMesh(mesh, mr.transform.localToWorldMatrix, mat, 0, null, subMesh, _mpb);
                    }
                    continue;
                }

                // SkinnedMeshRenderer - also handles multiple sub-meshes
                var smr = obj.GetComponent<SkinnedMeshRenderer>();
                if (smr != null && smr.sharedMesh != null)
                {
                    smr.GetPropertyBlock(_mpb);
                    _mpb.SetColor(ColorProp, obj.OutlineColor);
                    _mpb.SetFloat(ThicknessProp, obj.OutlineThickness);
                    
                    // Draw all sub-meshes
                    var mesh = smr.sharedMesh;
                    for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
                    {
                        Graphics.DrawMesh(mesh, smr.transform.localToWorldMatrix, mat, 0, null, subMesh, _mpb);
                    }
                }
            }
        }

        private static Material GetMaterial()
        {
            if (_outlineMaterial != null) return _outlineMaterial;
            var shader = Shader.Find("AdvancedOutlineSystem/Outline3D");
            if (shader == null)
            {
                Debug.LogError("[OutlineSystem] Shader 'AdvancedOutlineSystem/Outline3D' not found.");
                return null;
            }
            _outlineMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            return _outlineMaterial;
        }

        private static void CleanupMaterial()
        {
            // Clean up material when not in use to prevent memory leaks
            if (!EditorApplication.isPlaying && _outlineMaterial != null)
            {
                Object.DestroyImmediate(_outlineMaterial);
                _outlineMaterial = null;
            }
        }
    }
}