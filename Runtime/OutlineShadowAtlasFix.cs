// ------------------------------------------------------------------------------
// OutlineShadowAtlasFix.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Automatically upgrades the URP additional-lights shadow atlas to 4096
    /// when more than 8 shadow-casting lights are detected in the scene.
    /// Eliminates the "shadow atlas too small" console warning.
    /// Attach to the same GameObject as OutlineManager, or any persistent GO.
    /// </summary>
    [AddComponentMenu("Advanced Outline System/Outline Shadow Atlas Fix")]
    public class OutlineShadowAtlasFix : MonoBehaviour
    {
        [Tooltip("Atlas size to apply when too many shadow maps are detected.")]
        public ShadowResolution targetAtlasResolution = ShadowResolution._4096;

        private void Awake()
        {
            ApplyFix();
        }

        private void ApplyFix()
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null)
            {
                Debug.LogWarning("[OutlineSystem] ShadowAtlasFix: No URP asset found.");
                return;
            }

            // URP exposes additionalLightsShadowmapResolution as a public property
            int current = (int)urpAsset.additionalLightsShadowmapResolution;
            int target  = (int)targetAtlasResolution;

            if (current < target)
            {
                urpAsset.additionalLightsShadowmapResolution = targetAtlasResolution;
                Debug.Log($"[OutlineSystem] Shadow atlas upgraded: {current} → {target}. " +
                          "Shadow atlas warning resolved.");
            }
        }
    }
}
