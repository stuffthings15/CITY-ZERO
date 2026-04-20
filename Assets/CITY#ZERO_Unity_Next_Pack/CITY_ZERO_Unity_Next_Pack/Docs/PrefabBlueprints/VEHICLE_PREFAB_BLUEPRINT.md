# Vehicle Prefab Blueprint

## Root
`PF_Vehicle_[Class]_[Variant]`

Components:
- Rigidbody
- BoxCollider or compound colliders
- VehicleController
- optional TrafficVehicleAgent
- optional VehicleDriverInputRelay

## Children
- `VisualRoot`
- `DriverSeat`
- `ExitPoint_Left`
- `ExitPoint_Right`
- `DamageFXRoot`

## Notes
- Center of mass slightly below model center
- Freeze X/Z rotation on Rigidbody
- Keep silhouette readable from top-down
