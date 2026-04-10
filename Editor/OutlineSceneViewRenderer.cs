// ------------------------------------------------------------------------------
// OutlineSceneViewRenderer.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// Renders 3D outlines in the Scene View for real-time editing preview.
    /// This allows users to see outline changes immediately in the editor.
    /// Includes comprehensive debugging to help identify issues.
    /// </summary>
    [InitializeOnLoad]
    public static class OutlineSceneViewRenderer
    {
        private static Material _outlineMaterial;
        private static MaterialPropertyBlock _mpb;
        private static bool _debugEnabled = true;
        private static int _lastFrameCount = -1;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");

        static OutlineSceneViewRenderer()
        {
            Debug.Log("[OutlineSystem] OutlineSceneViewRenderer initialized.");
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.update += CleanupMaterial;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (EditorApplication.isPlaying) return;

            // Prevent multiple draws per frame
            if (Time.frameCount == _lastFrameCount) return;
            _lastFrameCount = Time.frameCount;

            if (_debugEnabled)
            {
                Debug.Log($"[OutlineSystem] SceneView render check - Frame: {Time.frameCount}");
            }

            DrawOutlinesInSceneView();
        }

        private static void DrawOutlinesInSceneView()
        {
            // Check for OutlineManager
            var manager = OutlineManager.Instance;
            if (manager == null)
            {
                if (_debugEnabled)
                    Debug.LogWarning("[OutlineSystem] No OutlineManager instance found. Creating one...");
                
                // Try to create manager
                OutlineManager.GetOrCreateInstance();
                manager = OutlineManager.Instance;
                
                if (manager == null)
                {
                    Debug.LogError("[OutlineSystem] Failed to create OutlineManager!");
                    return;
                }
                
                if (_debugEnabled)
                    Debug.Log("[OutlineSystem] OutlineManager created successfully.");
            }

            if (_debugEnabled)
            {
                Debug.Log($"[OutlineSystem] Objects3D count: {manager.Objects3D.Count}");
            }

            if (manager.Objects3D.Count == 0)
            {
                if (_debugEnabled)
                    Debug.Log("[OutlineSystem] No 3D objects to render outlines for.");
                return;
            }

            var mat = GetMaterial();
            if (mat == null)
            {
                Debug.LogError("[OutlineSystem] Failed to get outline material!");
                return;
            }

            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            int renderedCount = 0;
            int skippedCount = 0;

            foreach (var obj in manager.Objects3D)
            {
                if (obj == null)
                {
                    if (_debugEnabled)
                        Debug.LogWarning("[OutlineSystem] Found null object in manager list.");
                    continue;
                }

                if (!obj.OutlineEnabled)
                {
                    if (_debugEnabled)
                        Debug.Log($"[OutlineSystem] Object '{obj.name}' has outline disabled.");
                    skippedCount++;
                    continue;
                }

                if (_debugEnabled)
                {
                    Debug.Log($"[OutlineSystem] Rendering outline for '{obj.name}' - Color: {obj.OutlineColor}, Thickness: {obj.OutlineThickness}");
                }

                // MeshRenderer - handles multiple materials
                var mr = obj.GetComponent<MeshRenderer>();
                var mf = obj.GetComponent<MeshFilter>();
                if (mr != null && mf != null && mf.sharedMesh != null)
                {
                    if (_debugEnabled)
                    {
                        Debug.Log($"[OutlineSystem] Found MeshRenderer on '{obj.name}' with mesh: {mf.sharedMesh.name}, subMeshCount: {mf.sharedMesh.subMeshCount}");
                    }

                    mr.GetPropertyBlock(_mpb);
                    _mpb.SetColor(ColorProp, obj.OutlineColor);
                    _mpb.SetFloat(ThicknessProp, obj.OutlineThickness);
                    
                    // Draw all sub-meshes (handles multiple materials)
                    var mesh = mf.sharedMesh;
                    for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
                    {
                        Graphics.DrawMesh(mesh, mr.transform.localToWorldMatrix, mat, 0, null, subMesh, _mpb);
                        renderedCount++;
                    }
                    continue;
                }

                // SkinnedMeshRenderer - also handles multiple sub-meshes
                var smr = obj.GetComponent<SkinnedMeshRenderer>();
                if (smr != null && smr.sharedMesh != null)
                {
                    if (_debugEnabled)
                    {
                        Debug.Log($"[OutlineSystem] Found SkinnedMeshRenderer on '{obj.name}' with mesh: {smr.sharedMesh.name}, subMeshCount: {smr.sharedMesh.subMeshCount}");
                    }

                    smr.GetPropertyBlock(_mpb);
                    _mpb.SetColor(ColorProp, obj.OutlineColor);
                    _mpb.SetFloat(ThicknessProp, obj.OutlineThickness);
                    
                    // Draw all sub-meshes
                    var mesh = smr.sharedMesh;
                    for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
                    {
                        Graphics.DrawMesh(mesh, smr.transform.localToWorldMatrix, mat, 0, null, subMesh, _mpb);
                        renderedCount++;
                    }
                    continue;
                }

                if (_debugEnabled)
                {
                    Debug.LogWarning($"[OutlineSystem] Object '{obj.name}' has no valid MeshRenderer or SkinnedMeshRenderer!");
                }
                skippedCount++;
            }

            if (_debugEnabled)
            {
                Debug.Log($"[OutlineSystem] Rendered {renderedCount} outline meshes, skipped {skippedCount} objects.");
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
            Debug.Log($"[OutlineSystem] Created outline material with shader: {shader.name}");
            return _outlineMaterial;
        }

        private static void CleanupMaterial()
        {
            // Clean up material when not in use to prevent memory leaks
            if (!EditorApplication.isPlaying && _outlineMaterial != null)
            {
                Object.DestroyImmediate(_outlineMaterial);
                _outlineMaterial = null;
                if (_debugEnabled)
                    Debug.Log("[OutlineSystem] Cleaned up outline material.");
            }
        }

        /// <summary>
        /// Toggle debug logging on/off
        /// </summary>
        public static void ToggleDebug()
        {
            _debugEnabled = !_debugEnabled;
            string status = _debugEnabled ? "enabled" : "disabled";
            Debug.Log($"[OutlineSystem] Debug logging {status}.");
        }

        /// <summary>
        /// Force refresh the scene view outlines
        /// </summary>
        public static void ForceRefreshScene()
        {
            _lastFrameCount = -1;
            _outlineMaterial = null;
            SceneView.RepaintAll();
            Debug.Log("[OutlineSystem] Force refresh triggered.");
        }
    }
}