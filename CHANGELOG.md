# Changelog

All notable changes are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.0.8] — 2026-04-10

### Added
- **Auto-detection of outline mode**: `OutlineObject` now automatically detects whether to use 2D or 3D outline based on the renderer type (SpriteRenderer → 2D, MeshRenderer/SkinnedMeshRenderer → 3D)
- **Auto-creation of OutlineManager**: No longer need to manually add OutlineManager to scene — it's created automatically when the first OutlineObject is enabled
- **Multi-material support**: OutlineRenderPass now properly handles MeshRenderers with multiple materials/sub-meshes
- **Enhanced Editor**: Custom inspector now shows detected renderer type and auto-detect mode toggle
- **Improved validation**: OutlineSystemValidator now checks for OutlineRenderFeature in URP Renderer

### Changed
- `OutlineObject` now has `[RequireComponent(typeof(Renderer))]` attribute to ensure proper setup
- Added `AutoDetectMode` property to allow manual override when needed
- Updated `.gitignore` to allow `.unitypackage` files to be tracked in RELEASES folder

### Fixed
- Outline not working when OutlineManager was missing from scene
- Manual mode selection being overridden — now shows warning when AutoDetectMode is enabled
- Multiple materials on single MeshRenderer not being outlined correctly

---



### Fixed
- Migrated `OutlineRenderPass`, `ScreenSpaceOutlinePass`, `Outline2DPass` to Unity 6 / URP 17 **RenderGraph API** (`RecordRenderGraph` + `AddUnsafePass`) — resolves `CommandBufferPool` not found and `cameraColorTarget` obsolete errors
- Fixed `OutlineShadowAtlasFix` — removed ambiguous `ShadowResolution` cast; reads atlas size as `int` and writes back via explicit URP enum cast
- Updated Runtime and Editor asmdefs to reference `Unity.RenderPipelines.Core.Runtime` — resolves Burst `AdvancedOutlineSystem.Editor` assembly resolution failure
- Removed stale `versionDefines` from Runtime asmdef

---



### Fixed
- Added `.meta` files for all package assets — eliminates "no meta file, immutable folder" warnings when installed via Git URL in Unity Package Manager
- Added `OutlineShadowAtlasFix` component — automatically upgrades the URP additional-lights shadow atlas to 4096 when needed, resolving the "shadow atlas too small" console warning
- Added `OutlineShadowAtlasFix.cs.meta` for proper Unity import

---

## [1.0.0] — 2026-04-10

### Added
- `OutlineObject` — per-object color, thickness, enable/disable, and mode control
- `OutlineManager` — zero-allocation singleton registry with batched render lists
- `OutlineRenderFeature` — URP `ScriptableRendererFeature` injecting three passes
- `OutlineRenderPass` — 3D inverted-hull outline via back-face vertex normal expansion
- `ScreenSpaceOutlinePass` — full-screen depth + normal Roberts cross edge detection
- `Outline2DPass` — sprite alpha-based edge detection for `SpriteRenderer`
- `Outline3D.shader` — URP HLSL, inverted hull, GPU instancing support
- `Outline2D.shader` — URP HLSL, alpha neighbour sampling, transparent blend
- `ScreenSpaceOutline.shader` — URP HLSL, `DeclareDepthTexture` + `DeclareNormalsTexture`
- `OutlineObjectEditor` — custom Inspector with sliders, color picker, mode selector
- `OutlinePackageExporter` — Tools menu export and validation utilities
- `OutlineSystemValidator` — startup shader and manager presence checks
- `DemoSceneSetup` — procedural runtime demo, no hard scene dependencies
- Full documentation: README, SetupGuide, API reference
- Assembly definitions for Runtime (no Editor refs) and Editor (Editor-only, references Runtime)
