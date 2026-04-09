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
    /// IMPORTANT: AssetDatabase.ExportPackage only works on assets that live
    /// inside the project's Assets/ folder. When the package is installed via
    /// Git URL it lives in Library/PackageCache and cannot be exported directly.
    ///
    /// This tool copies the package into Assets/AdvancedOutlineSystem_Export/,
    /// exports from there, then removes the temporary copy automatically.
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
            // 1. Locate the package source on disk
            string sourcePath = FindPackageSourcePath();
            if (string.IsNullOrEmpty(sourcePath))
            {
                Debug.LogError(
                    "[OutlineSystem] Cannot locate package source. " +
                    "Make sure 'com.mostafa.advancedoutline' is installed.");
                return;
            }

            // 2. Copy into Assets/ so AssetDatabase can see it
            if (Directory.Exists(TempAssetPath))
                Directory.Delete(TempAssetPath, true);

            CopyDirectory(sourcePath, TempAssetPath);
            AssetDatabase.Refresh();

            // 3. Export
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
                // 4. Always clean up the temporary copy
                if (Directory.Exists(TempAssetPath))
                    Directory.Delete(TempAssetPath, true);

                string metaFile = TempAssetPath + ".meta";
                if (File.Exists(metaFile))
                    File.Delete(metaFile);

                AssetDatabase.Refresh();
            }
        }

        // ── Validate ──────────────────────────────────────────────────────────

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

            string source = FindPackageSourcePath();
            if (string.IsNullOrEmpty(source))
            {
                Debug.LogError("[OutlineSystem] ✗ Package source folder not found.");
                ok = false;
            }
            else
            {
                Debug.Log($"[OutlineSystem] ✓ Package source: {source}");

                string runtimeAsmdef = Path.Combine(source, "Runtime",
                    "AdvancedOutlineSystem.Runtime.asmdef");
                string editorAsmdef  = Path.Combine(source, "Editor",
                    "AdvancedOutlineSystem.Editor.asmdef");
                string pkgJson       = Path.Combine(source, "package.json");

                LogCheck("package.json",       File.Exists(pkgJson),       ref ok);
                LogCheck("Runtime asmdef",     File.Exists(runtimeAsmdef), ref ok);
                LogCheck("Editor asmdef",      File.Exists(editorAsmdef),  ref ok);
            }

            Debug.Log(ok
                ? "[OutlineSystem] ✓ All validations passed."
                : "[OutlineSystem] ⚠ Validation completed with errors.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Finds the package root on disk regardless of how it was installed
        /// (Git URL → PackageCache, local path, or copied into Assets/).
        /// </summary>
        private static string FindPackageSourcePath()
        {
            // Strategy 1: UnityEditor.PackageManager resolved path
            var listRequest = UnityEditor.PackageManager.Client.List(true);
            while (!listRequest.IsCompleted) { /* spin — fast in Editor */ }

            if (listRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                foreach (var pkg in listRequest.Result)
                {
                    if (pkg.name == PackageName)
                    {
                        string resolved = pkg.resolvedPath;
                        if (!string.IsNullOrEmpty(resolved) && Directory.Exists(resolved))
                            return resolved;
                    }
                }
            }

            // Strategy 2: Scan Library/PackageCache
            string cacheRoot = Path.Combine(
                Directory.GetParent(Application.dataPath).FullName,
                "Library", "PackageCache");

            if (Directory.Exists(cacheRoot))
            {
                foreach (var dir in Directory.GetDirectories(cacheRoot))
                {
                    string dirName = Path.GetFileName(dir);
                    if (dirName.StartsWith(PackageName))
                        return dir;
                }
            }

            // Strategy 3: Already in Assets/
            string assetsPath = Path.Combine(Application.dataPath, "AdvancedOutlineSystem");
            if (Directory.Exists(assetsPath))
                return assetsPath;

            return null;
        }

        private static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.GetFiles(source))
            {
                string fileName = Path.GetFileName(file);
                // Skip .git internals and existing .meta files
                if (fileName.EndsWith(".meta")) continue;
                File.Copy(file, Path.Combine(dest, fileName), true);
            }
            foreach (var dir in Directory.GetDirectories(source))
            {
                string dirName = Path.GetFileName(dir);
                // Skip .git folder and tilde-suffixed folders (Samples~, Documentation~)
                // — they are intentionally excluded from .unitypackage exports
                if (dirName == ".git" || dirName.EndsWith("~")) continue;
                CopyDirectory(dir, Path.Combine(dest, dirName));
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
