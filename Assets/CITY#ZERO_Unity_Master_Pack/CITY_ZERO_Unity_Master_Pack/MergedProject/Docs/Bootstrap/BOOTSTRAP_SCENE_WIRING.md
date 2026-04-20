# Bootstrap Scene Wiring

## Minimum bootstrap scene objects
- `GameBootstrap`
- `ConfigDatabase`
- `SaveManager`
- `MissionManager`
- `MissionFactory`
- `HeatSystem`
- optional `DebugCommandRegistry`

## Suggested scene split
### SCN_Bootstrap
Persistent managers only.

### SCN_MainMenu
- EventSystem
- InputSystemUIInputModule
- title UI
- settings UI

### SCN_Sandbox_Global
- player spawn
- HUD canvas
- camera rig
- district-independent systems

### SCN_District_OldQuarter
- roads
- buildings
- traffic lanes
- mission triggers
- NPC spawn points

## Wiring order
1. `GameBootstrap` loads configs
2. `MissionFactory` references `ConfigDatabase`, `HeatSystem`, and player transform
3. `MissionManager` references `ConfigDatabase`
4. `HudPresenter` or `MissionUIBinder` references mission and heat systems
5. `SaveManager` references player transform
