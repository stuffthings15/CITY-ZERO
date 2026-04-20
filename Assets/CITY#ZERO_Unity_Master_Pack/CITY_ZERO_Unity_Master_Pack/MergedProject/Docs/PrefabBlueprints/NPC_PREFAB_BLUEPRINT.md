# NPC Prefab Blueprint

## Root
`PF_NPC_[FactionOrCivilian]_[Variant]`

Components:
- CapsuleCollider
- HealthComponent
- one of:
  - CivilianAIController
  - PoliceAIController
  - FactionAgentController

## Children
- `VisualRoot`
- `HeadMarker`
- `WeaponSocket`
- `DebugLabelAnchor`

## Notes
- Police and faction prefabs can also receive `WeaponMotor`
- Attach `AIDebugBillboard` to a world-space label object for debug mode
