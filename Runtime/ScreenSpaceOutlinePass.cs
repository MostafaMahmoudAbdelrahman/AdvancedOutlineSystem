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
    /// Renders a full-screen blit that detects edges from the depth/normal buffer.
    /// </summary>
    public class ScreenSpaceOutlinePass : ScriptableRenderPass, System.IDisposable
    {
        private readonly string _profilerTag;
        private Material        _material;

        private static readonly int ColorProp     = Shader.PropertyToID("_OutlineColor");
        private static readonly int ThicknessProp = Shader.PropertyToID("_OutlineThickness");

        private RenderTargetIdentifier  _cameraColorTarget;
        private RenderTextureDescriptor _descriptor;

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
            _cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var manager = OutlineManager.Instance;
            if (manager == null) return;

            var objects = manager.ObjectsScreen;
            if (objects.Count == 0) return;

            var mat = GetMaterial();
            if (mat == null) return;

            // First registered screen-space object drives the global settings
            var first = objects[0];
            mat.SetColor(ColorProp,     first.OutlineColor);
            mat.SetFloat(ThicknessProp, first.OutlineThickness);

            var cmd = CommandBufferPool.Get(_profilerTag);

            int tempId = Shader.PropertyToID("_ScreenSpaceOutlineTemp");
            cmd.GetTemporaryRT(tempId, _descriptor);
            cmd.Blit(_cameraColorTarget, tempId, mat);
            cmd.Blit(tempId, _cameraColorTarget);
            cmd.ReleaseTemporaryRT(tempId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
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
