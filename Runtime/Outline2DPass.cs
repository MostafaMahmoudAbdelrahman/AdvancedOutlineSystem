// ------------------------------------------------------------------------------
// Outline2DPass.cs
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
    /// 2D outline pass for SpriteRenderer objects.
    /// Compatible with Unity 6 / URP 17 RenderGraph API.
    /// </summary>
    public class Outline2DPass : ScriptableRenderPass, System.IDisposable
    {
        private readonly string _profilerTag;
        private Material        _material;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");
        private static readonly int MainTexProp   = Shader.PropertyToID("_MainTex");

        public Outline2DPass(string tag, RenderPassEvent evt)
        {
            _profilerTag    = tag;
            renderPassEvent = evt;
        }

        private Material GetMaterial()
        {
            if (_material != null) return _material;
            var shader = Shader.Find("AdvancedOutlineSystem/Outline2D");
            if (shader == null)
            {
                Debug.LogError("[OutlineSystem] Shader 'AdvancedOutlineSystem/Outline2D' not found.");
                return null;
            }
            _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            return _material;
        }

        // ── Unity 6 / URP 17 RenderGraph path ────────────────────────────────
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var manager = OutlineManager.Instance;
            if (manager == null || manager.Objects2D.Count == 0) return;

            var mat = GetMaterial();
            if (mat == null) return;

            var resourceData = frameData.Get<UniversalResourceData>();

            using (var builder = renderGraph.AddUnsafePass<PassData>(_profilerTag, out var passData))
            {
                passData.ColorTarget = resourceData.activeColorTexture;
                passData.Pass        = this;
                passData.Material    = mat;

                builder.UseTexture(passData.ColorTarget, AccessFlags.Write);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData data, UnsafeGraphContext ctx) =>
                    data.Pass.ExecutePass(ctx.cmd, data.Material));
            }
        }

        private void ExecutePass(UnsafeCommandBuffer cmd, Material mat)
        {
            var manager = OutlineManager.Instance;
            if (manager == null) return;

            foreach (var obj in manager.Objects2D)
            {
                if (obj == null || !obj.OutlineEnabled) continue;

                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr == null || sr.sprite == null) continue;

                mat.SetColor(ColorProp,     obj.OutlineColor);
                mat.SetFloat(ThicknessProp, obj.OutlineThickness);
                mat.SetTexture(MainTexProp, sr.sprite.texture);

                cmd.DrawRenderer(sr, mat, 0, 0);
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
            public Outline2DPass Pass;
            public Material      Material;
        }
    }
}
