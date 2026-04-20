# Minimap Blueprint

This document describes a basic minimap implementation suitable for a top‑down open world game. The minimap provides spatial awareness, showing the player’s position, nearby objectives, enemies, and other points of interest.

## Implementation Options

There are two common approaches to implementing a minimap:

1. **Secondary Camera + Render Texture** – A camera renders a simplified version of your world onto a `RenderTexture` which is displayed in the UI. This is performant and requires no custom meshes, but requires managing layers and culling.
2. **UI Overlay** – Convert world positions to minimap coordinates via math and draw icons directly on the UI. This is more flexible for stylized maps, but requires manual translation and scaling.

The `MinimapController` script in this pack assumes the second approach for simplicity. It takes world positions and places icons on the minimap panel.

## Prefab Hierarchy

```
MinimapCanvas (Canvas)
├── MinimapPanel (Image)
├── PlayerIcon (Image)
├── POIContainer (RectTransform)
│   ├── [Dynamic POI Instances] (Image)
└── Mask (RectMask2D or Image with Mask component)
```

### Notes

- **MinimapCanvas** – A dedicated canvas with a render mode of **Screen Space – Overlay**. You can anchor it to the corner of the screen.
- **MinimapPanel** – The background for the minimap. Use a simple circular or rectangular sprite.
- **PlayerIcon** – An image representing the player. It rotates based on the player’s orientation.
- **POIContainer** – Contains icons for points of interest (objectives, enemies, shops). The `MinimapController` will instantiate icon prefabs under this container.
- **Mask** – Use `RectMask2D` to clip icons within the minimap bounds. For circular maps, use an `Image` with a mask.

## Scripting

Attach the `MinimapController` to the **MinimapCanvas**. Assign references to the necessary components and provide a prefab for POI icons (e.g. a small `Image` with a sprite for each type). The script includes methods for registering and unregistering POIs at runtime.

Example usage:

```csharp
public class GameBootstrap : MonoBehaviour
{
    public MinimapController minimap;
    public Transform player;

    void Start()
    {
        minimap.SetPlayer(player);
    }
}

// When spawning an objective or enemy:
minimap.RegisterPOI(targetTransform, enemyIconSprite);

// When the POI is destroyed or completed:
minimap.UnregisterPOI(targetTransform);
```

You will need to supply your own sprites for player and POI icons.