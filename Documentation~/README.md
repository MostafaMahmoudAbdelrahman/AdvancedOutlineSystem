# Advanced Outline System (URP)

High-performance 2D/3D outline rendering system for Unity's Universal Render Pipeline.

**Author:** Mostafa Mahmoud Abdelrahman © 2026  
**Portfolio:** https://mostafamahmoudabdelrahman.github.io/mostafa-mahmoud-portfolio/

---

## Features

| Feature | Description |
|---------|-------------|
| 3D Outline | Inverted-hull — expands vertices along normals, renders back-faces only |
| Screen-Space Outline | Full-screen depth + normal edge detection (Roberts cross) |
| 2D Outline | Alpha-based edge detection for SpriteRenderer |
| Zero allocations | MaterialPropertyBlock — no material duplication per frame |
| Per-object control | Color, thickness, enable/disable, mode — all per object |
| Custom Inspector | Sliders, color picker, mode selector with contextual help |
| Welcome Window | Auto-opens on import with quick-start guide |
| In-editor Docs | Full documentation browser inside Unity |

---

## Requirements

| Requirement | Minimum |
|-------------|---------|
| Unity | 2021.3 LTS |
| Universal Render Pipeline | 12.0.0 |

> **Unity 6 users:** Enable Compatibility Mode  
> Project Settings → Graphics → Render Graph → Compatibility Mode (Render Graph Disabled)

---

## Installation

### Via Unity Package Manager — Git URL

1. Open **Window → Package Manager**
2. Click **+** → **Add package from git URL**
3. Enter:
   ```
   https://github.com/MostafaMahmoudAbdelrahman/AdvancedOutlineSystem.git#v1.0.7
   ```
4. Click **Add**

A Welcome Window opens automatically after import.

---

## Quick Setup

### 1. Add the Renderer Feature

1. Select `Assets/Settings/UniversalRenderer.asset`
2. Inspector → **Add Renderer Feature** → **Outline Render Feature**

### 2. Add OutlineManager

Create an empty GameObject → **Add Component → OutlineManager**

### 3. Add outlines to objects

Select any GameObject with a `MeshRenderer`, `SkinnedMeshRenderer`, or `SpriteRenderer`  
→ **Add Component → OutlineObject** → configure in Inspector

---

## Runtime Usage

```csharp
using AdvancedOutlineSystem;

var outline = GetComponent<OutlineObject>();

// Enable with color
outline.OutlineColor     = Color.yellow;
outline.OutlineThickness = 5f;
outline.OutlineEnabled   = true;

// Disable
outline.OutlineEnabled = false;

// Change mode
outline.Mode = OutlineMode.ScreenSpace;
```

---

## In-Editor Documentation

After installing, open:  
**Tools → Advanced Outline System → Documentation**

Covers: How To Use, Setup Guide, API Reference, Troubleshooting.

---

## Documentation Files

| File | Contents |
|------|----------|
| [SetupGuide.md](SetupGuide.md) | URP setup, depth/normal requirements, shadow atlas |
| [API.md](API.md) | Full public API with code examples |

---

## License

See [LICENSE.md](../LICENSE.md) — usage subject to the Unity Asset Store EULA.
