// ------------------------------------------------------------------------------
// ScreenSpaceOutlinePass.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Screen-space outline pass using depth + normals edge detection.
    /// Compatible with Unity 6 / URP 17 RenderGraph API.
    /// </summary>
    public class ScreenSpaceOutlinePass : ScriptableRenderPass, System.IDisposable
    {
        private readonly string _profilerTag;
        private Material        _material;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");

        public ScreenSpaceOutlinePass(string tag, RenderPassEvent evt)
        {
            _profilerTag    = tag;
            renderPassEvent = evt;
        }

        private Material GetMaterial()
        {
            if (_material != null) return _material;
            var shader = Shader.Find("AdvancedOutlineSystem/ScreenSpaceOutline");
            if (shader == null)
            {
                Debug.LogError("[OutlineSystem] Shader 'AdvancedOutlineSystem/ScreenSpaceOutline' not found.");
                return null;
            }
            _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            return _material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var manager = OutlineManager.Instance;
            if (manager == null || manager.ObjectsScreen.Count == 0) return;

            var mat = GetMaterial();
            if (mat == null) return;

            var first = manager.ObjectsScreen[0];
            mat.SetColor(ColorProp,     first.OutlineColor);
            mat.SetFloat(ThicknessProp, first.OutlineThickness);

            var resourceData = frameData.Get<UniversalResourceData>();

            using (var builder = renderGraph.AddUnsafePass<PassData>(_profilerTag, out var passData))
            {
                passData.ColorTarget = resourceData.activeColorTexture;
                passData.Material    = mat;

                builder.UseTexture(passData.ColorTarget, AccessFlags.ReadWrite);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, UnsafeGraphContext ctx) =>
                {
                    // Get native CommandBuffer — required for Blitter in Unity 6
                    CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);

                    // Full-screen blit: copy color target through the outline material
                    ctx.cmd.SetRenderTarget(data.ColorTarget);
                    Blitter.BlitTexture(cmd, data.ColorTarget,
                        new Vector4(1, 1, 0, 0), data.Material, 0);
                });
            }
        }

        public void Dispose()
        {
            if (_material != null)
            {
                Object.DestroyImmediate(_material);
                _material = null;
            }
        }

        private class PassData
        {
            public TextureHandle ColorTarget;
            public Material      Material;
        }
    }
}
