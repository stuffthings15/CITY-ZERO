# Player Prefab Blueprint

## Root
`PF_Player`

Components:
- Rigidbody
- CapsuleCollider
- PlayerController
- InteractableDetector
- HealthComponent
- WeaponMotor

## Children
- `VisualRoot`
- `AimPivot`
  - `FirePoint`
- `Detector`
  - SphereCollider (trigger)
- `CameraTarget`

## Notes
- Freeze X/Z rotation on Rigidbody
- Use top-down capsule or simplified humanoid mesh
- Place `FirePoint` on the forward-right axis used by `WeaponMotor`
