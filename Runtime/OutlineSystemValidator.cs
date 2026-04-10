// ------------------------------------------------------------------------------
// OutlineSystemValidator.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AdvancedOutlineSystem
{
    /// <summary>
    /// Runtime validator — logs warnings if the system is misconfigured.
    /// Runs once on Awake in the Editor and in Development builds.
    /// </summary>
    public class OutlineSystemValidator : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Validate();
#endif
        }

        private void Validate()
        {
            CheckShader("AdvancedOutlineSystem/Outline3D");
            CheckShader("AdvancedOutlineSystem/Outline2D");
            CheckShader("AdvancedOutlineSystem/ScreenSpaceOutline");

            // Check if OutlineManager exists - if not, it will be auto-created
            if (OutlineManager.Instance == null)
            {
                Debug.Log("[OutlineSystem] No OutlineManager found in scene. " +
                         "One will be created automatically when the first OutlineObject is enabled.");
            }

            // Check URP Renderer for OutlineRenderFeature
            CheckRenderFeature();
        }

        private static void CheckShader(string name)
        {
            if (Shader.Find(name) == null)
                Debug.LogError($"[OutlineSystem] Shader '{name}' not found. " +
                               "Ensure the shader is included in Graphics Settings or in a Resources folder.");
        }

        private static void CheckRenderFeature()
        {
            // Get the current pipeline asset
            var pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipelineAsset == null)
            {
                Debug.LogWarning("[OutlineSystem] Universal Render Pipeline asset not found. " +
                                "Ensure you're using URP.");
                return;
            }

            // Check all renderer data
            var renderers = pipelineAsset.GetRendererVariantList();
            bool foundFeature = false;

            foreach (var rendererData in renderers)
            {
                if (rendererData == null) continue;

                var features = rendererData.rendererFeatures;
                if (features != null)
                {
                    foreach (var feature in features)
                    {
                        if (feature is OutlineRenderFeature && feature.isActive)
                        {
                            foundFeature = true;
                            break;
                        }
                    }
                }

                if (foundFeature) break;
            }

            if (!foundFeature)
            {
                Debug.LogWarning("[OutlineSystem] OutlineRenderFeature not found in any active URP Renderer. " +
                                "Outlines will not render. " +
                                "Add OutlineRenderFeature to your URP Renderer Data asset.");
            }
        }
    }
}