# Master Import Order

## First pass
1. Create a fresh Unity project
2. Install required packages:
   - Input System
   - TextMeshPro
   - Cinemachine
   - AI Navigation
   - URP
3. Copy in `MergedProject/Assets/_Project/`
4. Copy in `MergedProject/Packages/manifest.recommended.json` as reference only
5. Reopen Unity and allow asset import

## Second pass
1. Import or create config JSONs
2. Place asmdef files where needed
3. Import `CITY_ZERO.inputactions`
4. Generate PlayerInput bindings or connect action maps manually
5. Create bootstrap, sandbox, and district scenes from templates

## Third pass
1. Create prefabs from blueprint docs
2. Wire player, vehicles, HUD, mission systems
3. Run config and scene validation tools
4. Test the sample mission loop `mq_a1_03_dead_drop_dive`

## Best-practice merge decisions
- Prefer `MissionManagerSafe` over the earlier mission manager
- Prefer `MissionFactorySafe` over the earlier mission factory
- Prefer `VehicleControllerClean` over the earlier vehicle controller for new work
- Keep old files only as fallback references
