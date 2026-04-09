# Advanced Outline System (URP)

A high-performance, zero-allocation outline rendering system for Unity's Universal Render Pipeline.

---

## Features

- **3D Outline** — Inverted-hull technique. Expands vertices along normals, renders back-faces only. Works on `MeshRenderer` and `SkinnedMeshRenderer`.
- **Screen-Space Outline** — Full-screen depth + normal edge detection using the Roberts cross operator.
- **2D Outline** — Alpha-based edge detection for `SpriteRenderer`.
- **Zero per-frame allocations** — All property overrides use `MaterialPropertyBlock`. No material duplication.
- **Per-object control** — Color, thickness, and enable/disable independently per object.
- **Custom Inspector** — Sliders, color picker, mode selector, and contextual help per mode.
- **URP only** — No HDRP, no Built-in pipeline dependencies.

---

## Requirements

| Requirement | Minimum Version |
|-------------|----------------|
| Unity | 2021.3 LTS |
| Universal Render Pipeline | 12.0.0 |

---

## Installation

### Via Unity Package Manager — Git URL

1. Open **Window → Package Manager**
2. Click **+** → **Add package from git URL**
3. Enter:
   ```
   https://github.com/MostafaMahmoudAbdelrahman/AdvancedOutlineSystem.git
   ```
4. Click **Add**

### Via .unitypackage

1. Download `AdvancedOutlineSystem.unitypackage`
2. In Unity: **Assets → Import Package → Custom Package**
3. Select the file and click **Import All**

---

## URP Setup

### 1. Add the Renderer Feature

1. Locate your **URP Renderer** asset (e.g. `Assets/Settings/UniversalRenderer.asset`)
2. Select it in the Inspector
3. Click **Add Renderer Feature** at the bottom
4. Choose **Outline Render Feature**

### 2. Enable Depth and Normals Textures

1. Select your **URP Asset** (e.g. `Assets/Settings/UniversalRenderPipelineAsset.asset`)
2. Enable **Depth Texture** under the General section
3. Enable **Opaque Texture** (recommended for screen-space mode)

---

## Quick Start

### 1. Add OutlineManager to your scene

Create an empty GameObject and add the `OutlineManager` component.
It is a persistent singleton (`DontDestroyOnLoad`).

### 2. Add outlines to objects

Add the `OutlineObject` component to any GameObject with a `MeshRenderer`, `SkinnedMeshRenderer`, or `SpriteRenderer`.

Configure in the Inspector:
- **Enabled** — toggle on/off
- **Outline Mode** — `Outline3D`, `ScreenSpace`, or `Outline2D`
- **Color** — RGBA outline color
- **Thickness** — 0 to 20

---

## Usage Example

```csharp
using AdvancedOutlineSystem;
using UnityEngine;

public class HighlightOnHover : MonoBehaviour
{
    private OutlineObject _outline;

    private void Awake() => _outline = GetComponent<OutlineObject>();

    private void OnMouseEnter()
    {
        _outline.OutlineColor     = Color.yellow;
        _outline.OutlineThickness = 5f;
        _outline.OutlineEnabled   = true;
    }

    private void OnMouseExit() => _outline.OutlineEnabled = false;
}
```

---

## Documentation

| File | Contents |
|------|----------|
| [SetupGuide.md](SetupGuide.md) | URP setup, depth/normal requirements, common mistakes |
| [API.md](API.md) | Full public API reference with code examples |

---

## Author

Mostafa Mahmoud Abdelrahman © 2026 All rights reserved.
Portfolio: https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/

## License

See [LICENSE.md](../LICENSE.md) — usage subject to the Unity Asset Store EULA.
