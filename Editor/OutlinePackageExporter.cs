// ------------------------------------------------------------------------------
// OutlinePackageExporter.cs
// Copyright (c) 2026 Mostafa Mahmoud Abdelrahman
// Website: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/
// ------------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;

namespace AdvancedOutlineSystem.Editor
{
    /// <summary>
    /// Editor utilities for exporting and validating the Advanced Outline System package.
    /// Access via Tools → Advanced Outline System menu.
    /// </summary>
    public static class OutlinePackageExporter
    {
        private const string PackagePath = "Assets/AdvancedOutlineSystem";
        private const string OutputFile  = "AdvancedOutlineSystem.unitypackage";

        [MenuItem("Tools/Advanced Outline System/Export .unitypackage")]
        public static void Export()
        {
            string outputPath = Path.Combine(
                Directory.GetParent(Application.dataPath).FullName,
                OutputFile);

            AssetDatabase.ExportPackage(
                PackagePath,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

            Debug.Log($"[OutlineSystem] Package exported → {outputPath}");
            EditorUtility.RevealInFinder(outputPath);
        }

        [MenuItem("Tools/Advanced Outline System/Validate Setup")]
        public static void ValidateSetup()
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
                {
                    Debug.Log($"[OutlineSystem] ✓ Shader found: {s}");
                }
            }

            string pkgJson = Path.Combine(
                Application.dataPath, "AdvancedOutlineSystem", "package.json");
            if (!File.Exists(pkgJson))
            {
                Debug.LogError("[OutlineSystem] ✗ package.json not found.");
                ok = false;
            }
            else Debug.Log("[OutlineSystem] ✓ package.json found.");

            string runtimeAsmdef = Path.Combine(Application.dataPath,
                "AdvancedOutlineSystem", "Runtime", "AdvancedOutlineSystem.Runtime.asmdef");
            string editorAsmdef = Path.Combine(Application.dataPath,
                "AdvancedOutlineSystem", "Editor", "AdvancedOutlineSystem.Editor.asmdef");

            if (!File.Exists(runtimeAsmdef))
            { Debug.LogError("[OutlineSystem] ✗ Runtime asmdef missing."); ok = false; }
            else Debug.Log("[OutlineSystem] ✓ Runtime asmdef found.");

            if (!File.Exists(editorAsmdef))
            { Debug.LogError("[OutlineSystem] ✗ Editor asmdef missing."); ok = false; }
            else Debug.Log("[OutlineSystem] ✓ Editor asmdef found.");

            Debug.Log(ok
                ? "[OutlineSystem] ✓ All validations passed. Ready for export."
                : "[OutlineSystem] ⚠ Validation completed with errors. Fix before exporting.");
        }
    }
}
