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
    /// Exports the Advanced Outline System as a .unitypackage.
    ///
    /// When installed via Git URL the package lives in Library/PackageCache
    /// which AssetDatabase cannot export directly. This tool copies the package
    /// into a temporary Assets/ folder, exports, then cleans up automatically.
    /// </summary>
    public static class OutlinePackageExporter
    {
        private const string PackageName    = "com.mostafa.advancedoutline";
        private const string TempAssetPath  = "Assets/AdvancedOutlineSystem_Export";
        private const string OutputFileName = "AdvancedOutlineSystem.unitypackage";

        // ── Export ────────────────────────────────────────────────────────────

        [MenuItem("Tools/Advanced Outline System/Export .unitypackage")]
        public static void Export()
        {
            string sourcePath = FindPackageSourcePath();
            if (string.IsNullOrEmpty(sourcePath))
            {
                Debug.LogError("[OutlineSystem] Cannot locate package source. " +
                               "Make sure 'com.mostafa.advancedoutline' is installed.");
                return;
            }

            Debug.Log($"[OutlineSystem] Package source: {sourcePath}");

            // Copy into Assets/ so AssetDatabase can see it
            if (Directory.Exists(TempAssetPath))
                Directory.Delete(TempAssetPath, true);

            CopyDirectory(sourcePath, TempAssetPath);
            AssetDatabase.Refresh();

            string outputPath = Path.Combine(
                Directory.GetParent(Application.dataPath).FullName,
                OutputFileName);

            try
            {
                AssetDatabase.ExportPackage(
                    TempAssetPath,
                    outputPath,
                    ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

                Debug.Log($"[OutlineSystem] ✓ Package exported → {outputPath}");
                EditorUtility.RevealInFinder(outputPath);
            }
            finally
            {
                Cleanup();
            }
        }

        // ── Validate ──────────────────────────────────────────────────────────

        [MenuItem("Tools/Advanced Outline System/Validate Setup")]
        public static void ValidateSetup()
        {
            bool ok = true;

            // Shaders
            foreach (var s in new[]
            {
                "AdvancedOutlineSystem/Outline3D",
                "AdvancedOutlineSystem/Outline2D",
                "AdvancedOutlineSystem/ScreenSpaceOutline"
            })
            {
                if (Shader.Find(s) == null)
                { Debug.LogError($"[OutlineSystem] ✗ Missing shader: {s}"); ok = false; }
                else
                    Debug.Log($"[OutlineSystem] ✓ Shader found: {s}");
            }

            // Package source
            string source = FindPackageSourcePath();
            if (string.IsNullOrEmpty(source))
            {
                Debug.LogError("[OutlineSystem] ✗ Package source folder not found.");
                ok = false;
            }
            else
            {
                Debug.Log($"[OutlineSystem] ✓ Package source: {source}");
                LogCheck("package.json",   File.Exists(Path.Combine(source, "package.json")),                                          ref ok);
                LogCheck("Runtime asmdef", File.Exists(Path.Combine(source, "Runtime", "AdvancedOutlineSystem.Runtime.asmdef")),        ref ok);
                LogCheck("Editor asmdef",  File.Exists(Path.Combine(source, "Editor",  "AdvancedOutlineSystem.Editor.asmdef")),         ref ok);
            }

            Debug.Log(ok
                ? "[OutlineSystem] ✓ All validations passed. Ready for export."
                : "[OutlineSystem] ⚠ Validation completed with errors. Fix before exporting.");
        }

        // ── Source path discovery ─────────────────────────────────────────────

        /// <summary>
        /// Finds the package root on disk using three strategies in order:
        ///   1. Scan Library/PackageCache for a folder starting with the package name
        ///   2. Check the Packages/ folder (local path installs)
        ///   3. Check Assets/ (manual copy installs)
        /// Deliberately avoids PackageManager.Client.List — it requires async
        /// callbacks and is unreliable when called synchronously in Editor menus.
        /// </summary>
        public static string FindPackageSourcePath()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;

            // Strategy 1 — Library/PackageCache (Git URL / registry installs)
            string cacheRoot = Path.Combine(projectRoot, "Library", "PackageCache");
            if (Directory.Exists(cacheRoot))
            {
                foreach (var dir in Directory.GetDirectories(cacheRoot))
                {
                    string name = Path.GetFileName(dir);
                    if (name.StartsWith(PackageName))
                    {
                        Debug.Log($"[OutlineSystem] Found in PackageCache: {dir}");
                        return dir;
                    }
                }
            }

            // Strategy 2 — Packages/ folder (local path installs)
            string packagesFolder = Path.Combine(projectRoot, "Packages", PackageName);
            if (Directory.Exists(packagesFolder))
            {
                Debug.Log($"[OutlineSystem] Found in Packages/: {packagesFolder}");
                return packagesFolder;
            }

            // Strategy 3 — Assets/ (manual copy)
            string assetsFolder = Path.Combine(Application.dataPath, "AdvancedOutlineSystem");
            if (Directory.Exists(assetsFolder))
            {
                Debug.Log($"[OutlineSystem] Found in Assets/: {assetsFolder}");
                return assetsFolder;
            }

            return null;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);

            foreach (var file in Directory.GetFiles(source))
            {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".meta")) continue; // Unity regenerates these
                File.Copy(file, Path.Combine(dest, fileName), true);
            }

            foreach (var dir in Directory.GetDirectories(source))
            {
                string dirName = Path.GetFileName(dir);
                // Skip .git and tilde folders (Samples~, Documentation~)
                if (dirName == ".git" || dirName.EndsWith("~")) continue;
                CopyDirectory(dir, Path.Combine(dest, dirName));
            }
        }

        private static void Cleanup()
        {
            try
            {
                if (Directory.Exists(TempAssetPath))
                    Directory.Delete(TempAssetPath, true);

                string meta = TempAssetPath + ".meta";
                if (File.Exists(meta)) File.Delete(meta);

                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[OutlineSystem] Cleanup warning: {e.Message}");
            }
        }

        private static void LogCheck(string label, bool condition, ref bool ok)
        {
            if (condition)
                Debug.Log($"[OutlineSystem] ✓ {label} found.");
            else
            {
                Debug.LogError($"[OutlineSystem] ✗ {label} missing.");
                ok = false;
            }
        }
    }
}
