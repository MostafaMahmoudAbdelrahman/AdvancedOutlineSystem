// ------------------------------------------------------------------------------
// OutlineValidator.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// Validates that all required shaders and assets are present.
    /// Called from the Welcome Window, Documentation Window, and the Tools menu.
    /// </summary>
    public static class OutlineValidator
    {
        [MenuItem("Tools/Advanced Outline System/Validate Setup")]
        public static void RunValidation()
        {
            bool ok = true;

            string[] shaders =
            {
                "AdvancedOutlineSystem/Outline3D",
                "AdvancedOutlineSystem/Outline2D",
                "AdvancedOutlineSystem/ScreenSpaceOutline"
            };

            foreach (var s in shaders)
            {
                if (Shader.Find(s) == null)
                {
                    Debug.LogError($"[OutlineSystem] ✗ Missing shader: {s}");
                    ok = false;
                }
                else
                    Debug.Log($"[OutlineSystem] ✓ Shader found: {s}");
            }

            if (ok)
                Debug.Log("[OutlineSystem] ✓ All shaders validated. Package is ready to use.");
            else
                Debug.LogWarning("[OutlineSystem] ⚠ Some shaders are missing. Try re-importing the package.");
        }
    }
}
