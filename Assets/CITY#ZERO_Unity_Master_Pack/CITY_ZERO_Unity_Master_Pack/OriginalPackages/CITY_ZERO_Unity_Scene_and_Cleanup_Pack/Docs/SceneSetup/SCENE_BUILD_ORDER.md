# Scene Build Order

## 1. SCN_Bootstrap
Add:
- `GameBootstrap`
- `ConfigDatabase`
- `HeatSystem`
- `SaveManager`
- `MissionManager`
- `MissionFactory`
- optional `DebugCommandRegistry`

## 2. SCN_MainMenu
Add:
- `EventSystem`
- `InputSystemUIInputModule`
- menu canvas
- settings panel

## 3. SCN_Sandbox_Global
Add:
- `PF_Player`
- camera rig
- HUD canvas
- debug canvas
- global light
- world origin helpers

## 4. SCN_District_OldQuarter
Add:
- road kit
- buildings
- traffic lanes
- vehicle spawn points
- civilian spawn points
- police patrol points
- mission trigger volumes
- shop markers
- safehouse marker

## Suggested additive flow
- open `SCN_Bootstrap`
- load `SCN_Sandbox_Global`
- load district scene additively
