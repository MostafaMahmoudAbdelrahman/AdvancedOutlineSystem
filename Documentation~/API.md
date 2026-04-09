# API Reference

## OutlineObject

`MonoBehaviour` — attach to any GameObject to enable outline rendering.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `OutlineColor` | `Color` | White | The outline color (RGBA) |
| `OutlineThickness` | `float` | 3 | Outline width, clamped 0–20 |
| `OutlineEnabled` | `bool` | true | Enable or disable the outline |
| `Mode` | `OutlineMode` | Outline3D | Which rendering technique to use |

### Events

| Event | Signature | Fired When |
|-------|-----------|------------|
| `OnChanged` | `Action` | Any property changes |

### Add and configure at runtime

```csharp
using AdvancedOutlineSystem;
using UnityEngine;

var outline = gameObject.AddComponent<OutlineObject>();
outline.OutlineColor     = Color.cyan;
outline.OutlineThickness = 4f;
outline.Mode             = OutlineMode.Outline3D;
outline.OutlineEnabled   = true;
```

### Toggle on/off

```csharp
// Enable
GetComponent<OutlineObject>().OutlineEnabled = true;

// Disable (component stays, zero render cost)
GetComponent<OutlineObject>().OutlineEnabled = false;
```

### Hover highlight pattern

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
        _outline.OutlineThickness = 6f;
        _outline.OutlineEnabled   = true;
    }

    private void OnMouseExit() => _outline.OutlineEnabled = false;
}
```

### Selection system pattern

```csharp
using AdvancedOutlineSystem;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private OutlineObject _current;

    public void Select(OutlineObject target)
    {
        if (_current != null) _current.OutlineEnabled = false;
        _current = target;
        _current.OutlineColor     = Color.white;
        _current.OutlineThickness = 5f;
        _current.OutlineEnabled   = true;
    }

    public void Deselect()
    {
        if (_current == null) return;
        _current.OutlineEnabled = false;
        _current = null;
    }
}
```

### React to property changes

```csharp
private void OnEnable()  => GetComponent<OutlineObject>().OnChanged += OnOutlineChanged;
private void OnDisable() => GetComponent<OutlineObject>().OnChanged -= OnOutlineChanged;
private void OnOutlineChanged() => Debug.Log("Outline updated.");
```

---

## OutlineManager

`MonoBehaviour` singleton — tracks all active `OutlineObject` instances.

### Access

```csharp
OutlineManager.Instance
```

### Methods

| Method | Description |
|--------|-------------|
| `Register(OutlineObject)` | Add object to the appropriate render list |
| `Unregister(OutlineObject)` | Remove object from all render lists |
| `RefreshObject(OutlineObject)` | Re-bucket after `Mode` changes |

> `OutlineObject` calls these automatically via `OnEnable` / `OnDisable`.

### Read-only lists

```csharp
IReadOnlyList<OutlineObject> Objects3D      // OutlineMode.Outline3D
IReadOnlyList<OutlineObject> ObjectsScreen  // OutlineMode.ScreenSpace
IReadOnlyList<OutlineObject> Objects2D      // OutlineMode.Outline2D
```

### Query active count

```csharp
int total = OutlineManager.Instance.Objects3D.Count
          + OutlineManager.Instance.ObjectsScreen.Count
          + OutlineManager.Instance.Objects2D.Count;
```

---

## OutlineMode (enum)

```csharp
public enum OutlineMode
{
    Outline3D,    // Inverted-hull back-face expansion
    ScreenSpace,  // Full-screen depth + normal edge detection
    Outline2D     // Sprite alpha-based edge detection
}
```

---

## OutlineRenderFeature

`ScriptableRendererFeature` — add to your URP Renderer asset.

### Settings

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| `renderPassEvent` | `RenderPassEvent` | `BeforeRenderingPostProcessing` | Injection point in the URP frame |
| `defaultThickness` | `float` | 3 | Inspector default (not used per-object) |
| `defaultColor` | `Color` | White | Inspector default (not used per-object) |

### Render passes injected

| Pass | Condition | Description |
|------|-----------|-------------|
| `OutlineRenderPass` | `Objects3D.Count > 0` | Inverted-hull 3D outlines |
| `ScreenSpaceOutlinePass` | `ObjectsScreen.Count > 0` | Full-screen blit edge detection |
| `Outline2DPass` | `Objects2D.Count > 0` | Per-sprite alpha edge detection |

---

## Shaders

| Shader | Path |
|--------|------|
| 3D Outline | `AdvancedOutlineSystem/Outline3D` |
| 2D Outline | `AdvancedOutlineSystem/Outline2D` |
| Screen-Space | `AdvancedOutlineSystem/ScreenSpaceOutline` |

All shaders use URP ShaderLibrary includes and compile on URP 12+.
