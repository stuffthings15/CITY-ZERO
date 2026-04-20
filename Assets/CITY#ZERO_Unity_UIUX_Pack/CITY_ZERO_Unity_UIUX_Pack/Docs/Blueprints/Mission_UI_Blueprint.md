# Mission UI Blueprint

The mission UI displays the player’s current objective, mission title, and optionally progress counters. This blueprint focuses on a simple overlay integrated into the HUD or as a separate panel.

## Prefab Hierarchy

```
MissionUIPanel (RectTransform)
├── TitleText (TextMeshProUGUI)
├── ObjectiveText (TextMeshProUGUI)
└── ProgressText (TextMeshProUGUI) [optional]
```

### Notes

- **MissionUIPanel** – Anchored in a screen corner or along the top edge. Use a background image or color block if you want it distinct from the HUD.
- **TitleText** – Displays the mission’s name. Use bold or larger font size for emphasis.
- **ObjectiveText** – Shows the current objective description. Update this as objectives change during the mission.
- **ProgressText** – Optional field for displaying counters (e.g. “Enemies remaining: 3/10” or “Packages delivered: 2/5”). Hide when not needed.

## Scripting

The `MissionUI` script provided in this pack manages the mission UI elements. It exposes methods for setting the title, objective, and progress. It is designed to work with your mission system.

Example:

```csharp
MissionUI missionUI = FindObjectOfType<MissionUI>();
missionUI.SetTitle("Smuggling Run");
missionUI.SetObjective("Deliver the packages to the drop points.");
missionUI.SetProgress(0, 5);

// Later, after delivering a package:
missionUI.SetProgress(1, 5);

// When mission ends:
missionUI.Clear();
```

You can also hook up the `MissionUI` to a `MissionManager` to automatically update the UI based on mission events. The sample `MissionManager` in previous packs includes events like `OnObjectiveUpdated` and `OnProgressChanged`.