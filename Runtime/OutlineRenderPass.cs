// ------------------------------------------------------------------------------
// OutlineRenderPass.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Renders 3D outlines using the inverted-hull technique.
    /// Expands vertices along normals, renders back-faces only.
    /// Uses MaterialPropertyBlock — zero per-frame allocations.
    /// Works in URP with Compatibility Mode enabled.
    /// </summary>
    public class OutlineRenderPass : ScriptableRenderPass, System.IDisposable
    {
        private readonly string       _profilerTag;
        private Material              _outlineMaterial;
        private MaterialPropertyBlock _mpb;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");

        public OutlineRenderPass(string tag, RenderPassEvent evt, OutlineMode mode)
        {
            _profilerTag    = tag;
            renderPassEvent = evt;
            _mpb            = new MaterialPropertyBlock();
        }

        private Material GetMaterial()
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

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var manager = OutlineManager.Instance;
            if (manager == null || manager.Objects3D.Count == 0) return;

            var mat = GetMaterial();
            if (mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

            foreach (var obj in manager.Objects3D)
            {
                if (obj == null || !obj.OutlineEnabled) continue;

                // MeshRenderer
                var mr = obj.GetComponent<MeshRenderer>();
                var mf = obj.GetComponent<MeshFilter>();
                if (mr != null && mf != null && mf.sharedMesh != null)
                {
                    mr.GetPropertyBlock(_mpb);
                    _mpb.SetColor(ColorProp, obj.OutlineColor);
                    _mpb.SetFloat(ThicknessProp, obj.OutlineThickness);
                    for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
                        cmd.DrawRenderer(mr, mat, i, 0);
                    continue;
                }

                // SkinnedMeshRenderer
                var smr = obj.GetComponent<SkinnedMeshRenderer>();
                if (smr != null && smr.sharedMesh != null)
                {
                    smr.GetPropertyBlock(_mpb);
                    _mpb.SetColor(ColorProp, obj.OutlineColor);
                    _mpb.SetFloat(ThicknessProp, obj.OutlineThickness);
                    for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)
                        cmd.DrawRenderer(smr, mat, i, 0);
                }
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            if (_outlineMaterial != null)
            {
                Object.DestroyImmediate(_outlineMaterial);
                _outlineMaterial = null;
            }
        }
    }
}
