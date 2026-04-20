# Unity System Implementation Notes

## Player Controller
Use CharacterController or Rigidbody-based top-down movement.
For MVP, Rigidbody with custom planar movement is preferred for collision feel.

Core features:
- Move vector from Input System
- Aim vector from mouse or right-stick
- Sprint modifier
- Roll with invulnerability window tuning
- Interact priority system
- Weapon fire delegated to equipped weapon component

## Camera
Use Cinemachine virtual camera:
- Small dead zone
- Low damping
- Lookahead based on move direction
- Optional slight offset toward aim vector
- Screen shake impulse source on impact and explosions

## Combat
- Hitscan for pistols, SMGs, revolvers
- Pseudo-spread hitscan for shotguns in MVP
- Melee arc traces
- Separate `Damageable`, `Armorable`, `FactionAffiliation`, `ThreatEmitter` components

## Vehicle Controller
For MVP:
- Simplified top-down arcade forces
- Acceleration force on local forward axis
- Steering modifies angular velocity and grip curve
- Traction reduction based on surface tag
- Handbrake temporarily drops lateral grip

## Heat System
Core inputs:
- `crimeSeverity`
- `witnessCount`
- `districtHeatMultiplier`
- `cameraDetected`
- `policeLineOfSight`

Escalation loop:
1. Crime generates incident score
2. Witnesses and cameras send reports
3. Dispatch creates incident entry
4. Nearest patrols assigned
5. Active line of sight pushes heat up over time
6. Losing line of sight decays search state gradually

## AI
Use state-machine plus utility scoring for MVP.

Civilian states:
- Wander
- IdleUseProp
- Panic
- Flee
- CallPolice
- Cower
- ReturnToRoutine

Police states:
- Patrol
- Investigate
- PursueOnFoot
- PursueVehicle
- CombatContain
- ArrestAttempt
- SearchLastKnown
- ReturnPatrol

Faction states:
- PatrolTerritory
- GuardLocation
- AttackRival
- AttackPlayer
- Fallback
- CallReinforcements
- HoldChokepoint

## Mission System
Use data-driven mission graphs with code-backed objective handlers.

Core classes:
- `MissionManager`
- `MissionRuntime`
- `MissionDefinition`
- `MissionObjective`
- `MissionTrigger`
- `MissionRewardProcessor`

Objective handler types:
- Reach location
- Steal vehicle
- Collect item
- Deliver item
- Survive timer
- Kill target
- Destroy target
- Escape heat
- Hold zone
- Follow route
