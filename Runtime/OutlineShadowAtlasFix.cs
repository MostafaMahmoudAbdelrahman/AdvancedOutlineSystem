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
    /// Automatically upgrades the URP additional-lights shadow atlas to 4096.
    /// Eliminates the "shadow atlas too small" console warning.
    /// Attach to the same GameObject as OutlineManager, or any persistent GO.
    /// </summary>
    [AddComponentMenu("Advanced Outline System/Outline Shadow Atlas Fix")]
    public class OutlineShadowAtlasFix : MonoBehaviour
    {
        [Tooltip("Atlas size to apply. Default 4096 fits up to 32 shadow maps.")]
        public int targetAtlasSize = 4096;

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

            int current = (int)urpAsset.additionalLightsShadowmapResolution;

            if (current < targetAtlasSize)
            {
                // Cast to the fully-qualified URP type to avoid ambiguity with UnityEngine.ShadowResolution
                urpAsset.additionalLightsShadowmapResolution =
                    (UnityEngine.Rendering.Universal.ShadowResolution)targetAtlasSize;

                Debug.Log($"[OutlineSystem] Shadow atlas upgraded: {current} → {targetAtlasSize}. " +
                          "Shadow atlas warning resolved.");
            }
        }
    }
}
