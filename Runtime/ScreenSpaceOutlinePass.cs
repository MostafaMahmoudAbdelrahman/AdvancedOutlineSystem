// ------------------------------------------------------------------------------
// ScreenSpaceOutlinePass.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Screen-space outline pass using depth + normals edge detection.
    /// Full-screen blit via CommandBufferPool — works in URP with
    /// Compatibility Mode enabled (Project Settings → Graphics → Render Graph).
    /// </summary>
    public class ScreenSpaceOutlinePass : ScriptableRenderPass, System.IDisposable
    {
        private readonly string _profilerTag;
        private Material        _material;

        private RenderTargetIdentifier  _cameraColorTarget;
        private RenderTextureDescriptor _descriptor;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");
        private static readonly int TempTexId     = Shader.PropertyToID("_ScreenSpaceOutlineTemp");

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

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _descriptor        = renderingData.cameraData.cameraTargetDescriptor;
            _descriptor.depthBufferBits = 0;
            _cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var manager = OutlineManager.Instance;
            if (manager == null || manager.ObjectsScreen.Count == 0) return;

            var mat = GetMaterial();
            if (mat == null) return;

            var first = manager.ObjectsScreen[0];
            mat.SetColor(ColorProp,     first.OutlineColor);
            mat.SetFloat(ThicknessProp, first.OutlineThickness);

            CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

            cmd.GetTemporaryRT(TempTexId, _descriptor);
            cmd.Blit(_cameraColorTarget, TempTexId, mat);
            cmd.Blit(TempTexId, _cameraColorTarget);
            cmd.ReleaseTemporaryRT(TempTexId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // No persistent RT to release
        }

        public void Dispose()
        {
            if (_material != null)
            {
                Object.DestroyImmediate(_material);
                _material = null;
            }
        }
    }
}
