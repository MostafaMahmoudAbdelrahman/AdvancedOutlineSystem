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
    /// Checks the URP shadow atlas size on startup and logs a warning
    /// if it is too small to fit all active shadow maps without quality loss.
    /// To fix the warning: URP Asset → Shadows → Additional Lights Shadow Atlas → 4096.
    /// </summary>
    [AddComponentMenu("Advanced Outline System/Outline Shadow Atlas Fix")]
    public class OutlineShadowAtlasFix : MonoBehaviour
    {
        [Tooltip("Minimum recommended atlas size in pixels. 4096 fits up to 32 shadow maps.")]
        public int recommendedAtlasSize = 4096;

        private void Awake() => CheckAtlas();

        private void CheckAtlas()
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null)
            {
                Debug.LogWarning("[OutlineSystem] ShadowAtlasFix: No URP asset found.");
                return;
            }

            // additionalLightsShadowmapResolution is UnityEngine.Rendering.Universal.ShadowResolution
            // Cast to int explicitly to compare against the recommended size.
            int current = (int)urpAsset.additionalLightsShadowmapResolution;

            if (current < recommendedAtlasSize)
            {
                Debug.LogWarning(
                    $"[OutlineSystem] Additional lights shadow atlas is {current}px. " +
                    $"Recommended minimum: {recommendedAtlasSize}px. " +
                    "Fix: select your URP Asset → Shadows → " +
                    "Additional Lights Shadow Atlas Resolution → set to 4096.");
            }
            else
            {
                Debug.Log($"[OutlineSystem] Shadow atlas size OK: {current}px.");
            }
        }
    }
}
