# Setup Guide

## 1. URP Renderer Feature

1. Find your **URP Renderer** asset — default: `Assets/Settings/UniversalRenderer.asset`
2. Select it and click **Add Renderer Feature** in the Inspector
3. Choose **Outline Render Feature**
4. The feature is active with these defaults:
   - Render Pass Event: `Before Rendering Post Processing`
   - Default Thickness: `3`
   - Default Color: White

> If **Outline Render Feature** is not listed, ensure the package compiled without errors.

---

## 2. Depth and Normals Texture Requirements

The **Screen-Space** outline mode requires depth and normals data from the camera.

### Enable Depth Texture

1. Select your **URP Asset** (`Assets/Settings/UniversalRenderPipelineAsset.asset`)
2. Under **General**, enable **Depth Texture**

### Enable Opaque Texture

Enable **Opaque Texture** on the same URP Asset for best screen-space results.

### Normals Prepass

The `ScreenSpaceOutline` shader uses `DeclareNormalsTexture.hlsl` which requires the depth-normals prepass. This is automatically triggered by URP 12+ when `SampleSceneNormals` is called in a render pass.

> If screen-space normals appear black, confirm URP version ≥ 12 and that no other pass is consuming the normals buffer before this one.

---

## 3. Scene Setup

### OutlineManager

Add an empty GameObject to your scene and attach the `OutlineManager` component.

- Singleton — only one instance needed per project
- Persists across scene loads via `DontDestroyOnLoad`
- `DemoSceneSetup` creates it automatically if missing

### OutlineObject

Add `OutlineObject` to any GameObject with:
- `MeshRenderer` + `MeshFilter` — static 3D meshes
- `SkinnedMeshRenderer` — animated characters
- `SpriteRenderer` — 2D sprites

| Property | Description |
|----------|-------------|
| Enabled | Toggle outline without removing the component |
| Outline Mode | `Outline3D`, `ScreenSpace`, or `Outline2D` |
| Color | RGBA outline color |
| Thickness | 0–20 range |

---

## 4. Outline Mode Reference

| Mode | Technique | Best For |
|------|-----------|----------|
| `Outline3D` | Inverted hull — back-face vertex expansion | Opaque 3D meshes |
| `ScreenSpace` | Roberts cross on depth + normals | Full-scene stylised edge look |
| `Outline2D` | Alpha neighbour sampling | Sprites, UI elements |

---

## 5. Common Mistakes

| Problem | Cause | Fix |
|---------|-------|-----|
| No outline visible | Renderer Feature not added | Add `OutlineRenderFeature` to URP Renderer |
| No outline visible | `OutlineManager` missing | Add `OutlineManager` to a scene GameObject |
| Screen-space all black | Depth texture disabled | Enable **Depth Texture** on URP Asset |
| Screen-space normals missing | Normals prepass not running | Ensure URP ≥ 12; check no pass consumes normals first |
| 2D outline not showing | Wrong mode | Set `OutlineObject.Mode` to `Outline2D` |
| Outline flickers | Multiple `OutlineManager` instances | Ensure only one `OutlineManager` in the scene |
| Shader not found | Shaders not imported | Re-import package; run **Tools → Advanced Outline System → Validate Setup** |

---

## 6. Performance Guidelines

- **Outline3D** is cheapest — one extra back-face pass per registered mesh only
- **ScreenSpace** blits the full screen — use for a global stylised look, not per-object
- **Outline2D** samples 4 neighbours per pixel — lightweight for sprites
- Disable `OutlineObject.OutlineEnabled` at runtime instead of destroying the component
- Avoid changing `OutlineMode` at runtime frequently — triggers list re-bucketing in `OutlineManager`
