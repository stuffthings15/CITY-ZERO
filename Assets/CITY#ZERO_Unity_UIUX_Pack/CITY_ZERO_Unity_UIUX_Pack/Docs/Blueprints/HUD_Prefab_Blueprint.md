# HUD Prefab Blueprint

This document describes the recommended structure for the Heads‑Up Display (HUD) prefab. The HUD displays real‑time information like health, armor, ammunition, current objective, cash, and heat level. Use this as a starting point and adjust to suit your art direction.

## Prefab Hierarchy

```
HUDCanvas (Canvas)
├── Panel (RectTransform)
│   ├── HealthBar (Image)
│   ├── ArmorBar (Image)
│   ├── AmmoGroup (RectTransform)
│   │   ├── AmmoIcon (Image)
│   │   └── AmmoText (TextMeshProUGUI)
│   ├── HeatMeter (Image)
│   ├── CashGroup (RectTransform)
│   │   ├── CashIcon (Image)
│   │   └── CashText (TextMeshProUGUI)
│   └── ObjectiveText (TextMeshProUGUI)
└── DebugPanel (optional)
```

### Notes

- **HUDCanvas** – A `Canvas` component set to **Screen Space – Overlay** with a canvas scaler configured for your reference resolution (e.g. 1920×1080). It should also include a `GraphicRaycaster` if interactive UI elements are present.
- **Panel** – A top‑level `RectTransform` that positions and anchors the HUD elements. Use `Vertical Layout Group` or manual layout to arrange the bars and groups.
- **HealthBar / ArmorBar / HeatMeter** – Use `Image` components with filled type set to `Filled` (horizontal or radial depending on your design). The `HUDManager` script will modify the `fillAmount` of these images.
- **AmmoGroup** – Contains an icon and text to show current weapon ammo and total ammunition. Consider splitting the current clip and reserve count into separate text fields.
- **CashGroup** – Shows the player’s cash/influence. Use your preferred currency symbol or style.
- **ObjectiveText** – Displays the current mission or objective description. Use a `TextMeshProUGUI` component for crisp text rendering.
- **DebugPanel** – Optional area for developer debug information (e.g. FPS, memory usage, heat value). Remove in production builds.

## Scripting

Attach the `HUDManager` script to the **HUDCanvas** object. In the inspector, assign the references to each `Image` and `TextMeshProUGUI` field. The script provides public methods to update each element individually or as a group.

Example:

```csharp
public class GameBootstrap : MonoBehaviour
{
    public HUDManager hud;
    public PlayerController player;

    void Update()
    {
        hud.UpdateHealth(player.HealthNormalized);
        hud.UpdateArmor(player.ArmorNormalized);
        hud.UpdateAmmo(player.CurrentClip, player.ReserveAmmo);
        hud.UpdateHeat(player.HeatLevelNormalized);
        hud.UpdateCash(player.Cash);
        hud.UpdateObjective(MissionManager.Instance.CurrentObjective);
    }
}
```

Keep the design flexible. You can rearrange, resize, or hide elements during certain game states (e.g. hide cash and objective during cutscenes).