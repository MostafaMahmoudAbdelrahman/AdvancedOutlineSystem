// ------------------------------------------------------------------------------
// OutlineRenderFeature.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// URP ScriptableRendererFeature that injects the outline passes.
    /// Compatible with Unity 6 / URP 17 RenderGraph API.
    /// Add via the URP Renderer asset Inspector.
    /// </summary>
    public class OutlineRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class OutlineSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            [Range(0f, 20f)] public float defaultThickness = 3f;
            public Color defaultColor = Color.white;
        }

        public OutlineSettings settings = new OutlineSettings();

        private OutlineRenderPass      _pass3D;
        private ScreenSpaceOutlinePass _passScreen;
        private Outline2DPass          _pass2D;

        public override void Create()
        {
            _pass3D = new OutlineRenderPass(
                "Outline 3D Pass",
                settings.renderPassEvent,
                OutlineMode.Outline3D);

            _passScreen = new ScreenSpaceOutlinePass(
                "Screen Space Outline Pass",
                settings.renderPassEvent);

            _pass2D = new Outline2DPass(
                "Outline 2D Pass",
                settings.renderPassEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (OutlineManager.Instance == null) return;

            if (OutlineManager.Instance.Objects3D.Count > 0)
                renderer.EnqueuePass(_pass3D);

            if (OutlineManager.Instance.ObjectsScreen.Count > 0)
                renderer.EnqueuePass(_passScreen);

            if (OutlineManager.Instance.Objects2D.Count > 0)
                renderer.EnqueuePass(_pass2D);
        }

        protected override void Dispose(bool disposing)
        {
            _pass3D?.Dispose();
            _passScreen?.Dispose();
            _pass2D?.Dispose();
        }
    }
}
