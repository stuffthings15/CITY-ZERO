# Compile Cleanup Checklist

Use this after importing all earlier packs.

## Highest-risk integration points
- `PlayerController` input callback hookup
- `VehicleDriverInputRelay` action map switching
- `MissionFactory` and `MissionManager` references
- `MissionEventRelay` lifecycle cleanup
- `ConfigDatabase` JSON assignment
- TMP namespace availability
- Input System package present and enabled

## First pass actions
1. Verify package installation:
   - Input System
   - TextMeshPro
   - Cinemachine
2. Confirm asmdefs are placed in folders that match actual code layout
3. Let Unity reimport `.inputactions`
4. Open console and resolve namespace/reference errors in this order:
   - Core
   - Data
   - Gameplay
   - UI
   - Editor
5. Create temporary bootstrap scene and test:
   - `GameBootstrap`
   - `ConfigDatabase`
   - `MissionManager`
   - `MissionFactory`
   - `HeatSystem`
   - `SaveManager`

## Common fixes
- Replace `linearVelocity` with `velocity` if your Unity version requires it
- Reassign TMP references after prefab creation
- Ensure trigger colliders have at least one Rigidbody in overlap pair
- Verify PlayerInput action names match exactly

## Smoke test order
1. Player moves and aims
2. Fire input calls `WeaponMotor`
3. Interact prompt appears near vehicle
4. Enter/exit vehicle works
5. Mission trigger starts mission
6. Debug menu toggles
7. Save/load restores player position
