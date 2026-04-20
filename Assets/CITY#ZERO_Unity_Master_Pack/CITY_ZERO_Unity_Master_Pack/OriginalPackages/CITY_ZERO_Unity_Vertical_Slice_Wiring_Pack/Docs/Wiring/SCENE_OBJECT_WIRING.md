# Scene Object Wiring

## Bootstrap scene
- GameBootstrap
- ConfigDatabase
- SaveManager
- HeatSystem
- MissionManagerSafe
- MissionFactorySafe
- VerticalSliceGameFlow

## Sandbox scene
- Player prefab
- HUD canvas with `VerticalSliceHudPresenter`
- camera rig

## Old Quarter district scene
- `DistrictBootstrapper`
- `DistrictSpawnRegistry`
- `MissionTriggerVolume`
- `MissionPickupTrigger`
- `MissionDeliveryTrigger`
- `WorldMarker` objects for shop, garage, safehouse
- `SimpleSafehouseTrigger`

## Reference wiring
- `VerticalSliceGameFlow` needs:
  - `MissionFactorySafe`
  - `MissionManagerSafe`
  - player transform
  - optional `SaveManager`
- pickup and delivery triggers need matching event IDs from mission JSON targets
