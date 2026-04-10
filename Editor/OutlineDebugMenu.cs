// ------------------------------------------------------------------------------
// OutlineDebugMenu.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// Provides debug menu items for the Advanced Outline System.
    /// </summary>
    public static class OutlineDebugMenu
    {
        private const string MenuRoot = "Tools/Advanced Outline System";

        [MenuItem(MenuRoot + "/Debug/Toggle Debug Logs")]
        public static void ToggleDebugLogs()
        {
            OutlineSceneViewRenderer.ToggleDebug();
        }

        [MenuItem(MenuRoot + "/Debug/Force Refresh")]
        public static void ForceRefresh()
        {
            OutlineSceneViewRenderer.ForceRefreshScene();
        }

        [MenuItem(MenuRoot + "/Debug/Log Current State")]
        public static void LogCurrentState()
        {
            var manager = OutlineManager.Instance;
            if (manager == null)
            {
                Debug.Log("[OutlineSystem] No OutlineManager instance found.");
                return;
            }

            Debug.Log($"[OutlineSystem] OutlineManager found with:");
            Debug.Log($"  - 3D Objects: {manager.Objects3D.Count}");
            Debug.Log($"  - Screen Space Objects: {manager.ObjectsScreen.Count}");
            Debug.Log($"  - 2D Objects: {manager.Objects2D.Count}");

            foreach (var obj in manager.Objects3D)
            {
                if (obj != null)
                {
                    Debug.Log($"  - 3D: {obj.name} | Color: {obj.OutlineColor} | Thickness: {obj.OutlineThickness} | Enabled: {obj.OutlineEnabled}");
                }
            }
        }
    }
}