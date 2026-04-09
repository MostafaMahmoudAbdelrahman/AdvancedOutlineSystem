// ------------------------------------------------------------------------------
// OutlineShadowAtlasFix.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Logs a clear recommendation when the shadow atlas is too small.
    /// In URP 17 (Unity 6) additionalLightsShadowmapResolution is read-only at runtime;
    /// the atlas must be configured in the URP Asset before play mode.
    /// </summary>
    [AddComponentMenu("Advanced Outline System/Outline Shadow Atlas Fix")]
    public class OutlineShadowAtlasFix : MonoBehaviour
    {
        [Tooltip("Recommended minimum atlas size. If the current URP Asset value is smaller, a warning is logged.")]
        public int recommendedAtlasSize = 4096;

        private void Awake() => CheckAtlas();

        private void CheckAtlas()
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline;
            if (urpAsset == null)
            {
                Debug.LogWarning("[OutlineSystem] ShadowAtlasFix: No render pipeline asset found.");
                return;
            }

            // Use reflection to read the value safely across URP versions
            var prop = urpAsset.GetType().GetProperty("additionalLightsShadowmapResolution");
            if (prop == null)
            {
                Debug.LogWarning("[OutlineSystem] ShadowAtlasFix: Could not read shadow atlas size via reflection.");
                return;
            }

            int current = (int)prop.GetValue(urpAsset);
            if (current < recommendedAtlasSize)
            {
                Debug.LogWarning(
                    $"[OutlineSystem] Shadow atlas is {current}. " +
                    $"Recommended: {recommendedAtlasSize}. " +
                    "To fix: select your URP Asset → Shadows → Additional Lights Shadow Atlas Resolution → set to 4096.");
            }
            else
            {
                Debug.Log($"[OutlineSystem] Shadow atlas OK ({current}).");
            }
        }
    }
}
