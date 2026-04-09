# Changelog

All notable changes are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
