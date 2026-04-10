// ------------------------------------------------------------------------------
// OutlineSystemValidator.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;

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
        }

        private static void CheckShader(string name)
        {
            if (Shader.Find(name) == null)
                Debug.LogError($"[OutlineSystem] Shader '{name}' not found. " +
                               "Ensure the shader is included in Graphics Settings or in a Resources folder.");
        }
    }
}