# Known Overlaps and Decisions

## Intentional overlaps
Several packages introduced revised versions of the same conceptual systems.

### Missions
- Earlier packages include `MissionRuntime`, `MissionManager`, and `MissionFactory`
- Later packages include `MissionRuntimeSafe`, `MissionManagerSafe`, and `MissionFactorySafe`

Recommendation:
- Use the `Safe` variants for new scene wiring

### Vehicles
- Earlier packages include `VehicleController`
- Later packages include `VehicleControllerClean`

Recommendation:
- Use `VehicleControllerClean` for new prefabs

### HUD
- Earlier packages include `HudPresenter`
- Later packages include `HudPresenterSafe` and `VerticalSliceHudPresenter`

Recommendation:
- Use `VerticalSliceHudPresenter` for the first playable loop
- Use `HudPresenterSafe` for more general gameplay HUD scaffolding

### Player integration
- Some later files assume partial extensions or adapter-style bridging
- Check the `Scene and Cleanup Pack` notes before merging these into a single production codebase

Recommendation:
- Treat those files as reference scaffolds, not final architecture

## Conflict preservation
If two files had the same path but different contents, the later incoming file was preserved with:
`__from_<package>`
in the filename.

Search for:
`__from_`
to find these.
